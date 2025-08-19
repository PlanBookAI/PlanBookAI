using Microsoft.AspNetCore.Http;

namespace UserService.Extensions
{
    public static class AuthorizationExtensions
    {
        public static bool IsAuthenticated(this HttpContext context)
        {
            return context.Items.TryGetValue("IsAuthenticated", out var isAuth) && 
                   isAuth is bool auth && auth;
        }

        public static string? GetUserId(this HttpContext context)
        {
            return context.Items.TryGetValue("UserId", out var userId) ? userId?.ToString() : null;
        }

        public static string? GetUserRole(this HttpContext context)
        {
            return context.Items.TryGetValue("UserRole", out var userRole) ? userRole?.ToString() : null;
        }

        public static string? GetUserEmail(this HttpContext context)
        {
            return context.Items.TryGetValue("UserEmail", out var userEmail) ? userEmail?.ToString() : null;
        }

        public static bool HasRole(this HttpContext context, string role)
        {
            var userRole = context.GetUserRole();
            return !string.IsNullOrEmpty(userRole) && 
                   userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasAnyRole(this HttpContext context, params string[] roles)
        {
            var userRole = context.GetUserRole();
            return !string.IsNullOrEmpty(userRole) && 
                   roles.Any(role => userRole.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsAdmin(this HttpContext context)
        {
            return context.HasRole("ADMIN");
        }

        public static bool IsManager(this HttpContext context)
        {
            return context.HasRole("MANAGER");
        }

        public static bool IsStaff(this HttpContext context)
        {
            return context.HasRole("STAFF");
        }

        public static bool IsTeacher(this HttpContext context)
        {
            return context.HasRole("TEACHER");
        }
    }
}
