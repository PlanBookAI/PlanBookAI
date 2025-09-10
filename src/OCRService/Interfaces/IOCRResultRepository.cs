using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OCRService.Models.Entities;

namespace OCRService.Interfaces
{
    /// <summary>
    /// Interface cho repository xử lý kết quả OCR
    /// </summary>
    public interface IOCRResultRepository
    {
        // === CRUD OPERATIONS ===

        /// <summary>
        /// Tạo kết quả OCR mới
        /// </summary>
        Task<OCRResult> CreateAsync(OCRResult ocrResult);

        /// <summary>
        /// Lấy kết quả OCR theo ID
        /// </summary>
        Task<OCRResult?> GetByIdAsync(Guid id);

        /// <summary>
        /// Cập nhật kết quả OCR
        /// </summary>
        Task<bool> UpdateAsync(OCRResult ocrResult);

        /// <summary>
        /// Xóa kết quả OCR
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        // === QUERY METHODS ===

        /// <summary>
        /// Lấy tất cả kết quả OCR
        /// </summary>
        Task<IEnumerable<OCRResult>> GetAllAsync();

        /// <summary>
        /// Lấy kết quả OCR theo học sinh
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByStudentAsync(Guid studentId);

        /// <summary>
        /// Lấy kết quả OCR theo đề thi
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByExamAsync(Guid examId);

        /// <summary>
        /// Lấy kết quả OCR theo phương thức chấm
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByGradingMethodAsync(string gradingMethod);

        /// <summary>
        /// Lấy kết quả OCR theo thời gian chấm
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByGradingDateRangeAsync(DateTime startDate, DateTime endDate);

        // === SPECIAL QUERY METHODS ===

        /// <summary>
        /// Lấy kết quả OCR đã chấm
        /// </summary>
        Task<IEnumerable<OCRResult>> GetGradedResultsAsync();

        /// <summary>
        /// Lấy kết quả OCR chưa chấm
        /// </summary>
        Task<IEnumerable<OCRResult>> GetUngradedResultsAsync();

        /// <summary>
        /// Lấy kết quả OCR chấm thủ công
        /// </summary>
        Task<IEnumerable<OCRResult>> GetManualGradedResultsAsync();

        /// <summary>
        /// Lấy kết quả OCR chấm tự động
        /// </summary>
        Task<IEnumerable<OCRResult>> GetAutoGradedResultsAsync();

        // === SCORE & GRADING METHODS ===

        /// <summary>
        /// Lấy kết quả OCR theo điểm số
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByScoreRangeAsync(decimal minScore, decimal maxScore);

        /// <summary>
        /// Lấy kết quả OCR có điểm cao nhất
        /// </summary>
        Task<IEnumerable<OCRResult>> GetTopScoringResultsAsync(int topCount);

        /// <summary>
        /// Lấy kết quả OCR có điểm thấp nhất
        /// </summary>
        Task<IEnumerable<OCRResult>> GetLowestScoringResultsAsync(int bottomCount);

        /// <summary>
        /// Lấy kết quả OCR theo độ tin cậy OCR
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByOcrConfidenceAsync(decimal minConfidence);

        // === BATCH OPERATIONS ===

        /// <summary>
        /// Cập nhật nhiều kết quả OCR cùng lúc
        /// </summary>
        Task<bool> UpdateMultipleAsync(IEnumerable<OCRResult> ocrResults);

        /// <summary>
        /// Xóa nhiều kết quả OCR cùng lúc
        /// </summary>
        Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Cập nhật điểm số cho nhiều kết quả
        /// </summary>
        Task<bool> UpdateScoresAsync(IEnumerable<(Guid Id, decimal Score)> scoreUpdates);

        // === PERFORMANCE METHODS ===

        /// <summary>
        /// Lấy kết quả OCR với phân trang
        /// </summary>
        Task<(IEnumerable<OCRResult> Items, int TotalCount)> GetWithPaginationAsync(
            int pageNumber, 
            int pageSize, 
            Guid? studentId = null,
            Guid? examId = null,
            string? gradingMethod = null);

        /// <summary>
        /// Lấy kết quả OCR với include navigation properties
        /// </summary>
        Task<OCRResult?> GetByIdWithIncludeAsync(Guid id);

        /// <summary>
        /// Lấy kết quả OCR theo học sinh với include
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByStudentWithIncludeAsync(Guid studentId);

        /// <summary>
        /// Lấy kết quả OCR theo đề thi với include
        /// </summary>
        Task<IEnumerable<OCRResult>> GetByExamWithIncludeAsync(Guid examId);

        // === STATISTICS METHODS ===

        /// <summary>
        /// Đếm tổng số kết quả OCR
        /// </summary>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Đếm kết quả OCR theo học sinh
        /// </summary>
        Task<int> GetCountByStudentAsync(Guid studentId);

        /// <summary>
        /// Đếm kết quả OCR theo đề thi
        /// </summary>
        Task<int> GetCountByExamAsync(Guid examId);

        /// <summary>
        /// Đếm kết quả OCR theo phương thức chấm
        /// </summary>
        Task<int> GetCountByGradingMethodAsync(string gradingMethod);

        /// <summary>
        /// Đếm kết quả OCR theo ngày
        /// </summary>
        Task<int> GetCountByDateAsync(DateTime date);

        // === ANALYTICS METHODS ===

        /// <summary>
        /// Tính điểm trung bình theo đề thi
        /// </summary>
        Task<decimal> GetAverageScoreByExamAsync(Guid examId);

        /// <summary>
        /// Tính điểm trung bình theo học sinh
        /// </summary>
        Task<decimal> GetAverageScoreByStudentAsync(Guid studentId);

        /// <summary>
        /// Tính điểm trung bình theo lớp
        /// </summary>
        Task<decimal> GetAverageScoreByClassAsync(string className);

        /// <summary>
        /// Lấy thống kê điểm theo đề thi
        /// </summary>
        Task<object> GetScoreStatisticsByExamAsync(Guid examId);

        // === CLEANUP METHODS ===

        /// <summary>
        /// Xóa kết quả OCR cũ (theo thời gian)
        /// </summary>
        Task<int> CleanupOldResultsAsync(DateTime cutoffDate);

        /// <summary>
        /// Xóa kết quả OCR có điểm thấp
        /// </summary>
        Task<int> CleanupLowScoreResultsAsync(decimal minScore);
    }
}
