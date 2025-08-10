using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskService.Models.Entities
{
    [Table("question_choices", Schema = "assessment")]
    public class LuaChon
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("question_id")]
        public Guid CauHoiId { get; set; }

        [Required]
        [Column("choice_order")]
        public char ThuTu { get; set; } // A, B, C, D

        [Required]
        [Column("content")]
        public string NoiDung { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        // Mối quan hệ N-1: Nhiều lựa chọn thuộc về một câu hỏi
        [ForeignKey("CauHoiId")]
        public virtual CauHoi CauHoi { get; set; } = null!;
    }
}