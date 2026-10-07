using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Models.DTOs;
using UserManagement.API.Models.Entities;

namespace UserManagement.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDtos = new List<RoleResponseDto>();

            foreach (var role in roles)
            {
                roleDtos.Add(await MapToRoleResponseDtoAsync(role));
            }

            return roleDtos;
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return null;

            return await MapToRoleResponseDtoAsync(role);
        }

        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto request)
        {
            var roleExists = await _roleManager.RoleExistsAsync(request.Name);
            if (roleExists)
            {
                throw new InvalidOperationException($"Role '{request.Name}' already exists.");
            }

            var role = new ApplicationRole(request.Name, request.Description)
            {
                CreatedAt = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role creation failed: {errors}");
            }

            if (request.Permissions.Any())
            {
                foreach (var permission in request.Permissions.Distinct())
                {
                    await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                }
            }

            return await MapToRoleResponseDtoAsync(role);
        }

        public async Task<RoleResponseDto?> UpdateRoleAsync(string id, UpdateRoleDto request)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return null;

            role.Name = request.Name;
            role.Description = request.Description;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role update failed: {errors}");
            }

            return await MapToRoleResponseDtoAsync(role);
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> ManageRolePermissionsAsync(string id, List<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return false;

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var permissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();

            foreach (var claim in permissionClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permission in permissions.Distinct())
            {
                await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
            }

            return true;
        }

        private async Task<RoleResponseDto> MapToRoleResponseDtoAsync(ApplicationRole role)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                UpdatedAt = role.UpdatedAt,
                UpdatedBy = role.UpdatedBy,
                IsDeleted = role.IsDeleted,
                DeletedAt = role.DeletedAt,
                DeletedBy = role.DeletedBy,
                Permissions = permissions
            };
        }
    }
}
