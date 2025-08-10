using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities
{

    [Table("profiles", Schema = "users")]
    public class HoSoNguoiDung
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; } 

        [Required]
        [StringLength(255)]
        [Column("full_name")]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("phone_number")]
        public string? SoDienThoai { get; set; }

        [Column("birth_date")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(500)]
        [Column("address")]
        public string? DiaChi { get; set; }

        [StringLength(500)]
        [Column("avatar_url")]
        public string? AnhDaiDienUrl { get; set; }

        [Column("bio")]
        public string? MoTaBanThan { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;
    }
}