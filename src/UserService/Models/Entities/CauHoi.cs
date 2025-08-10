using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskService.Models.Enums;

namespace TaskService.Models.Entities
{
    [Table("questions", Schema = "assessment")]
    public class CauHoi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc.")]
        [Column("content")]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [Column("type")]
        [EnumDataType(typeof(LoaiCauHoi))]
        public string Loai { get; set; } = nameof(LoaiCauHoi.MULTIPLE_CHOICE);

        [Required]
        [Column("difficulty")]
        [EnumDataType(typeof(MucDoKho))]
        public string DoKho { get; set; } = nameof(MucDoKho.MEDIUM);

        [Required]
        [StringLength(50)]
        [Column("subject")]
        public string MonHoc { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("topic")]
        public string? ChuDe { get; set; }

        [StringLength(10)]
        [Column("correct_answer")]
        public string? DapAnDung { get; set; } // Chỉ áp dụng cho trắc nghiệm (A, B, C, D)

        [Column("explanation")]
        public string? LoiGiai { get; set; }

        [Required]
        [Column("created_by")]
        public Guid NguoiTaoId { get; set; } // ID của giáo viên tạo câu hỏi

        [StringLength(20)]
        [Column("status")]
        public string TrangThai { get; set; } = "ACTIVE";

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        // Mối quan hệ 1-N: Một câu hỏi có nhiều lựa chọn
        public virtual ICollection<LuaChon> CacLuaChon { get; set; } = new List<LuaChon>();

        // Mối quan hệ N-M: Một câu hỏi có thể thuộc về nhiều đề thi
        public virtual ICollection<CauHoiDeThi> CauHoiTrongDeThi { get; set; } = new List<CauHoiDeThi>();
    }
}