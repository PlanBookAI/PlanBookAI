using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OCRService.Data;
using OCRService.Interfaces;
using OCRService.Models.Entities;

namespace OCRService.Repositories
{
    /// <summary>
    /// Implementation của IOCRResultRepository
    /// </summary>
    public class OCRResultRepository : IOCRResultRepository
    {
        private readonly OCRDbContext _context;
        private readonly ILogger<OCRResultRepository> _logger;

        public OCRResultRepository(OCRDbContext context, ILogger<OCRResultRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // === CRUD OPERATIONS ===

        public async Task<OCRResult> CreateAsync(OCRResult ocrResult)
        {
            try
            {
                _logger.LogInformation("Tạo kết quả OCR mới với ID: {Id}", ocrResult.Id);
                
                ocrResult.Id = Guid.NewGuid();
                ocrResult.GradedAt = DateTime.UtcNow;
                
                var result = await _context.OCRResults.AddAsync(ocrResult);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã tạo kết quả OCR thành công với ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo kết quả OCR");
                throw;
            }
        }

        public async Task<OCRResult?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo ID: {Id}", id);
                
                return await _context.OCRResults
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OCRResult ocrResult)
        {
            try
            {
                _logger.LogInformation("Cập nhật kết quả OCR với ID: {Id}", ocrResult.Id);
                
                _context.OCRResults.Update(ocrResult);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật kết quả OCR thành công với ID: {Id}", ocrResult.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật kết quả OCR với ID: {Id}", ocrResult.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Xóa kết quả OCR với ID: {Id}", id);
                
                var ocrResult = await GetByIdAsync(id);
                if (ocrResult == null)
                {
                    _logger.LogWarning("Không tìm thấy kết quả OCR để xóa với ID: {Id}", id);
                    return false;
                }
                
                _context.OCRResults.Remove(ocrResult);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã xóa kết quả OCR thành công với ID: {Id}", id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa kết quả OCR với ID: {Id}", id);
                throw;
            }
        }

        // === QUERY METHODS ===

        public async Task<IEnumerable<OCRResult>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Lấy tất cả kết quả OCR");
                
                return await _context.OCRResults
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả kết quả OCR");
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByStudentAsync(Guid studentId)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo học sinh: {StudentId}", studentId);
                
                return await _context.OCRResults
                    .Where(x => x.StudentId == studentId)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo học sinh: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByExamAsync(Guid examId)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo đề thi: {ExamId}", examId);
                
