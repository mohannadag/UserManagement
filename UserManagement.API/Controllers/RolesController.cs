using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Authorization;
using UserManagement.API.Models.DTOs;
using UserManagement.API.Models.Enums;
using UserManagement.API.Services;

namespace UserManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [HasPermission(SystemPermissions.Roles.View)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [HasPermission(SystemPermissions.Roles.View)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound(new { message = "Role not found." });
            return Ok(role);
        }

        [HttpPost]
        [HasPermission(SystemPermissions.Roles.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(request);
                return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [HasPermission(SystemPermissions.Roles.Edit)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleDto request)
        {
            try
            {
                var role = await _roleService.UpdateRoleAsync(id, request);
                if (role == null) return NotFound(new { message = "Role not found." });
                return Ok(role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [HasPermission(SystemPermissions.Roles.Delete)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success) return NotFound(new { message = "Role not found or could not be deleted." });
            return Ok(new { message = "Role deleted successfully." });
        }

        [HttpPost("{id}/permissions")]
        [HasPermission(SystemPermissions.Roles.ManagePermissions)]
        public async Task<IActionResult> ManagePermissions(string id, [FromBody] ManageRolePermissionsDto request)
        {
            var success = await _roleService.ManageRolePermissionsAsync(id, request.Permissions);
            if (!success) return BadRequest(new { message = "Failed to update role permissions." });
            return Ok(new { message = "Role permissions updated successfully." });
        }
    }
}
