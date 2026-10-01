using UserManagement.API.Models.DTOs;

namespace UserManagement.API.Services
{
    public interface IPermissionService
    {
        List<PermissionGroupDto> GetAllPermissions();
    }
}
