using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Entities
{
    [Table("users", Schema = "users")]
    public class NguoiDung
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("full_name")]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("password_hash")]
        public string MatKhauMaHoa { get; set; } = string.Empty;

        [Column("role_id")]
        public int VaiTroId { get; set; }

        [Column("is_active")]
        public bool LaHoatDong { get; set; } = true;

        [Column("last_login")]
        public DateTime? LanDangNhapCuoi { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        [ForeignKey("VaiTroId")]
        public virtual VaiTro VaiTro { get; set; } = null!;
    }
}
