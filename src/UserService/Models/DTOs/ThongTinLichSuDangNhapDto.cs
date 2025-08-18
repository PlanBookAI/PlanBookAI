namespace UserService.Models.DTOs;

public class ThongTinLichSuDangNhapDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; } // Required như database schema
    public DateTime NgayDangNhap { get; set; }
}
