using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Authorization;
using UserManagement.API.Models.Enums;
using UserManagement.API.Services;

namespace UserManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [HasPermission(SystemPermissions.Permissions.View)]
        public IActionResult GetAllPermissions()
        {
            var permissions = _permissionService.GetAllPermissions();
            return Ok(permissions);
        }
    }
}
