using UserManagement.API.Models.DTOs;

namespace UserManagement.API.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(string id);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto request);
        Task<UserResponseDto?> UpdateUserAsync(string id, UpdateUserDto request);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> AssignRolesToUserAsync(string id, List<string> roles);
        Task<bool> AssignPermissionsToUserAsync(string id, List<string> permissions);
        Task<bool> ToggleUserStatusAsync(string id);
    }
}
