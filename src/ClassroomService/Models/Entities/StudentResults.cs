using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ClassroomService.Models.Entities
{
    public enum GradingMethod
    {
        OCR,
        MANUAL,
        AUTO
    }

    [Table("student_results")]
    public class StudentResults
    {
        [Key]
        public int Id { get; set; }
        
        [Column("student_id")]
        public int StudentId { get; set; }
        
        [Column("exam_id")]
        public int ExamId { get; set; }
        
        [Column("score")]
        public decimal Score { get; set; }
        
        [Column("actual_duration")]
        public int ActualDuration { get; set; }
        
        [Column("answer_details", TypeName = "jsonb")]
        public string AnswerDetails { get; set; }
        
        [Column("grading_method")]
        public GradingMethod GradingMethod { get; set; }
        
        [MaxLength(500)]
        public string Notes { get; set; }
        
        [Column("exam_date")]
        public DateTime ExamDate { get; set; }
        
        [Column("graded_at")]
        public DateTime? GradedAt { get; set; }
        
        // Thuộc tính điều hướng
        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }
    }
}