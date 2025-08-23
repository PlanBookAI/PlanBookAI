using System.Security.Claims;

namespace ExamService.Extensions
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Lấy UserId (dưới dạng Guid) từ claims của người dùng đã được xác thực.
        /// Middleware đã đảm bảo header 'X-User-Id' tồn tại và được thêm vào claims.
        /// </summary>
        /// <param name="context">HttpContext hiện tại.</param>
        /// <returns>Guid của người dùng.</returns>
        /// <exception cref="InvalidOperationException">Ném ra khi không tìm thấy UserId (lỗi logic hoặc request không được xác thực).</exception>
        public static Guid GetUserId(this HttpContext context)
        {
            // 'this HttpContext context' chính là cú pháp để tạo một extension method
            var userIdClaim = context.User.FindFirst("UserId");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            // Nếu request đi đến được đây mà không có UserId, đó là một lỗi hệ thống nghiêm trọng.
            // Gateway đã phải chặn các request không hợp lệ.
            throw new InvalidOperationException("Không thể xác định User ID từ thông tin xác thực.");
        }

        public static string GetUserRole(this HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}