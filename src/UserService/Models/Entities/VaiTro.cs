using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities;

[Table("roles", Schema = "auth")]
public class VaiTro
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(50)]
    public string Ten { get; set; } = string.Empty;

    [Column("description")]
    public string? MoTa { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    // Navigation properties
    public virtual ICollection<NguoiDung> NguoiDung { get; set; } = new List<NguoiDung>();

    // Sessions navigation property
    public virtual ICollection<LichSuDangNhap> LichSuDangNhap { get; set; } = new List<LichSuDangNhap>();

    // Computed properties for backward compatibility
    public bool HoatDong => IsActive;
    public DateTime TaoLuc => CreatedAt;
    public DateTime CapNhatLuc => UpdatedAt;
}
