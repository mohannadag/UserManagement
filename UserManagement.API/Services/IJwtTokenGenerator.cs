using System.Security.Claims;
using UserManagement.API.Models.Entities;

namespace UserManagement.API.Services
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
