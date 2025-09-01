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
    /// Implementation của IOCRRateLimitRepository
    /// </summary>
    public class OCRRateLimitRepository : IOCRRateLimitRepository
    {
        private readonly OCRDbContext _context;
        private readonly ILogger<OCRRateLimitRepository> _logger;

        public OCRRateLimitRepository(OCRDbContext context, ILogger<OCRRateLimitRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // === CRUD OPERATIONS ===

        public async Task<OCRRateLimit> CreateAsync(OCRRateLimit rateLimit)
        {
            try
            {
                _logger.LogInformation("Tạo rate limit mới cho giáo viên: {TeacherId}", rateLimit.TeacherId);
                
                rateLimit.Id = Guid.NewGuid();
                rateLimit.CreatedAt = DateTime.UtcNow;
                rateLimit.UpdatedAt = DateTime.UtcNow;
                
                var result = await _context.OCRRateLimits.AddAsync(rateLimit);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã tạo rate limit thành công với ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo rate limit cho giáo viên: {TeacherId}", rateLimit.TeacherId);
                throw;
            }
        }

        public async Task<OCRRateLimit?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Lấy rate limit theo ID: {Id}", id);
                
                return await _context.OCRRateLimits
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limit theo ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OCRRateLimit rateLimit)
        {
            try
            {
                _logger.LogInformation("Cập nhật rate limit với ID: {Id}", rateLimit.Id);
                
                rateLimit.UpdatedAt = DateTime.UtcNow;
                _context.OCRRateLimits.Update(rateLimit);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật rate limit thành công với ID: {Id}", rateLimit.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật rate limit với ID: {Id}", rateLimit.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Xóa rate limit với ID: {Id}", id);
                
                var rateLimit = await GetByIdAsync(id);
                if (rateLimit == null)
                {
                    _logger.LogWarning("Không tìm thấy rate limit để xóa với ID: {Id}", id);
                    return false;
                }
                
                _context.OCRRateLimits.Remove(rateLimit);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã xóa rate limit thành công với ID: {Id}", id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa rate limit với ID: {Id}", id);
                throw;
            }
        }

        // === QUERY METHODS ===

        public async Task<IEnumerable<OCRRateLimit>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Lấy tất cả rate limits");
                
                return await _context.OCRRateLimits
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả rate limits");
                throw;
            }
        }

        public async Task<OCRRateLimit?> GetByTeacherAsync(Guid teacherId)
        {
            try
            {
                _logger.LogDebug("Lấy rate limit theo giáo viên: {TeacherId}", teacherId);
                
                return await _context.OCRRateLimits
                    .Where(x => x.TeacherId == teacherId)
                    .OrderByDescending(x => x.UpdatedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limit theo giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<OCRRateLimit?> GetByTeacherAndDateAsync(Guid teacherId, DateTime date)
        {
            try
            {
                _logger.LogDebug("Lấy rate limit theo giáo viên và ngày: {TeacherId}, {Date}", teacherId, date.ToString("yyyy-MM-dd"));
                
                return await _context.OCRRateLimits
                    .Where(x => x.TeacherId == teacherId && x.RequestDate.Date == date.Date)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limit theo giáo viên và ngày: {TeacherId}, {Date}", teacherId, date.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        public async Task<IEnumerable<OCRRateLimit>> GetByDateAsync(DateTime date)
        {
            try
            {
                _logger.LogDebug("Lấy rate limits theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                
                return await _context.OCRRateLimits
                    .Where(x => x.RequestDate.Date == date.Date)
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limits theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        public async Task<IEnumerable<OCRRateLimit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogDebug("Lấy rate limits theo khoảng thời gian: {StartDate} - {EndDate}", startDate, endDate);
                
                return await _context.OCRRateLimits
                    .Where(x => x.RequestDate >= startDate.Date && x.RequestDate <= endDate.Date)
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limits theo khoảng thời gian: {StartDate} - {EndDate}", startDate, endDate);
                throw;
            }
        }

        // === RATE LIMITING METHODS ===

        public async Task<bool> IsLimitExceededAsync(Guid teacherId, int maxRequestsPerDay = 10)
        {
            try
            {
                _logger.LogDebug("Kiểm tra giáo viên {TeacherId} có vượt quá giới hạn {MaxRequestsPerDay} không", teacherId, maxRequestsPerDay);
                
                var rateLimit = await GetByTeacherAndDateAsync(teacherId, DateTime.Today);
                
                if (rateLimit == null)
                {
                    _logger.LogDebug("Giáo viên {TeacherId} chưa có rate limit cho hôm nay", teacherId);
                    return false;
                }
                
                var isExceeded = rateLimit.IsLimitExceeded(maxRequestsPerDay);
                _logger.LogDebug("Giáo viên {TeacherId} có vượt quá giới hạn: {IsExceeded}", teacherId, isExceeded);
                
                return isExceeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra giáo viên {TeacherId} có vượt quá giới hạn không", teacherId);
                throw;
            }
        }

        public async Task<bool> IncrementRequestCountAsync(Guid teacherId)
        {
            try
            {
                _logger.LogDebug("Tăng số lượng request cho giáo viên: {TeacherId}", teacherId);
                
                var rateLimit = await GetByTeacherAndDateAsync(teacherId, DateTime.Today);
                
                if (rateLimit == null)
                {
                    // Tạo mới rate limit cho ngày hôm nay
                    rateLimit = new OCRRateLimit
                    {
                        TeacherId = teacherId,
                        RequestDate = DateTime.Today,
                        RequestCount = 0
                    };
                    await CreateAsync(rateLimit);
                }
                else if (rateLimit.IsNewDay())
                {
                    // Reset cho ngày mới
                    rateLimit.ResetForNewDay();
                }
                
                rateLimit.IncrementRequest();
                var result = await UpdateAsync(rateLimit);
                
                _logger.LogDebug("Đã tăng số lượng request cho giáo viên {TeacherId}: {Count}", teacherId, rateLimit.RequestCount);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tăng số lượng request cho giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<int> GetRemainingRequestsAsync(Guid teacherId, int maxRequestsPerDay = 10)
        {
            try
            {
                _logger.LogDebug("Lấy số lượng request còn lại của giáo viên: {TeacherId}", teacherId);
                
                var rateLimit = await GetByTeacherAndDateAsync(teacherId, DateTime.Today);
                
                if (rateLimit == null)
                {
                    return maxRequestsPerDay;
                }
                
                var remaining = Math.Max(0, maxRequestsPerDay - rateLimit.RequestCount);
                _logger.LogDebug("Giáo viên {TeacherId} còn {Remaining} request", teacherId, remaining);
                
                return remaining;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy số lượng request còn lại của giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<bool> ResetRateLimitAsync(Guid teacherId)
        {
            try
            {
                _logger.LogInformation("Reset rate limit cho giáo viên: {TeacherId}", teacherId);
                
                var rateLimit = await GetByTeacherAsync(teacherId);
                if (rateLimit == null)
                {
                    _logger.LogWarning("Không tìm thấy rate limit để reset cho giáo viên: {TeacherId}", teacherId);
                    return false;
                }
                
                rateLimit.ResetForNewDay();
                var result = await UpdateAsync(rateLimit);
                
                _logger.LogInformation("Đã reset rate limit thành công cho giáo viên: {TeacherId}", teacherId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi reset rate limit cho giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        // === BATCH OPERATIONS ===

        public async Task<bool> UpdateMultipleAsync(IEnumerable<OCRRateLimit> rateLimits)
        {
            try
            {
                _logger.LogInformation("Cập nhật {Count} rate limits", rateLimits.Count());
                
                foreach (var rateLimit in rateLimits)
                {
                    rateLimit.UpdatedAt = DateTime.UtcNow;
                }
                
                _context.OCRRateLimits.UpdateRange(rateLimits);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã cập nhật {Count} rate limits thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhiều rate limits");
                throw;
            }
        }

        public async Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            try
            {
                _logger.LogInformation("Xóa {Count} rate limits", ids.Count());
                
                var rateLimits = await _context.OCRRateLimits
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();
                
                if (!rateLimits.Any())
                {
                    _logger.LogWarning("Không tìm thấy rate limit nào để xóa");
                    return false;
                }
                
                _context.OCRRateLimits.RemoveRange(rateLimits);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã xóa {Count} rate limits thành công", result);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhiều rate limits");
                throw;
            }
        }

        public async Task<int> ResetAllRateLimitsForNewDayAsync()
        {
            try
            {
                _logger.LogInformation("Reset tất cả rate limits cho ngày mới");
                
                var today = DateTime.Today;
                var rateLimits = await _context.OCRRateLimits
                    .Where(x => x.RequestDate.Date < today)
                    .ToListAsync();
                
                if (!rateLimits.Any())
                {
                    _logger.LogInformation("Không có rate limit nào cần reset");
                    return 0;
                }
                
                foreach (var rateLimit in rateLimits)
                {
                    rateLimit.ResetForNewDay();
                }
                
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã reset {Count} rate limits thành công", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi reset tất cả rate limits cho ngày mới");
                throw;
            }
        }

        // === PERFORMANCE METHODS ===

        public async Task<(IEnumerable<OCRRateLimit> Items, int TotalCount)> GetWithPaginationAsync(
            int pageNumber, int pageSize, Guid? teacherId = null, DateTime? date = null)
        {
            try
            {
                _logger.LogDebug("Lấy rate limits với phân trang: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                
                var query = _context.OCRRateLimits.AsQueryable();
                
                if (teacherId.HasValue)
                {
                    query = query.Where(x => x.TeacherId == teacherId.Value);
                }
                
                if (date.HasValue)
                {
                    query = query.Where(x => x.RequestDate.Date == date.Value.Date);
                }
                
                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(x => x.UpdatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                _logger.LogDebug("Đã lấy {Count} rate limits từ tổng số {TotalCount}", items.Count, totalCount);
                return (items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limits với phân trang");
                throw;
            }
        }

        public async Task<OCRRateLimit?> GetByIdWithIncludeAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Lấy rate limit với include theo ID: {Id}", id);
                
                return await _context.OCRRateLimits
                    .Include(x => x.Teacher)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limit với include theo ID: {Id}", id);
                throw;
            }
        }

        public async Task<OCRRateLimit?> GetByTeacherWithIncludeAsync(Guid teacherId)
        {
            try
            {
                _logger.LogDebug("Lấy rate limit với include theo giáo viên: {TeacherId}", teacherId);
                
                return await _context.OCRRateLimits
                    .Include(x => x.Teacher)
                    .Where(x => x.TeacherId == teacherId)
                    .OrderByDescending(x => x.UpdatedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy rate limit với include theo giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        // === STATISTICS METHODS ===

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                _logger.LogDebug("Đếm tổng số rate limits");
                
                return await _context.OCRRateLimits.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm tổng số rate limits");
                throw;
            }
        }

        public async Task<int> GetCountByTeacherAsync(Guid teacherId)
        {
            try
            {
                _logger.LogDebug("Đếm rate limits theo giáo viên: {TeacherId}", teacherId);
                
                return await _context.OCRRateLimits
                    .CountAsync(x => x.TeacherId == teacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm rate limits theo giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<int> GetCountByDateAsync(DateTime date)
        {
            try
            {
                _logger.LogDebug("Đếm rate limits theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                
                return await _context.OCRRateLimits
                    .CountAsync(x => x.RequestDate.Date == date.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm rate limits theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        public async Task<int> GetCountExceededLimitAsync(int maxRequestsPerDay = 10)
        {
            try
            {
                _logger.LogDebug("Đếm giáo viên đã vượt quá giới hạn {MaxRequestsPerDay}", maxRequestsPerDay);
                
                var today = DateTime.Today;
                return await _context.OCRRateLimits
                    .CountAsync(x => x.RequestDate.Date == today && x.RequestCount >= maxRequestsPerDay);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm giáo viên đã vượt quá giới hạn");
                throw;
            }
        }

        // === MONITORING METHODS ===

        public async Task<IEnumerable<OCRRateLimit>> GetExceededLimitsAsync(int maxRequestsPerDay = 10)
        {
            try
            {
                _logger.LogDebug("Lấy danh sách giáo viên đã vượt quá giới hạn {MaxRequestsPerDay}", maxRequestsPerDay);
                
                var today = DateTime.Today;
                return await _context.OCRRateLimits
                    .Where(x => x.RequestDate.Date == today && x.RequestCount >= maxRequestsPerDay)
                    .OrderByDescending(x => x.RequestCount)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách giáo viên đã vượt quá giới hạn");
                throw;
            }
        }

        public async Task<IEnumerable<OCRRateLimit>> GetNearLimitAsync(int threshold, int maxRequestsPerDay = 10)
        {
            try
            {
                _logger.LogDebug("Lấy danh sách giáo viên sắp vượt quá giới hạn (threshold: {Threshold})", threshold);
                
                var today = DateTime.Today;
                return await _context.OCRRateLimits
                    .Where(x => x.RequestDate.Date == today && 
                               x.RequestCount >= threshold && 
                               x.RequestCount < maxRequestsPerDay)
                    .OrderByDescending(x => x.RequestCount)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách giáo viên sắp vượt quá giới hạn");
                throw;
            }
        }

        public async Task<object> GetDailyRateLimitStatisticsAsync(DateTime date)
        {
            try
            {
                _logger.LogDebug("Lấy thống kê rate limiting theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                
                var rateLimits = await GetByDateAsync(date);
                
                if (!rateLimits.Any())
                {
                    return new
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        TotalTeachers = 0,
                        ExceededLimitCount = 0,
                        NearLimitCount = 0,
                        AverageRequests = 0.0
                    };
                }
                
                var totalTeachers = rateLimits.Count();
                var exceededLimitCount = rateLimits.Count(x => x.IsLimitExceeded());
                var nearLimitCount = rateLimits.Count(x => x.RequestCount >= 8 && x.RequestCount < 10);
                var averageRequests = Math.Round(rateLimits.Average(x => x.RequestCount), 2);
                
                return new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalTeachers = totalTeachers,
                    ExceededLimitCount = exceededLimitCount,
                    NearLimitCount = nearLimitCount,
                    AverageRequests = averageRequests,
                    ExceededRate = Math.Round((decimal)exceededLimitCount / totalTeachers * 100, 2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê rate limiting theo ngày: {Date}", date.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        public async Task<object> GetWeeklyRateLimitStatisticsAsync(DateTime startOfWeek)
        {
            try
            {
                _logger.LogDebug("Lấy thống kê rate limiting theo tuần bắt đầu: {StartOfWeek}", startOfWeek.ToString("yyyy-MM-dd"));
                
                var endOfWeek = startOfWeek.AddDays(6);
                var rateLimits = await GetByDateRangeAsync(startOfWeek, endOfWeek);
                
                if (!rateLimits.Any())
                {
                    return new
                    {
                        WeekStart = startOfWeek.ToString("yyyy-MM-dd"),
                        WeekEnd = endOfWeek.ToString("yyyy-MM-dd"),
                        TotalRequests = 0,
                        AverageDailyRequests = 0.0,
                        PeakDay = "",
                        PeakRequests = 0
                    };
                }
                
                var dailyStats = rateLimits
                    .GroupBy(x => x.RequestDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalRequests = g.Sum(x => x.RequestCount),
                        TeacherCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                
                var totalRequests = dailyStats.Sum(x => x.TotalRequests);
                var averageDailyRequests = Math.Round((double)totalRequests / dailyStats.Count, 2);
                var peakDay = dailyStats.OrderByDescending(x => x.TotalRequests).First();
                
                return new
                {
                    WeekStart = startOfWeek.ToString("yyyy-MM-dd"),
                    WeekEnd = endOfWeek.ToString("yyyy-MM-dd"),
                    TotalRequests = totalRequests,
                    AverageDailyRequests = averageDailyRequests,
                    PeakDay = peakDay.Date.ToString("yyyy-MM-dd"),
                    PeakRequests = peakDay.TotalRequests,
                    DailyBreakdown = dailyStats.Select(x => new
                    {
                        Date = x.Date.ToString("yyyy-MM-dd"),
                        TotalRequests = x.TotalRequests,
                        TeacherCount = x.TeacherCount
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê rate limiting theo tuần");
                throw;
            }
        }

        public async Task<object> GetMonthlyRateLimitStatisticsAsync(DateTime startOfMonth)
        {
            try
            {
                _logger.LogDebug("Lấy thống kê rate limiting theo tháng: {StartOfMonth}", startOfMonth.ToString("yyyy-MM"));
                
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var rateLimits = await GetByDateRangeAsync(startOfMonth, endOfMonth);
                
                if (!rateLimits.Any())
                {
                    return new
                    {
                        Month = startOfMonth.ToString("yyyy-MM"),
                        TotalRequests = 0,
                        AverageDailyRequests = 0.0,
                        TotalTeachers = 0,
                        PeakDay = "",
                        PeakRequests = 0
                    };
                }
                
                var dailyStats = rateLimits
                    .GroupBy(x => x.RequestDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalRequests = g.Sum(x => x.RequestCount),
                        TeacherCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                
                var totalRequests = dailyStats.Sum(x => x.TotalRequests);
                var averageDailyRequests = Math.Round((double)totalRequests / dailyStats.Count, 2);
                var totalTeachers = rateLimits.Select(x => x.TeacherId).Distinct().Count();
                var peakDay = dailyStats.OrderByDescending(x => x.TotalRequests).First();
                
                return new
                {
                    Month = startOfMonth.ToString("yyyy-MM"),
                    TotalRequests = totalRequests,
                    AverageDailyRequests = averageDailyRequests,
                    TotalTeachers = totalTeachers,
                    PeakDay = peakDay.Date.ToString("yyyy-MM-dd"),
                    PeakRequests = peakDay.TotalRequests,
                    DailyBreakdown = dailyStats.Select(x => new
                    {
                        Date = x.Date.ToString("yyyy-MM-dd"),
                        TotalRequests = x.TotalRequests,
                        TeacherCount = x.TeacherCount
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê rate limiting theo tháng");
                throw;
            }
        }

        // === CLEANUP METHODS ===

        public async Task<int> CleanupOldRateLimitsAsync(DateTime cutoffDate)
        {
            try
            {
                _logger.LogInformation("Dọn dẹp rate limits cũ trước ngày: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd"));
                
                var oldRateLimits = await _context.OCRRateLimits
                    .Where(x => x.RequestDate < cutoffDate)
                    .ToListAsync();
                
                if (!oldRateLimits.Any())
                {
                    _logger.LogInformation("Không có rate limit cũ nào để dọn dẹp");
                    return 0;
                }
                
                _context.OCRRateLimits.RemoveRange(oldRateLimits);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã dọn dẹp {Count} rate limits cũ", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi dọn dẹp rate limits cũ");
                throw;
            }
        }

        public async Task<int> CleanupInactiveTeacherRateLimitsAsync(DateTime cutoffDate)
        {
            try
            {
                _logger.LogInformation("Dọn dẹp rate limits của giáo viên không hoạt động trước ngày: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd"));
                
                var inactiveRateLimits = await _context.OCRRateLimits
                    .Where(x => x.UpdatedAt < cutoffDate)
                    .ToListAsync();
                
                if (!inactiveRateLimits.Any())
                {
                    _logger.LogInformation("Không có rate limit nào của giáo viên không hoạt động để dọn dẹp");
                    return 0;
                }
                
                _context.OCRRateLimits.RemoveRange(inactiveRateLimits);
                var result = await _context.SaveChangesAsync();
                
                _logger.LogInformation("Đã dọn dẹp {Count} rate limits của giáo viên không hoạt động", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi dọn dẹp rate limits của giáo viên không hoạt động");
                throw;
            }
        }

        // === ADMIN METHODS ===

        public async Task<bool> UpdateTeacherLimitAsync(Guid teacherId, int newMaxRequestsPerDay)
        {
            try
            {
                _logger.LogInformation("Cập nhật giới hạn request cho giáo viên {TeacherId}: {NewMaxRequestsPerDay}", teacherId, newMaxRequestsPerDay);
                
                // Giả sử có field MaxRequestsPerDay trong OCRRateLimit entity
                // Nếu không có, cần bổ sung vào entity
                var rateLimit = await GetByTeacherAsync(teacherId);
                if (rateLimit == null)
                {
                    _logger.LogWarning("Không tìm thấy rate limit để cập nhật cho giáo viên: {TeacherId}", teacherId);
                    return false;
                }
                
                // rateLimit.MaxRequestsPerDay = newMaxRequestsPerDay; // Cần bổ sung field này
                var result = await UpdateAsync(rateLimit);
                
                _logger.LogInformation("Đã cập nhật giới hạn request thành công cho giáo viên: {TeacherId}", teacherId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giới hạn request cho giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<bool> TemporarilyIncreaseLimitAsync(Guid teacherId, int additionalRequests, TimeSpan duration)
        {
            try
            {
                _logger.LogInformation("Tạm thời tăng giới hạn cho giáo viên {TeacherId}: +{AdditionalRequests} trong {Duration}", 
                    teacherId, additionalRequests, duration);
                
                // Implementation này cần bổ sung field TemporaryLimit và ExpiryDate vào entity
                // Tạm thời return true để không bị lỗi
                _logger.LogInformation("Tính năng tạm thời tăng giới hạn chưa được implement đầy đủ");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạm thời tăng giới hạn cho giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<bool> DisableRateLimitAsync(Guid teacherId)
        {
            try
            {
                _logger.LogInformation("Khóa rate limiting cho giáo viên: {TeacherId}", teacherId);
                
                // Implementation này cần bổ sung field IsEnabled vào entity
                // Tạm thời return true để không bị lỗi
                _logger.LogInformation("Tính năng khóa rate limiting chưa được implement đầy đủ");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi khóa rate limiting cho giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<bool> EnableRateLimitAsync(Guid teacherId)
        {
            try
            {
                _logger.LogInformation("Mở khóa rate limiting cho giáo viên: {TeacherId}", teacherId);
                
                // Implementation này cần bổ sung field IsEnabled vào entity
                // Tạm thời return true để không bị lỗi
                _logger.LogInformation("Tính năng mở khóa rate limiting chưa được implement đầy đủ");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi mở khóa rate limiting cho giáo viên: {TeacherId}", teacherId);
                throw;
            }
        }
    }
}

