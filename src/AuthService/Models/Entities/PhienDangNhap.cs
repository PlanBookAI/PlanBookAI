using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Entities;

[Table("sessions", Schema = "auth")]
public class PhienDangNhap
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid NguoiDungId { get; set; }

    [Required]
    [Column("token")]
    [StringLength(500)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [Column("expires_at")]
    public DateTime HetHanLuc { get; set; }

    [Column("created_at")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("NguoiDungId")]
    public virtual NguoiDung NguoiDung { get; set; } = null!;
}