                return await _context.OCRResults
                    .Where(x => x.ExamId == examId)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo đề thi: {ExamId}", examId);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByGradingMethodAsync(string gradingMethod)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo phương thức chấm: {GradingMethod}", gradingMethod);
                
                return await _context.OCRResults
                    .Where(x => x.GradingMethod == gradingMethod)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo phương thức chấm: {GradingMethod}", gradingMethod);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByGradingDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo khoảng thời gian chấm: {StartDate} - {EndDate}", startDate, endDate);
                
                return await _context.OCRResults
                    .Where(x => x.GradedAt >= startDate && x.GradedAt <= endDate)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo khoảng thời gian chấm: {StartDate} - {EndDate}", startDate, endDate);
                throw;
            }
        }

        // === SPECIAL QUERY METHODS ===

        public async Task<IEnumerable<OCRResult>> GetGradedResultsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR đã chấm");
                
                return await _context.OCRResults
                    .Where(x => x.Score.HasValue)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR đã chấm");
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetUngradedResultsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR chưa chấm");
                
                return await _context.OCRResults
                    .Where(x => !x.Score.HasValue)
                    .OrderBy(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR chưa chấm");
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetManualGradedResultsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR chấm thủ công");
                
                return await _context.OCRResults
                    .Where(x => x.GradingMethod == "MANUAL")
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR chấm thủ công");
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetAutoGradedResultsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR chấm tự động");
                
                return await _context.OCRResults
                    .Where(x => x.GradingMethod == "OCR" || x.GradingMethod == "AUTO")
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR chấm tự động");
                throw;
            }
        }

        // === SCORE & GRADING METHODS ===

        public async Task<IEnumerable<OCRResult>> GetByScoreRangeAsync(decimal minScore, decimal maxScore)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo khoảng điểm: {MinScore} - {MaxScore}", minScore, maxScore);
                
                return await _context.OCRResults
                    .Where(x => x.Score >= minScore && x.Score <= maxScore)
                    .OrderByDescending(x => x.Score)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo khoảng điểm: {MinScore} - {MaxScore}", minScore, maxScore);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetTopScoringResultsAsync(int topCount)
        {
            try
            {
                _logger.LogDebug("Lấy {Count} kết quả OCR có điểm cao nhất", topCount);
                
                return await _context.OCRResults
                    .Where(x => x.Score.HasValue)
                    .OrderByDescending(x => x.Score)
                    .Take(topCount)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy {Count} kết quả OCR có điểm cao nhất", topCount);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetLowestScoringResultsAsync(int bottomCount)
        {
            try
            {
                _logger.LogDebug("Lấy {Count} kết quả OCR có điểm thấp nhất", bottomCount);
                
                return await _context.OCRResults
                    .Where(x => x.Score.HasValue)
                    .OrderBy(x => x.Score)
                    .Take(bottomCount)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy {Count} kết quả OCR có điểm thấp nhất", bottomCount);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByOcrConfidenceAsync(decimal minConfidence)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR theo độ tin cậy tối thiểu: {MinConfidence}", minConfidence);
                
                return await _context.OCRResults
                    .Where(x => x.OcrConfidence >= minConfidence)
                    .OrderByDescending(x => x.OcrConfidence)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR theo độ tin cậy tối thiểu: {MinConfidence}", minConfidence);
                throw;
            }
        }

        // === BATCH OPERATIONS ===

        public async Task<bool> UpdateMultipleAsync(IEnumerable<OCRResult> ocrResults)
        {
            try
            {
                _logger.LogInformation("Cập nhật {Count} kết quả OCR", ocrResults.Count());
                
                _context.OCRResults.UpdateRange(ocrResults);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật {Count} kết quả OCR thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhiều kết quả OCR");
                throw;
            }
        }

        public async Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            try
            {
                _logger.LogInformation("Xóa {Count} kết quả OCR", ids.Count());
                
                var ocrResults = await _context.OCRResults
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();
                
                if (!ocrResults.Any())
                {
                    _logger.LogWarning("Không tìm thấy kết quả OCR nào để xóa");
                    return false;
                }
                
                _context.OCRResults.RemoveRange(ocrResults);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã xóa {Count} kết quả OCR thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhiều kết quả OCR");
                throw;
            }
        }

        public async Task<bool> UpdateScoresAsync(IEnumerable<(Guid Id, decimal Score)> scoreUpdates)
        {
            try
            {
                _logger.LogInformation("Cập nhật điểm số cho {Count} kết quả OCR", scoreUpdates.Count());
                
                var ids = scoreUpdates.Select(x => x.Id).ToList();
                var ocrResults = await _context.OCRResults
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();
                
                foreach (var (id, score) in scoreUpdates)
                {
                    var ocrResult = ocrResults.FirstOrDefault(x => x.Id == id);
                    if (ocrResult != null)
                    {
                        ocrResult.UpdateScore(score);
                    }
                }
                
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật điểm số cho {Count} kết quả OCR thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật điểm số cho nhiều kết quả OCR");
                throw;
            }
        }

        // === PERFORMANCE METHODS ===

        public async Task<(IEnumerable<OCRResult> Items, int TotalCount)> GetWithPaginationAsync(
            int pageNumber, int pageSize, Guid? studentId = null, Guid? examId = null, string? gradingMethod = null)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR với phân trang: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                
                var query = _context.OCRResults.AsQueryable();
                
                if (studentId.HasValue)
                {
                    query = query.Where(x => x.StudentId == studentId.Value);
                }
                
                if (examId.HasValue)
                {
                    query = query.Where(x => x.ExamId == examId.Value);
                }
                
                if (!string.IsNullOrEmpty(gradingMethod))
                {
                    query = query.Where(x => x.GradingMethod == gradingMethod);
                }
                
                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(x => x.GradedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                _logger.LogDebug("Đã lấy {Count} kết quả OCR từ tổng số {TotalCount}", items.Count, totalCount);
                return (items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR với phân trang");
                throw;
            }
        }

        public async Task<OCRResult?> GetByIdWithIncludeAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR với include theo ID: {Id}", id);
                
                return await _context.OCRResults
                    .Include(x => x.Student)
                    .Include(x => x.Exam)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR với include theo ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByStudentWithIncludeAsync(Guid studentId)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR với include theo học sinh: {StudentId}", studentId);
                
                return await _context.OCRResults
                    .Include(x => x.Student)
                    .Include(x => x.Exam)
                    .Where(x => x.StudentId == studentId)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR với include theo học sinh: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IEnumerable<OCRResult>> GetByExamWithIncludeAsync(Guid examId)
        {
            try
            {
                _logger.LogDebug("Lấy kết quả OCR với include theo đề thi: {ExamId}", examId);
                
                return await _context.OCRResults
                    .Include(x => x.Student)
                    .Include(x => x.Exam)
                    .Where(x => x.ExamId == examId)
                    .OrderByDescending(x => x.GradedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả OCR với include theo đề thi: {ExamId}", examId);
                throw;
            }
        }

        // === STATISTICS METHODS ===

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                _logger.LogDebug("Đếm tổng số kết quả OCR");
                
                return await _context.OCRResults.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm tổng số kết quả OCR");
                throw;
            }
        }

        public async Task<int> GetCountByStudentAsync(Guid studentId)
        {
            try
            {
                _logger.LogDebug("Đếm kết quả OCR theo học sinh: {StudentId}", studentId);
                
                return await _context.OCRResults
                    .CountAsync(x => x.StudentId == studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm kết quả OCR theo học sinh: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<int> GetCountByExamAsync(Guid examId)
        {
            try
            {
                _logger.LogDebug("Đếm kết quả OCR theo đề thi: {ExamId}", examId);
                
                return await _context.OCRResults
                    .CountAsync(x => x.ExamId == examId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm kết quả OCR theo đề thi: {ExamId}", examId);
                throw;
            }
        }

        public async Task<int> GetCountByGradingMethodAsync(string gradingMethod)
        {
            try
            {
                _logger.LogDebug("Đếm kết quả OCR theo phương thức chấm: {GradingMethod}", gradingMethod);
                
                return await _context.OCRResults
                    .CountAsync(x => x.GradingMethod == gradingMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm kết quả OCR theo phương thức chấm: {GradingMethod}", gradingMethod);
                throw;
            }
        }

        public async Task<int> GetCountByDateAsync(DateTime date)
        {
            try
            {
                _logger.LogDebug("Đếm kết quả OCR theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);
                
                return await _context.OCRResults
                    .CountAsync(x => x.GradedAt >= startDate && x.GradedAt < endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm kết quả OCR theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        // === ANALYTICS METHODS ===

        public async Task<decimal> GetAverageScoreByExamAsync(Guid examId)
        {
            try
            {
                _logger.LogDebug("Tính điểm trung bình theo đề thi: {ExamId}", examId);
                
                var average = await _context.OCRResults
                    .Where(x => x.ExamId == examId && x.Score.HasValue)
                    .AverageAsync(x => x.Score!.Value);
                
                return Math.Round(average, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính điểm trung bình theo đề thi: {ExamId}", examId);
                throw;
            }
        }

        public async Task<decimal> GetAverageScoreByStudentAsync(Guid studentId)
        {
            try
            {
                _logger.LogDebug("Tính điểm trung bình theo học sinh: {StudentId}", studentId);
                
                var average = await _context.OCRResults
                    .Where(x => x.StudentId == studentId && x.Score.HasValue)
                    .AverageAsync(x => x.Score!.Value);
                
                return Math.Round(average, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính điểm trung bình theo học sinh: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<decimal> GetAverageScoreByClassAsync(string className)
        {
            try
            {
                _logger.LogDebug("Tính điểm trung bình theo lớp: {ClassName}", className);
                
                // Giả sử có field Class trong Student entity
                // Nếu không có, cần bổ sung vào entity
                var average = await _context.OCRResults
                    .Include(x => x.Student)
                    .Where(x => x.Student.Lop == className && x.Score.HasValue)
                    .AverageAsync(x => x.Score!.Value);
                
                return Math.Round(average, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính điểm trung bình theo lớp: {ClassName}", className);
                throw;
            }
        }

        public async Task<object> GetScoreStatisticsByExamAsync(Guid examId)
        {
            try
            {
                _logger.LogDebug("Lấy thống kê điểm theo đề thi: {ExamId}", examId);
                
                var results = await _context.OCRResults
                    .Where(x => x.ExamId == examId && x.Score.HasValue)
                    .ToListAsync();
                
                if (!results.Any())
                {
                    return new
                    {
                        ExamId = examId,
                        TotalStudents = 0,
                        AverageScore = 0.0m,
                        HighestScore = 0.0m,
                        LowestScore = 0.0m,
                        PassedCount = 0,
                        FailedCount = 0
                    };
                }
                
                var scores = results.Select(x => x.Score!.Value).ToList();
                var averageScore = Math.Round(scores.Average(), 2);
                var highestScore = scores.Max();
                var lowestScore = scores.Min();
                var passedCount = scores.Count(x => x >= 5.0m);
                var failedCount = scores.Count(x => x < 5.0m);
                
                return new
                {
                    ExamId = examId,
                    TotalStudents = results.Count,
                    AverageScore = averageScore,
                    HighestScore = highestScore,
                    LowestScore = lowestScore,
                    PassedCount = passedCount,
                    FailedCount = failedCount,
                    PassRate = Math.Round((decimal)passedCount / results.Count * 100, 2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê điểm theo đề thi: {ExamId}", examId);
                throw;
            }
        }

        // === CLEANUP METHODS ===

        public async Task<int> CleanupOldResultsAsync(DateTime cutoffDate)
        {
            try
            {
                _logger.LogInformation("Dọn dẹp kết quả OCR cũ trước ngày: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd"));
                
                var oldResults = await _context.OCRResults
                    .Where(x => x.GradedAt < cutoffDate)
                    .ToListAsync();
                
                if (!oldResults.Any())
                {
                    _logger.LogInformation("Không có kết quả OCR cũ nào để dọn dẹp");
                    return 0;
                }
                
                _context.OCRResults.RemoveRange(oldResults);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã dọn dẹp {Count} kết quả OCR cũ", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi dọn dẹp kết quả OCR cũ");
                throw;
            }
        }

        public async Task<int> CleanupLowScoreResultsAsync(decimal minScore)
        {
            try
            {
                _logger.LogInformation("Dọn dẹp kết quả OCR có điểm thấp hơn: {MinScore}", minScore);
                
                var lowScoreResults = await _context.OCRResults
                    .Where(x => x.Score.HasValue && x.Score < minScore)
                    .ToListAsync();
                
                if (!lowScoreResults.Any())
                {
                    _logger.LogInformation("Không có kết quả OCR nào có điểm thấp để dọn dẹp");
                    return 0;
                }
                
                _context.OCRResults.RemoveRange(lowScoreResults);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã dọn dẹp {Count} kết quả OCR có điểm thấp", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi dọn dẹp kết quả OCR có điểm thấp");
                throw;
            }
        }
    }
}
