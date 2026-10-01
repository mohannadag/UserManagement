namespace UserManagement.API.Models.DTOs
{
    public class PermissionGroupDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public List<PermissionItemDto> Permissions { get; set; } = new();
    }

    public class PermissionItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
