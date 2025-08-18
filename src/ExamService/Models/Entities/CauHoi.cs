using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskService.Models.Entities
{
    [Table("questions", Schema = "assessment")]
    public class CauHoi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("question_text")]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [Column("subject")]
        public string MonHoc { get; set; } = string.Empty;

        [Column("question_type")]
        public string LoaiCauHoi { get; set; } = "multiple_choice";

        [Column("topic")]
        public string? ChuDe { get; set; }

        [Column("difficulty_level")]
        public string DoKho { get; set; } = "medium";

        [Column("points")]
        public decimal Diem { get; set; } = 1.0M;

        [Column("correct_answer")]
        public string DapAnDung { get; set; } = string.Empty;

        [Column("explanation")]
        public string? GiaiThich { get; set; }

        [Column("created_by")]
        public Guid NguoiTaoId { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        public virtual ICollection<LuaChon> LuaChons { get; set; }
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }

        public CauHoi()
        {
            LuaChons = new List<LuaChon>();
            ExamQuestions = new List<ExamQuestion>();
        }
    }
}
