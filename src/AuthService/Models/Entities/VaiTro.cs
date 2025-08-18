using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Entities;

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
    public bool HoatDong { get; set; } = true;

    [Column("created_at")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<NguoiDung> DanhSachNguoiDung { get; set; } = new List<NguoiDung>();
}
