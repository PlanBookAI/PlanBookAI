using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Entities
{
    [Table("roles", Schema = "users")]
    public class VaiTro
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Ten { get; set; } = string.Empty;

        [Column("description")]
        public string? MoTa { get; set; }

        [Column("is_active")]
        public bool LaHoatDong { get; set; } = true;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        public virtual ICollection<NguoiDung> DanhSachNguoiDung { get; set; } = new List<NguoiDung>();
    }
}
