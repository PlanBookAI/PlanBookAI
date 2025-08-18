using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities;

[Table("sessions", Schema = "auth")]
public class LichSuDangNhap
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("token")]
    [StringLength(500)]
    public string Token { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; } // Required như database schema

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Không set Kind vì database dùng timestamp without time zone

    // Không dùng navigation property để tránh EF Core tự động join
    // [ForeignKey("UserId")]
    // public virtual NguoiDung? NguoiDung { get; set; }

    // Computed properties
    public DateTime NgayDangNhap => CreatedAt;
}
