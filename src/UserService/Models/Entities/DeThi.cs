using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskService.Models.Entities
{
    [Table("exams", Schema = "assessment")]
    public class DeThi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string TieuDe { get; set; } = string.Empty;

        [Column("description")]
        public string? MoTa { get; set; }

        // ... các thuộc tính khác như MonHoc, Lop, ThoiGianLamBai ...

        // Mối quan hệ N-M: Một đề thi có nhiều câu hỏi
        public virtual ICollection<CauHoiDeThi> CauHoiTrongDeThi { get; set; } = new List<CauHoiDeThi>();
    }

    // Bảng trung gian cho mối quan hệ N-M giữa Đề Thi và Câu Hỏi
    [Table("exam_questions", Schema = "assessment")]
    public class CauHoiDeThi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("exam_id")]
        public Guid DeThiId { get; set; }

        [Required]
        [Column("question_id")]
        public Guid CauHoiId { get; set; }

        [Column("question_order")]
        public int ThuTuCauHoi { get; set; }

        [Column("points", TypeName = "decimal(4, 2)")]
        public decimal Diem { get; set; }

        [ForeignKey("DeThiId")]
        public virtual DeThi DeThi { get; set; } = null!;

        [ForeignKey("CauHoiId")]
        public virtual CauHoi CauHoi { get; set; } = null!;
    }
}