using UserManagement.API.Models.DTOs;
using UserManagement.API.Models.Enums;

namespace UserManagement.API.Services
{
    public class PermissionService : IPermissionService
    {
        public List<PermissionGroupDto> GetAllPermissions()
        {
            return new List<PermissionGroupDto>
            {
                new PermissionGroupDto
                {
                    ModuleName = "User Management",
                    Permissions = new List<PermissionItemDto>
                    {
                        new PermissionItemDto { Name = "View Users", ClaimValue = SystemPermissions.Users.View, Description = "Allows viewing list and details of users." },
                        new PermissionItemDto { Name = "Create User", ClaimValue = SystemPermissions.Users.Create, Description = "Allows creating new users." },
                        new PermissionItemDto { Name = "Edit User", ClaimValue = SystemPermissions.Users.Edit, Description = "Allows updating user details." },
                        new PermissionItemDto { Name = "Delete User", ClaimValue = SystemPermissions.Users.Delete, Description = "Allows deleting users." },
                        new PermissionItemDto { Name = "Assign User Roles", ClaimValue = SystemPermissions.Users.AssignRoles, Description = "Allows assigning roles to users." },
                        new PermissionItemDto { Name = "Assign User Permissions", ClaimValue = SystemPermissions.Users.AssignPermissions, Description = "Allows assigning direct claims/permissions to users." },
                        new PermissionItemDto { Name = "Toggle User Status", ClaimValue = SystemPermissions.Users.ToggleStatus, Description = "Allows enabling or disabling user accounts." }
                    }
                },
                new PermissionGroupDto
                {
                    ModuleName = "Role Management",
                    Permissions = new List<PermissionItemDto>
                    {
                        new PermissionItemDto { Name = "View Roles", ClaimValue = SystemPermissions.Roles.View, Description = "Allows viewing list and details of roles." },
                        new PermissionItemDto { Name = "Create Role", ClaimValue = SystemPermissions.Roles.Create, Description = "Allows creating new custom roles." },
                        new PermissionItemDto { Name = "Edit Role", ClaimValue = SystemPermissions.Roles.Edit, Description = "Allows modifying role metadata." },
                        new PermissionItemDto { Name = "Delete Role", ClaimValue = SystemPermissions.Roles.Delete, Description = "Allows deleting custom roles." },
                        new PermissionItemDto { Name = "Manage Role Permissions", ClaimValue = SystemPermissions.Roles.ManagePermissions, Description = "Allows configuring permissions assigned to roles." }
                    }
                },
                new PermissionGroupDto
                {
                    ModuleName = "Permissions Catalog",
                    Permissions = new List<PermissionItemDto>
                    {
                        new PermissionItemDto { Name = "View System Permissions", ClaimValue = SystemPermissions.Permissions.View, Description = "Allows viewing available system permission catalog." }
                    }
                }
            };
        }
    }
}
