using System.Security.Claims;

namespace PlanService.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            var sub = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? httpContext.User.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }

        public static bool IsTeacher(this HttpContext httpContext)
        {
            return httpContext.User.IsInRole("TEACHER");
        }
    }
}