using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UserManagement.API.Models.DTOs;
using UserManagement.API.Models.Entities;

namespace UserManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists."
                };
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {errors}"
                };
            }

            // Assign default "User" role if it exists
            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return await GenerateAuthResponseForUserAsync(user, "User registered successfully.");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto { Success = false, Message = "Account is disabled. Please contact administrator." };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };
            }

            return await GenerateAuthResponseForUserAsync(user, "Login successful.");
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            ClaimsPrincipal? principal;
            try
            {
                principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
            }
            catch
            {
                return new AuthResponseDto { Success = false, Message = "Invalid access token." };
            }

            if (principal == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid token principal." };
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponseDto { Success = false, Message = "Invalid token claims." };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token." };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto { Success = false, Message = "Account is disabled." };
            }

            return await GenerateAuthResponseForUserAsync(user, "Token refreshed successfully.");
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.Succeeded;
        }

        private async Task<AuthResponseDto> GenerateAuthResponseForUserAsync(ApplicationUser user, string message)
        {
            var accessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshTokenExpiryDays = Convert.ToDouble(_configuration["JwtSettings:RefreshTokenExpiryInDays"] ?? "7");
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            await _userManager.UpdateAsync(user);

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);
            var permissions = userClaims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

            return new AuthResponseDto
            {
                Success = true,
                Message = message,
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = user.RefreshTokenExpiryTime,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles,
                    Permissions = permissions
                }
            };
        }
    }
}
