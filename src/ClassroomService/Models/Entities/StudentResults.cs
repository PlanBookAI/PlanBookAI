using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    /// <summary>
    /// Grading method enumeration
    /// </summary>
    public enum GradingMethod
    {
        /// <summary>
        /// Optical Character Recognition grading
        /// </summary>
        OCR,
        /// <summary>
        /// Manual grading by teacher
        /// </summary>
        MANUAL,
        /// <summary>
        /// Automatic grading by system
        /// </summary>
        AUTO
    }

    /// <summary>
    /// Student result entity representing exam results
    /// </summary>
    [Table("student_results")]
    public class StudentResults
    {
        /// <summary>
        /// Result ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the student who took the exam
        /// </summary>
        [Column("student_id")]
        public int StudentId { get; set; }
        
        /// <summary>
        /// ID of the exam
        /// </summary>
        [Column("exam_id")]
        public int ExamId { get; set; }
        
        /// <summary>
        /// Score achieved by the student
        /// </summary>
        [Column("score")]
        public decimal Score { get; set; }
        
        /// <summary>
        /// Actual time taken to complete exam (in minutes)
        /// </summary>
        [Column("actual_duration")]
        public int ActualDuration { get; set; }
        
        /// <summary>
        /// Detailed answers in JSON format
        /// </summary>
        [Column("answer_details", TypeName = "jsonb")]
        public required string AnswerDetails { get; set; }
        
        /// <summary>
        /// Method used for grading
        /// </summary>
        [Column("grading_method")]
        public GradingMethod GradingMethod { get; set; }
        
        /// <summary>
        /// Additional notes about the result
        /// </summary>
        [MaxLength(500)]
        public required string Notes { get; set; }
        
        /// <summary>
        /// Date when the exam was taken
        /// </summary>
        [Column("exam_date")]
        public DateTime ExamDate { get; set; }
        
        /// <summary>
        /// Timestamp when the exam was graded
        /// </summary>
        [Column("graded_at")]
        public DateTime? GradedAt { get; set; }
        
        /// <summary>
        /// Navigation property to the student
        /// </summary>
        [ForeignKey("StudentId")]
        public virtual Students? Student { get; set; }
    }
}