using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities
{
    [Table("user_profiles", Schema = "users")]
    public class HoSoNguoiDung
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; } 

        [StringLength(20)]
        [Column("phone")]
        public string? SoDienThoai { get; set; }

        // HoTen không map với cột nào vì full_name nằm trong bảng users.users
        [NotMapped]
        public string? HoTen { get; set; }

        [Column("address")]
        public string? DiaChi { get; set; }

        // NgaySinh không map với cột nào vì không có trong bảng user_profiles
        [NotMapped]
        public DateTime? NgaySinh { get; set; }

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