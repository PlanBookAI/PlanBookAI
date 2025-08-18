using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities;

[Table("users", Schema = "auth")]
public class NguoiDung
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    [ForeignKey("RoleId")]
    public virtual VaiTro? VaiTro { get; set; }

    // Profile navigation property
    public virtual HoSoNguoiDung? HoSoNguoiDung { get; set; }

    // Không dùng navigation property LichSuDangNhap để tránh EF Core tự động join
    // public virtual ICollection<LichSuDangNhap> LichSuDangNhap { get; set; } = new List<LichSuDangNhap>();

    // Computed properties for backward compatibility
    public bool HoatDong => IsActive;
    public DateTime TaoLuc => CreatedAt;
    public DateTime CapNhatLuc => UpdatedAt;
}
