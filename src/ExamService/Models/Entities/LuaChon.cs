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
        [Column("content")]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [Column("question_id")]
        public Guid CauHoiId { get; set; }

        [Required]
        [Column("choice_order")]
        public string MaLuaChon { get; set; } = "A";
        
        [Column("display_order")]
        public int ThuTu { get; set; } = 0;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [ForeignKey("CauHoiId")]
        public virtual CauHoi CauHoi { get; set; } = null!;
    }
}
