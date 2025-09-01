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
    /// Implementation của IOCRRequestRepository
    /// </summary>
    public class OCRRequestRepository : IOCRRequestRepository
    {
        private readonly OCRDbContext _context;
        private readonly ILogger<OCRRequestRepository> _logger;

        public OCRRequestRepository(OCRDbContext context, ILogger<OCRRequestRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // === CRUD OPERATIONS ===

        public async Task<OCRRequest> CreateAsync(OCRRequest ocrRequest)
        {
            try
            {
                _logger.LogInformation("Tạo yêu cầu OCR mới với ID: {Id}", ocrRequest.Id);
                
                ocrRequest.Id = Guid.NewGuid();
                ocrRequest.CreatedAt = DateTime.UtcNow;
                
                var result = await _context.OCRRequests.AddAsync(ocrRequest);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã tạo yêu cầu OCR thành công với ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo yêu cầu OCR");
                throw;
            }
        }

        public async Task<OCRRequest?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR theo ID: {Id}", id);
                
                return await _context.OCRRequests
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR theo ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OCRRequest ocrRequest)
        {
            try
            {
                _logger.LogInformation("Cập nhật yêu cầu OCR với ID: {Id}", ocrRequest.Id);
                
                _context.OCRRequests.Update(ocrRequest);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật yêu cầu OCR thành công với ID: {Id}", ocrRequest.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật yêu cầu OCR với ID: {Id}", ocrRequest.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Xóa yêu cầu OCR với ID: {Id}", id);
                
                var ocrRequest = await GetByIdAsync(id);
                if (ocrRequest == null)
                {
                    _logger.LogWarning("Không tìm thấy yêu cầu OCR để xóa với ID: {Id}", id);
                    return false;
                }
                
                _context.OCRRequests.Remove(ocrRequest);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã xóa yêu cầu OCR thành công với ID: {Id}", id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa yêu cầu OCR với ID: {Id}", id);
                throw;
            }
        }

        // === QUERY METHODS ===

        public async Task<IEnumerable<OCRRequest>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Lấy tất cả yêu cầu OCR");
                
                return await _context.OCRRequests
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả yêu cầu OCR");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetByStatusAsync(string status)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR theo trạng thái: {Status}", status);
                
                return await _context.OCRRequests
                    .Where(x => x.OcrStatus == status)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR theo trạng thái: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetByTeacherAsync(Guid teacherId)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR theo giáo viên: {TeacherId}", teacherId);
                
                // Giả sử có field TeacherId trong OCRRequest
                // Nếu không có, cần bổ sung vào entity
                return await _context.OCRRequests
                    .Where(x => x.StudentId == teacherId) // Tạm thời dùng StudentId
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR theo giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetByStudentAsync(Guid studentId)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR theo học sinh: {StudentId}", studentId);
                
                return await _context.OCRRequests
                    .Where(x => x.StudentId == studentId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR theo học sinh: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetByExamAsync(Guid examId)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR theo đề thi: {ExamId}", examId);
                
                return await _context.OCRRequests
                    .Where(x => x.ExamId == examId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR theo đề thi: {ExamId}", examId);
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR theo khoảng thời gian: {StartDate} - {EndDate}", startDate, endDate);
                
                return await _context.OCRRequests
                    .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR theo khoảng thời gian: {StartDate} - {EndDate}", startDate, endDate);
                throw;
            }
        }

        // === SPECIAL QUERY METHODS ===

        public async Task<IEnumerable<OCRRequest>> GetPendingRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR đang chờ xử lý");
                
                return await _context.OCRRequests
                    .Where(x => x.OcrStatus == "PENDING")
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR đang chờ xử lý");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetProcessingRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR đang xử lý");
                
                return await _context.OCRRequests
                    .Where(x => x.OcrStatus == "PROCESSING")
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR đang xử lý");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetCompletedRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR đã hoàn thành");
                
                return await _context.OCRRequests
                    .Where(x => x.OcrStatus == "COMPLETED")
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR đã hoàn thành");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetFailedRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR thất bại");
                
                return await _context.OCRRequests
                    .Where(x => x.OcrStatus == "FAILED")
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR thất bại");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetFallbackRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR cần fallback");
                
                return await _context.OCRRequests
                    .Where(x => x.FallbackStatus == "MANUAL_GRADING_REQUIRED")
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR cần fallback");
                throw;
            }
        }

        // === RETRY & FALLBACK METHODS ===

        public async Task<IEnumerable<OCRRequest>> GetRetryableRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR có thể retry");
                
                return await _context.OCRRequests
                    .Where(x => x.OcrStatus == "FAILED" && x.CanRetry)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR có thể retry");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetManualGradingRequestsAsync()
        {
            try
            {
                _logger.LogDebug("Lấy danh sách yêu cầu OCR cần chấm thủ công");
                
                return await _context.OCRRequests
                    .Where(x => x.ManualGradingRequired)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách yêu cầu OCR cần chấm thủ công");
                throw;
            }
        }

        // === BATCH OPERATIONS ===

        public async Task<bool> UpdateMultipleAsync(IEnumerable<OCRRequest> ocrRequests)
        {
            try
            {
                _logger.LogInformation("Cập nhật {Count} yêu cầu OCR", ocrRequests.Count());
                
                _context.OCRRequests.UpdateRange(ocrRequests);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật {Count} yêu cầu OCR thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhiều yêu cầu OCR");
                throw;
            }
        }

        public async Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            try
            {
                _logger.LogInformation("Xóa {Count} yêu cầu OCR", ids.Count());
                
                var ocrRequests = await _context.OCRRequests
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();
                
                if (!ocrRequests.Any())
                {
                    _logger.LogWarning("Không tìm thấy yêu cầu OCR nào để xóa");
                    return false;
                }
                
                _context.OCRRequests.RemoveRange(ocrRequests);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã xóa {Count} yêu cầu OCR thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhiều yêu cầu OCR");
                throw;
            }
        }

        // === PERFORMANCE METHODS ===

        public async Task<(IEnumerable<OCRRequest> Items, int TotalCount)> GetWithPaginationAsync(
            int pageNumber, int pageSize, string? status = null, Guid? teacherId = null)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR với phân trang: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                
                var query = _context.OCRRequests.AsQueryable();
                
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(x => x.OcrStatus == status);
                }
                
                if (teacherId.HasValue)
                {
                    query = query.Where(x => x.StudentId == teacherId.Value); // Tạm thời dùng StudentId
                }
                
                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                _logger.LogDebug("Đã lấy {Count} yêu cầu OCR từ tổng số {TotalCount}", items.Count, totalCount);
                return (items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR với phân trang");
                throw;
            }
        }

        public async Task<OCRRequest?> GetByIdWithIncludeAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR với include theo ID: {Id}", id);
                
                return await _context.OCRRequests
                    .Include(x => x.Student)
                    .Include(x => x.Exam)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR với include theo ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OCRRequest>> GetByStatusWithIncludeAsync(string status)
        {
            try
            {
                _logger.LogDebug("Lấy yêu cầu OCR với include theo trạng thái: {Status}", status);
                
                return await _context.OCRRequests
                    .Include(x => x.Student)
                    .Include(x => x.Exam)
                    .Where(x => x.OcrStatus == status)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy yêu cầu OCR với include theo trạng thái: {Status}", status);
                throw;
            }
        }

        // === STATISTICS METHODS ===

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                _logger.LogDebug("Đếm tổng số yêu cầu OCR");
                
                return await _context.OCRRequests.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm tổng số yêu cầu OCR");
                throw;
            }
        }

        public async Task<int> GetCountByStatusAsync(string status)
        {
            try
            {
                _logger.LogDebug("Đếm yêu cầu OCR theo trạng thái: {Status}", status);
                
                return await _context.OCRRequests
                    .CountAsync(x => x.OcrStatus == status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm yêu cầu OCR theo trạng thái: {Status}", status);
                throw;
            }
        }

        public async Task<int> GetCountByTeacherAsync(Guid teacherId)
        {
            try
            {
                _logger.LogDebug("Đếm yêu cầu OCR theo giáo viên: {TeacherId}", teacherId);
                
                return await _context.OCRRequests
                    .CountAsync(x => x.StudentId == teacherId); // Tạm thời dùng StudentId
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm yêu cầu OCR theo giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<int> GetCountByDateAsync(DateTime date)
        {
            try
            {
                _logger.LogDebug("Đếm yêu cầu OCR theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);
                
                return await _context.OCRRequests
                    .CountAsync(x => x.CreatedAt >= startDate && x.CreatedAt < endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm yêu cầu OCR theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        // === CLEANUP METHODS ===

        public async Task<int> CleanupOldRequestsAsync(DateTime cutoffDate)
        {
            try
            {
                _logger.LogInformation("Dọn dẹp yêu cầu OCR cũ trước ngày: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd"));
                
                var oldRequests = await _context.OCRRequests
                    .Where(x => x.CreatedAt < cutoffDate)
                    .ToListAsync();
                
                if (!oldRequests.Any())
                {
                    _logger.LogInformation("Không có yêu cầu OCR cũ nào để dọn dẹp");
                    return 0;
                }
                
                _context.OCRRequests.RemoveRange(oldRequests);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã dọn dẹp {Count} yêu cầu OCR cũ", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi dọn dẹp yêu cầu OCR cũ");
                throw;
            }
        }

        public async Task<int> CleanupOldFailedRequestsAsync(DateTime cutoffDate)
        {
            try
            {
                _logger.LogInformation("Dọn dẹp yêu cầu OCR thất bại cũ trước ngày: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd"));
                
                var oldFailedRequests = await _context.OCRRequests
                    .Where(x => x.OcrStatus == "FAILED" && x.CreatedAt < cutoffDate)
                    .ToListAsync();
                
                if (!oldFailedRequests.Any())
                {
                    _logger.LogInformation("Không có yêu cầu OCR thất bại cũ nào để dọn dẹp");
                    return 0;
                }
                
                _context.OCRRequests.RemoveRange(oldFailedRequests);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã dọn dẹp {Count} yêu cầu OCR thất bại cũ", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi dọn dẹp yêu cầu OCR thất bại cũ");
                throw;
            }
        }
    }
}

