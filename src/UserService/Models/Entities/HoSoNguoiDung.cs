using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities;

[Table("user_profiles", Schema = "users")]
public class HoSoNguoiDung
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("full_name")]
    [StringLength(255)]
    public string HoTen { get; set; } = string.Empty;

    [Column("phone")]
    [StringLength(20)]
    public string? SoDienThoai { get; set; }

    [Column("address")]
    public string? DiaChi { get; set; }

    [Column("bio")]
    public string? MoTaBanThan { get; set; }

    [Column("avatar_url")]
    [StringLength(500)]
    public string? AnhDaiDienUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    // Navigation property
    [ForeignKey("UserId")]
    public virtual NguoiDung? NguoiDung { get; set; }

    // Computed properties for backward compatibility
    public DateTime TaoLuc => CreatedAt;
    public DateTime CapNhatLuc => UpdatedAt;
}
