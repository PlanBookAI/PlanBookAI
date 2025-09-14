using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    /// <summary>
    /// OCR processing status enumeration
    /// </summary>
    public enum OcrStatus
    {
        /// <summary>
        /// OCR processing is pending
        /// </summary>
        PENDING,
        /// <summary>
        /// OCR processing is in progress
        /// </summary>
        PROCESSING,
        /// <summary>
        /// OCR processing completed successfully
        /// </summary>
        COMPLETED,
        /// <summary>
        /// OCR processing failed
        /// </summary>
        FAILED
    }

    /// <summary>
    /// Answer sheet entity representing scanned answer sheets
    /// </summary>
    [Table("answer_sheets")]
    public class AnswerSheets
    {
        /// <summary>
        /// Answer sheet ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the student who submitted this answer sheet
        /// </summary>
        [Column("student_id")]
        public int StudentId { get; set; }
        
        /// <summary>
        /// ID of the exam this answer sheet belongs to
        /// </summary>
        [Column("exam_id")]
        public int ExamId { get; set; }
        
        /// <summary>
        /// URL to the scanned image of the answer sheet
        /// </summary>
        [Required, MaxLength(500)]
        [Column("image_url")]
        public required string ImageUrl { get; set; }
        
        /// <summary>
        /// OCR processing result in JSON format
        /// </summary>
        [Column("ocr_result", TypeName = "jsonb")]
        public required string OcrResult { get; set; }
        
        /// <summary>
        /// Current status of OCR processing
        /// </summary>
        [Column("ocr_status")]
        public OcrStatus OcrStatus { get; set; } = OcrStatus.PENDING;
        
        /// <summary>
        /// Accuracy percentage of OCR processing
        /// </summary>
        [Column("ocr_accuracy")]
        public decimal? OcrAccuracy { get; set; }
        
        /// <summary>
        /// Timestamp when OCR processing was completed
        /// </summary>
        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }
        
        /// <summary>
        /// Creation timestamp
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Navigation property to the student
        /// </summary>
        [ForeignKey("StudentId")]
        public virtual Students? Student { get; set; }
    }
}