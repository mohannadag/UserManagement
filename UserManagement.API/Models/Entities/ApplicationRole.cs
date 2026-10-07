using Microsoft.AspNetCore.Identity;
using UserManagement.API.Models.Interfaces;

namespace UserManagement.API.Models.Entities
{
    public class ApplicationRole : IdentityRole, IAuditable, ISoftDelete
    {
        public string? Description { get; set; }

        // IAuditable Implementation
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // ISoftDelete Implementation
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName, string? description = null) : base(roleName)
        {
            Description = description;
        }
    }
}
