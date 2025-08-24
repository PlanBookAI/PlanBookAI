using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    [Table("question_choices", Schema = "assessment")]
    public class LuaChon
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("choice_text")]
        public string NoiDung { get; set; } = string.Empty;

        [Column("question_id")]
        public Guid CauHoiId { get; set; }

        [Column("is_correct")]
        public bool LaDapAnDung { get; set; }

        [Column("order_index")]
        public int ThuTu { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [ForeignKey("CauHoiId")]
        public virtual CauHoi CauHoi { get; set; } = null!;
    }
}
