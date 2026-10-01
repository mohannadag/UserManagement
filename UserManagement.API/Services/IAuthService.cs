using UserManagement.API.Models.DTOs;

namespace UserManagement.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> RevokeTokenAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto request);
    }
}
