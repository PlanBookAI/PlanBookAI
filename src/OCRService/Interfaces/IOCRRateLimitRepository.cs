using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OCRService.Models.Entities;

namespace OCRService.Interfaces
{
    /// <summary>
    /// Interface cho repository quản lý rate limiting OCR
    /// </summary>
    public interface IOCRRateLimitRepository
    {
        // === CRUD OPERATIONS ===

        /// <summary>
        /// Tạo rate limit record mới
        /// </summary>
        Task<OCRRateLimit> CreateAsync(OCRRateLimit rateLimit);

        /// <summary>
        /// Lấy rate limit theo ID
        /// </summary>
        Task<OCRRateLimit?> GetByIdAsync(Guid id);

        /// <summary>
        /// Cập nhật rate limit
        /// </summary>
        Task<bool> UpdateAsync(OCRRateLimit rateLimit);

        /// <summary>
        /// Xóa rate limit
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        // === QUERY METHODS ===

        /// <summary>
        /// Lấy tất cả rate limits
        /// </summary>
        Task<IEnumerable<OCRRateLimit>> GetAllAsync();

        /// <summary>
        /// Lấy rate limit theo giáo viên
        /// </summary>
        Task<OCRRateLimit?> GetByTeacherAsync(Guid teacherId);

        /// <summary>
        /// Lấy rate limit theo ngày
        /// </summary>
        Task<OCRRateLimit?> GetByTeacherAndDateAsync(Guid teacherId, DateTime date);

        /// <summary>
        /// Lấy rate limits theo ngày
        /// </summary>
        Task<IEnumerable<OCRRateLimit>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy rate limits theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<OCRRateLimit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // === RATE LIMITING METHODS ===

        /// <summary>
        /// Kiểm tra giáo viên có vượt quá giới hạn không
        /// </summary>
        Task<bool> IsLimitExceededAsync(Guid teacherId, int maxRequestsPerDay = 10);

        /// <summary>
        /// Tăng số lượng request cho giáo viên
        /// </summary>
        Task<bool> IncrementRequestCountAsync(Guid teacherId);

        /// <summary>
        /// Lấy số lượng request còn lại của giáo viên
        /// </summary>
        Task<int> GetRemainingRequestsAsync(Guid teacherId, int maxRequestsPerDay = 10);

        /// <summary>
        /// Reset rate limit cho giáo viên
        /// </summary>
        Task<bool> ResetRateLimitAsync(Guid teacherId);

        // === BATCH OPERATIONS ===

        /// <summary>
        /// Cập nhật nhiều rate limits cùng lúc
        /// </summary>
        Task<bool> UpdateMultipleAsync(IEnumerable<OCRRateLimit> rateLimits);

        /// <summary>
        /// Xóa nhiều rate limits cùng lúc
        /// </summary>
        Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Reset tất cả rate limits cho ngày mới
        /// </summary>
        Task<int> ResetAllRateLimitsForNewDayAsync();

        // === PERFORMANCE METHODS ===

        /// <summary>
        /// Lấy rate limits với phân trang
        /// </summary>
        Task<(IEnumerable<OCRRateLimit> Items, int TotalCount)> GetWithPaginationAsync(
            int pageNumber, 
            int pageSize, 
            Guid? teacherId = null,
            DateTime? date = null);

        /// <summary>
        /// Lấy rate limit với include navigation properties
        /// </summary>
        Task<OCRRateLimit?> GetByIdWithIncludeAsync(Guid id);

        /// <summary>
        /// Lấy rate limit theo giáo viên với include
        /// </summary>
        Task<OCRRateLimit?> GetByTeacherWithIncludeAsync(Guid teacherId);

        // === STATISTICS METHODS ===

        /// <summary>
        /// Đếm tổng số rate limits
        /// </summary>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Đếm rate limits theo giáo viên
        /// </summary>
        Task<int> GetCountByTeacherAsync(Guid teacherId);

        /// <summary>
        /// Đếm rate limits theo ngày
        /// </summary>
        Task<int> GetCountByDateAsync(DateTime date);

        /// <summary>
        /// Đếm giáo viên đã vượt quá giới hạn
        /// </summary>
        Task<int> GetCountExceededLimitAsync(int maxRequestsPerDay = 10);

        // === MONITORING METHODS ===

        /// <summary>
        /// Lấy danh sách giáo viên đã vượt quá giới hạn
        /// </summary>
        Task<IEnumerable<OCRRateLimit>> GetExceededLimitsAsync(int maxRequestsPerDay = 10);

        /// <summary>
        /// Lấy danh sách giáo viên sắp vượt quá giới hạn
        /// </summary>
        Task<IEnumerable<OCRRateLimit>> GetNearLimitAsync(int threshold, int maxRequestsPerDay = 10);

        /// <summary>
        /// Lấy thống kê rate limiting theo ngày
        /// </summary>
        Task<object> GetDailyRateLimitStatisticsAsync(DateTime date);

        /// <summary>
        /// Lấy thống kê rate limiting theo tuần
        /// </summary>
        Task<object> GetWeeklyRateLimitStatisticsAsync(DateTime startOfWeek);

        /// <summary>
        /// Lấy thống kê rate limiting theo tháng
        /// </summary>
        Task<object> GetMonthlyRateLimitStatisticsAsync(DateTime startOfMonth);

        // === CLEANUP METHODS ===

        /// <summary>
        /// Xóa rate limits cũ (theo thời gian)
        /// </summary>
        Task<int> CleanupOldRateLimitsAsync(DateTime cutoffDate);

        /// <summary>
        /// Xóa rate limits của giáo viên không hoạt động
        /// </summary>
        Task<int> CleanupInactiveTeacherRateLimitsAsync(DateTime cutoffDate);

        // === ADMIN METHODS ===

        /// <summary>
        /// Cập nhật giới hạn request cho giáo viên
        /// </summary>
        Task<bool> UpdateTeacherLimitAsync(Guid teacherId, int newMaxRequestsPerDay);

        /// <summary>
        /// Tạm thời tăng giới hạn cho giáo viên
        /// </summary>
        Task<bool> TemporarilyIncreaseLimitAsync(Guid teacherId, int additionalRequests, TimeSpan duration);

        /// <summary>
        /// Khóa rate limiting cho giáo viên
        /// </summary>
        Task<bool> DisableRateLimitAsync(Guid teacherId);

        /// <summary>
        /// Mở khóa rate limiting cho giáo viên
        /// </summary>
        Task<bool> EnableRateLimitAsync(Guid teacherId);
    }
}
