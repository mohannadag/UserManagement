using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Models.DTOs;
using UserManagement.API.Models.Entities;

namespace UserManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserResponseDto>();

            foreach (var user in users)
            {
                userDtos.Add(await MapToUserResponseDtoAsync(user));
            }

            return userDtos;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            return await MapToUserResponseDtoAsync(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
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
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            if (request.Roles.Any())
            {
                foreach (var roleName in request.Roles)
                {
                    if (await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _userManager.AddToRoleAsync(user, roleName);
                    }
                }
            }

            return await MapToUserResponseDtoAsync(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(string id, UpdateUserDto request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UserName = request.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User update failed: {errors}");
            }

            return await MapToUserResponseDtoAsync(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> AssignRolesToUserAsync(string id, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            var validRoles = new List<string>();
            foreach (var roleName in roles)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    validRoles.Add(roleName);
                }
            }

            var addResult = await _userManager.AddToRolesAsync(user, validRoles);
            return addResult.Succeeded;
        }

        public async Task<bool> AssignPermissionsToUserAsync(string id, List<string> permissions)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var existingPermissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();

            foreach (var claim in existingPermissionClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            foreach (var permission in permissions.Distinct())
            {
                await _userManager.AddClaimAsync(user, new Claim("permission", permission));
            }

            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private async Task<UserResponseDto> MapToUserResponseDtoAsync(ApplicationUser user)
        {
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var claims = await _userManager.GetClaimsAsync(user);
            var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

            return new UserResponseDto
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
            };
        }
    }
}
