using UserManagement.API.Models.DTOs;

namespace UserManagement.API.Services
{
    public interface IRoleService
    {
        Task<List<RoleResponseDto>> GetAllRolesAsync();
        Task<RoleResponseDto?> GetRoleByIdAsync(string id);
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto request);
        Task<RoleResponseDto?> UpdateRoleAsync(string id, UpdateRoleDto request);
        Task<bool> DeleteRoleAsync(string id);
        Task<bool> ManageRolePermissionsAsync(string id, List<string> permissions);
    }
}
