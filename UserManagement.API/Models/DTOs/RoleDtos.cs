using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Models.DTOs
{
    public class RoleResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Audit & Soft Delete Properties
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

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
