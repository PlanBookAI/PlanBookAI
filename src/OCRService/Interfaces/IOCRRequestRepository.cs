using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OCRService.Models.Entities;

namespace OCRService.Interfaces
{
    /// <summary>
    /// Interface cho repository xử lý OCR requests
    /// </summary>
    public interface IOCRRequestRepository
    {
        // === CRUD OPERATIONS ===

        /// <summary>
        /// Tạo yêu cầu OCR mới
        /// </summary>
        Task<OCRRequest> CreateAsync(OCRRequest ocrRequest);

        /// <summary>
        /// Lấy yêu cầu OCR theo ID
        /// </summary>
        Task<OCRRequest?> GetByIdAsync(Guid id);

        /// <summary>
        /// Cập nhật yêu cầu OCR
        /// </summary>
        Task<bool> UpdateAsync(OCRRequest ocrRequest);

        /// <summary>
        /// Xóa yêu cầu OCR
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        // === QUERY METHODS ===

        /// <summary>
        /// Lấy tất cả yêu cầu OCR
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetAllAsync();

        /// <summary>
        /// Lấy yêu cầu OCR theo trạng thái
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetByStatusAsync(string status);

        /// <summary>
        /// Lấy yêu cầu OCR theo giáo viên
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetByTeacherAsync(Guid teacherId);

        /// <summary>
        /// Lấy yêu cầu OCR theo học sinh
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetByStudentAsync(Guid studentId);

        /// <summary>
        /// Lấy yêu cầu OCR theo đề thi
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetByExamAsync(Guid examId);

        /// <summary>
        /// Lấy yêu cầu OCR theo thời gian tạo
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // === SPECIAL QUERY METHODS ===

        /// <summary>
        /// Lấy danh sách yêu cầu OCR đang chờ xử lý
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetPendingRequestsAsync();

        /// <summary>
        /// Lấy danh sách yêu cầu OCR đang xử lý
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetProcessingRequestsAsync();

        /// <summary>
        /// Lấy danh sách yêu cầu OCR đã hoàn thành
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetCompletedRequestsAsync();

        /// <summary>
        /// Lấy danh sách yêu cầu OCR thất bại
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetFailedRequestsAsync();

        /// <summary>
        /// Lấy danh sách yêu cầu OCR cần fallback
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetFallbackRequestsAsync();

        // === RETRY & FALLBACK METHODS ===

        /// <summary>
        /// Lấy yêu cầu OCR có thể retry
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetRetryableRequestsAsync();

        /// <summary>
        /// Lấy yêu cầu OCR cần chấm thủ công
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetManualGradingRequestsAsync();

        // === BATCH OPERATIONS ===

        /// <summary>
        /// Cập nhật nhiều yêu cầu OCR cùng lúc
        /// </summary>
        Task<bool> UpdateMultipleAsync(IEnumerable<OCRRequest> ocrRequests);

        /// <summary>
        /// Xóa nhiều yêu cầu OCR cùng lúc
        /// </summary>
        Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids);

        // === PERFORMANCE METHODS ===

        /// <summary>
        /// Lấy yêu cầu OCR với phân trang
        /// </summary>
        Task<(IEnumerable<OCRRequest> Items, int TotalCount)> GetWithPaginationAsync(
            int pageNumber, 
            int pageSize, 
            string? status = null,
            Guid? teacherId = null);

        /// <summary>
        /// Lấy yêu cầu OCR với include navigation properties
        /// </summary>
        Task<OCRRequest?> GetByIdWithIncludeAsync(Guid id);

        /// <summary>
        /// Lấy yêu cầu OCR theo trạng thái với include
        /// </summary>
        Task<IEnumerable<OCRRequest>> GetByStatusWithIncludeAsync(string status);

        // === STATISTICS METHODS ===

        /// <summary>
        /// Đếm tổng số yêu cầu OCR
        /// </summary>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Đếm yêu cầu OCR theo trạng thái
        /// </summary>
        Task<int> GetCountByStatusAsync(string status);

        /// <summary>
        /// Đếm yêu cầu OCR theo giáo viên
        /// </summary>
        Task<int> GetCountByTeacherAsync(Guid teacherId);

        /// <summary>
        /// Đếm yêu cầu OCR theo ngày
        /// </summary>
        Task<int> GetCountByDateAsync(DateTime date);

        // === CLEANUP METHODS ===

        /// <summary>
        /// Xóa yêu cầu OCR cũ (theo thời gian)
        /// </summary>
        Task<int> CleanupOldRequestsAsync(DateTime cutoffDate);

        /// <summary>
        /// Xóa yêu cầu OCR thất bại cũ
        /// </summary>
        Task<int> CleanupOldFailedRequestsAsync(DateTime cutoffDate);
    }
}
