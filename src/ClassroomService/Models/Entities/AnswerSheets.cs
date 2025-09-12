using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    public enum OcrStatus
    {
        PENDING,
        PROCESSING,
        COMPLETED,
        FAILED
    }

    [Table("answer_sheets")]
    public class AnswerSheets
    {
        [Key]
        public int Id { get; set; }
        
        [Column("student_id")]
        public int StudentId { get; set; }
        
        [Column("exam_id")]
        public int ExamId { get; set; }
        
        [Required, MaxLength(500)]
        [Column("image_url")]
        public string ImageUrl { get; set; }
        
        [Column("ocr_result", TypeName = "jsonb")]
        public string OcrResult { get; set; }
        
        [Column("ocr_status")]
        public OcrStatus OcrStatus { get; set; } = OcrStatus.PENDING;
        
        [Column("ocr_accuracy")]
        public decimal? OcrAccuracy { get; set; }
        
        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Thuộc tính điều hướng
        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }
    }
}