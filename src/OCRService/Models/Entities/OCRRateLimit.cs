using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCRService.Models.Entities
{
    /// <summary>
    /// Entity mapping với bảng users.ocr_rate_limits
    /// Quản lý rate limiting cho OCR service
    /// </summary>
    [Table("ocr_rate_limits", Schema = "users")]
    public class OCRRateLimit
    {
        /// <summary>
        /// ID chính
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ID giáo viên
        /// </summary>
        [Column("teacher_id")]
        public Guid TeacherId { get; set; }

        /// <summary>
        /// Ngày request
        /// </summary>
        [Column("request_date")]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Số lượng request trong ngày
        /// </summary>
        [Column("request_count")]
        public int RequestCount { get; set; } = 0;

        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời điểm cập nhật
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // === NAVIGATION PROPERTIES ===

        /// <summary>
        /// Giáo viên
        /// </summary>
        [ForeignKey("TeacherId")]
        public virtual User? Teacher { get; set; }

        // === METHODS ===

        /// <summary>
        /// Tăng số lượng request
        /// </summary>
        public void IncrementRequest()
        {
            RequestCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Kiểm tra có vượt quá giới hạn không (10 ảnh/ngày)
        /// </summary>
        public bool IsLimitExceeded(int maxRequestsPerDay = 10)
        {
            return RequestCount >= maxRequestsPerDay;
        }

        /// <summary>
        /// Reset count cho ngày mới
        /// </summary>
        public void ResetForNewDay()
        {
            RequestDate = DateTime.Today;
            RequestCount = 0;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Kiểm tra có phải ngày mới không
        /// </summary>
        public bool IsNewDay()
        {
            return RequestDate.Date != DateTime.Today.Date;
        }
    }

    /// <summary>
    /// User entity (placeholder - sẽ được tạo sau)
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

