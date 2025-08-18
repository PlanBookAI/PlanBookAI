using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Entities;

[Table("users", Schema = "auth")]
public class NguoiDung
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("email")]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [StringLength(255)]
    public string MatKhauMaHoa { get; set; } = string.Empty;

    [Required]
    [Column("role_id")]
    public int VaiTroId { get; set; }

    [Column("is_active")]
    public bool HoatDong { get; set; } = true;

    [Column("last_login")]
    public DateTime? LanDangNhapCuoi { get; set; }

    [Column("created_at")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VaiTroId")]
    public virtual VaiTro VaiTro { get; set; } = null!;

    public virtual ICollection<PhienDangNhap> DanhSachPhienDangNhap { get; set; } = new List<PhienDangNhap>();
}
