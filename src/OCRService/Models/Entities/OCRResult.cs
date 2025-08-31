using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCRService.Models.Entities
{
    /// <summary>
    /// Entity mapping với bảng students.student_results
    /// Lưu kết quả chấm bài và thông tin OCR
    /// </summary>
    [Table("student_results", Schema = "students")]
    public class OCRResult
    {
        /// <summary>
        /// ID chính của kết quả học sinh
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ID học sinh
        /// </summary>
        [Column("student_id")]
        public Guid StudentId { get; set; }

        /// <summary>
        /// ID đề thi
        /// </summary>
        [Column("exam_id")]
        public Guid ExamId { get; set; }

        /// <summary>
        /// Điểm số (0.00 - 10.00)
        /// </summary>
        [Column("score")]
        [Column(TypeName = "numeric(5,2)")]
        public decimal? Score { get; set; }

        /// <summary>
        /// Thời gian làm bài thực tế (phút)
        /// </summary>
        [Column("actual_duration")]
        public int? ActualDuration { get; set; }

        /// <summary>
        /// Chi tiết đáp án (JSON)
        /// </summary>
        [Column("answer_details")]
        public string? AnswerDetails { get; set; }

        /// <summary>
        /// Phương thức chấm (OCR, MANUAL, AUTO)
        /// </summary>
        [Column("grading_method")]
        [StringLength(50)]
        public string GradingMethod { get; set; } = "OCR";

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Column("notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Ngày thi
        /// </summary>
        [Column("exam_date")]
        public DateTime? ExamDate { get; set; }

        /// <summary>
        /// Thời điểm chấm bài
        /// </summary>
        [Column("graded_at")]
        public DateTime GradedAt { get; set; } = DateTime.UtcNow;

        // === CÁC COLUMNS BỔ SUNG CHO OCR ===

        /// <summary>
        /// ID kết quả OCR (để liên kết với OCR processing)
        /// </summary>
        [Column("ocr_result_id")]
        public Guid? OcrResultId { get; set; }

        /// <summary>
        /// Độ tin cậy của OCR (0.00 - 1.00)
        /// </summary>
        [Column("ocr_confidence")]
        [Column(TypeName = "numeric(3,2)")]
        public decimal? OcrConfidence { get; set; }

        /// <summary>
        /// Ghi chú về fallback (nếu có)
        /// </summary>
        [Column("fallback_note")]
        public string? FallbackNote { get; set; }

        // === NAVIGATION PROPERTIES ===

        /// <summary>
        /// Học sinh
        /// </summary>
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        /// <summary>
        /// Đề thi
        /// </summary>
        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }

        // === METHODS ===

        /// <summary>
        /// Cập nhật điểm số
        /// </summary>
        public void UpdateScore(decimal score, string gradingMethod = "OCR")
        {
            Score = score;
            GradingMethod = gradingMethod;
            GradedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Cập nhật thông tin OCR
        /// </summary>
        public void UpdateOcrInfo(Guid ocrResultId, decimal confidence)
        {
            OcrResultId = ocrResultId;
            OcrConfidence = confidence;
            GradingMethod = "OCR";
        }

        /// <summary>
        /// Cập nhật fallback note
        /// </summary>
        public void UpdateFallbackNote(string note)
        {
            FallbackNote = note;
            GradingMethod = "MANUAL";
        }

        /// <summary>
        /// Kiểm tra có phải chấm thủ công không
        /// </summary>
        public bool IsManualGrading => GradingMethod == "MANUAL";

        /// <summary>
        /// Kiểm tra có phải chấm OCR không
        /// </summary>
        public bool IsOcrGrading => GradingMethod == "OCR";
    }
}
