using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities
{
    [Table("activity_logs", Schema = "users")]
    public class LichSuHoatDong
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("action")]
        public string HanhDong { get; set; } = string.Empty; // Ví dụ: "CAP_NHAT_HO_SO"

        [Column("details")]
        public string? MoTaChiTiet { get; set; }

        [Required]
        [Column("timestamp")]
        public DateTime ThoiGian { get; set; } = DateTime.UtcNow;

        [StringLength(45)]
        [Column("ip_address")]
        public string? DiaChiIP { get; set; }
    }
}
