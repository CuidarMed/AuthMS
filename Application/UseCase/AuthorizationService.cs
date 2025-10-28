using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.UseCase
{
    /// <summary>
    /// Implementación del servicio de autorización para CuidarMed+
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasRole(string role)
        {
            var user = GetCurrentUser();
            if (user == null) return false;

            return user.IsInRole(role);
        }

        public bool HasAnyRole(params string[] roles)
        {
            var user = GetCurrentUser();
            if (user == null) return false;

            return roles.Any(role => user.IsInRole(role));
        }

        public int? GetCurrentUserId()
        {
            var user = GetCurrentUser();
            if (user == null) return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }

        public string? GetCurrentUserRole()
        {
            var user = GetCurrentUser();
            if (user == null) return null;

            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public bool CanAccessUserData(int targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return false;

            // Los usuarios solo pueden acceder a sus propios datos
            if (IsPatient() || IsDoctor())
            {
                return currentUserId == targetUserId;
            }

            return false;
        }

        public bool IsDoctor()
        {
            return HasRole(UserRoles.Doctor);
        }

        public bool IsPatient()
        {
            return HasRole(UserRoles.Patient);
        }

        public ClaimsPrincipal? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }
    }
}
