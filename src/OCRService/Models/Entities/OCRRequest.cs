using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCRService.Models.Entities
{
    /// <summary>
    /// Entity mapping với bảng students.answer_sheets
    /// Lưu thông tin yêu cầu OCR và trạng thái xử lý
    /// </summary>
    [Table("answer_sheets", Schema = "students")]
    public class OCRRequest
    {
        /// <summary>
        /// ID chính của bài làm (answer sheet)
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ID học sinh làm bài
        /// </summary>
        [Column("student_id")]
        public Guid StudentId { get; set; }

        /// <summary>
        /// ID đề thi
        /// </summary>
        [Column("exam_id")]
        public Guid ExamId { get; set; }

        /// <summary>
        /// URL ảnh bài làm
        /// </summary>
        [Column("image_url")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Kết quả OCR (JSON)
        /// </summary>
        [Column("ocr_result")]
        public string? OcrResult { get; set; }

        /// <summary>
        /// Trạng thái OCR (PENDING, PROCESSING, COMPLETED, FAILED)
        /// </summary>
        [Column("ocr_status")]
        [StringLength(50)]
        public string OcrStatus { get; set; } = "PENDING";

        /// <summary>
        /// Độ chính xác OCR (0.00 - 1.00)
        /// </summary>
        [Column("ocr_accuracy")]
        [Column(TypeName = "numeric(5,2)")]
        public decimal? OcrAccuracy { get; set; }

        /// <summary>
        /// Thời điểm xử lý OCR
        /// </summary>
        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // === CÁC COLUMNS BỔ SUNG CHO OCR ===

        /// <summary>
        /// ID yêu cầu OCR (để track)
        /// </summary>
        [Column("ocr_request_id")]
        public Guid? OcrRequestId { get; set; }

        /// <summary>
        /// Thời gian xử lý OCR (milliseconds)
        /// </summary>
        [Column("ocr_processing_time")]
        public int? OcrProcessingTime { get; set; }

        /// <summary>
        /// AI provider sử dụng (google_vision, ollama)
        /// </summary>
        [Column("ocr_provider")]
        [StringLength(50)]
        public string? OcrProvider { get; set; }

        /// <summary>
        /// Thời điểm bắt đầu xử lý OCR
        /// </summary>
        [Column("ocr_started_at")]
        public DateTime? OcrStartedAt { get; set; }

        /// <summary>
        /// Thời điểm hoàn thành OCR
        /// </summary>
        [Column("ocr_completed_at")]
        public DateTime? OcrCompletedAt { get; set; }

        /// <summary>
        /// Lỗi OCR (nếu có)
        /// </summary>
        [Column("ocr_error")]
        public string? OcrError { get; set; }

        /// <summary>
        /// Số lần retry
        /// </summary>
        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Giới hạn số lần retry
        /// </summary>
        [Column("max_retries")]
        public int MaxRetries { get; set; } = 3;

        // === CÁC COLUMNS CHO FALLBACK MECHANISM ===

        /// <summary>
        /// Trạng thái fallback (MANUAL_GRADING_REQUIRED, MANUAL_GRADING_COMPLETED)
        /// </summary>
        [Column("fallback_status")]
        [StringLength(50)]
        public string? FallbackStatus { get; set; }

        /// <summary>
        /// Lý do fallback
        /// </summary>
        [Column("fallback_reason")]
        public string? FallbackReason { get; set; }

        /// <summary>
        /// Cần chấm thủ công hay không
        /// </summary>
        [Column("manual_grading_required")]
        public bool ManualGradingRequired { get; set; } = false;

        // === NAVIGATION PROPERTIES ===

        /// <summary>
        /// Học sinh làm bài
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
        /// Kiểm tra có thể retry không
        /// </summary>
        public bool CanRetry => RetryCount < MaxRetries;

        /// <summary>
        /// Tăng số lần retry
        /// </summary>
        public void IncrementRetry()
        {
            RetryCount++;
        }

        /// <summary>
        /// Cập nhật trạng thái OCR
        /// </summary>
        public void UpdateOcrStatus(string status, string? error = null)
        {
            OcrStatus = status;
            OcrError = error;
            
            switch (status)
            {
                case "PROCESSING":
                    OcrStartedAt = DateTime.UtcNow;
                    break;
                case "COMPLETED":
                    OcrCompletedAt = DateTime.UtcNow;
                    ProcessedAt = DateTime.UtcNow;
                    break;
                case "FAILED":
                    OcrCompletedAt = DateTime.UtcNow;
                    break;
            }
        }

        /// <summary>
        /// Cập nhật fallback status
        /// </summary>
        public void UpdateFallbackStatus(string status, string reason)
        {
            FallbackStatus = status;
            FallbackReason = reason;
            ManualGradingRequired = status == "MANUAL_GRADING_REQUIRED";
        }
    }

    /// <summary>
    /// Student entity (placeholder - sẽ được tạo sau)
    /// </summary>
    public class Student
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Exam entity (placeholder - sẽ được tạo sau)
    /// </summary>
    public class Exam
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
