using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    [Table("questions", Schema = "assessment")]
    public class CauHoi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("content")]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [Column("subject")]
        public string MonHoc { get; set; } = "HOA_HOC";

        [Required]
        [Column("type")]
        public string LoaiCauHoi { get; set; } = "MULTIPLE_CHOICE";

        [Column("topic")]
        public string? ChuDe { get; set; }

        [Column("difficulty")]
        public string DoKho { get; set; } = "MEDIUM";

        [Column("points")]
        public decimal Diem { get; set; } = 1.0M;

        [Column("correct_answer")]
        public string? DapAnDung { get; set; }

        [Column("explanation")]
        public string? GiaiThich { get; set; }

        [Required]
        [Column("created_by")]
        public Guid NguoiTaoId { get; set; }

        [Column("status")]
        public string TrangThai { get; set; } = "ACTIVE";

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
