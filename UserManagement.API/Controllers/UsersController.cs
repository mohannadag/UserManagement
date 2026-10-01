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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [HasPermission(SystemPermissions.Users.View)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [HasPermission(SystemPermissions.Users.View)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });
            return Ok(user);
        }

        [HttpPost]
        [HasPermission(SystemPermissions.Users.Create)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [HasPermission(SystemPermissions.Users.Edit)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, request);
                if (user == null) return NotFound(new { message = "User not found." });
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [HasPermission(SystemPermissions.Users.Delete)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(new { message = "User not found or could not be deleted." });
            return Ok(new { message = "User deleted successfully." });
        }

        [HttpPost("{id}/roles")]
        [HasPermission(SystemPermissions.Users.AssignRoles)]
        public async Task<IActionResult> AssignRoles(string id, [FromBody] AssignUserRolesDto request)
        {
            var success = await _userService.AssignRolesToUserAsync(id, request.Roles);
            if (!success) return BadRequest(new { message = "Failed to update user roles." });
            return Ok(new { message = "User roles updated successfully." });
        }

        [HttpPost("{id}/permissions")]
        [HasPermission(SystemPermissions.Users.AssignPermissions)]
        public async Task<IActionResult> AssignPermissions(string id, [FromBody] AssignUserPermissionsDto request)
        {
            var success = await _userService.AssignPermissionsToUserAsync(id, request.Permissions);
            if (!success) return BadRequest(new { message = "Failed to update user permissions." });
            return Ok(new { message = "User permissions updated successfully." });
        }

        [HttpPost("{id}/toggle-status")]
        [HasPermission(SystemPermissions.Users.ToggleStatus)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var success = await _userService.ToggleUserStatusAsync(id);
            if (!success) return NotFound(new { message = "User not found." });
            return Ok(new { message = "User status toggled successfully." });
        }
    }
}
