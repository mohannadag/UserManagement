using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Models.DTOs
{
    public class RoleResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class CreateRoleDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<string> Permissions { get; set; } = new();
    }

    public class UpdateRoleDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class ManageRolePermissionsDto
    {
        [Required]
        public List<string> Permissions { get; set; } = new();
    }
}
