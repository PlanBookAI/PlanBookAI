using Microsoft.EntityFrameworkCore;
using PlanService.Data;
using PlanService.Models.Entities;

namespace PlanService.Repositories
{
    /// <summary>
    /// Implementation của IGiaoAnRepository
    /// Xử lý truy cập dữ liệu cho Giáo Án theo Repository pattern
    /// </summary>
    public class GiaoAnRepository : IGiaoAnRepository
    {
        private readonly PlanDbContext _context;

        public GiaoAnRepository(PlanDbContext context)
        {
            _context = context;
        }

        // Basic CRUD Operations
        public async Task<IEnumerable<GiaoAn>> GetAllAsync()
        {
            return await _context.GiaoAns
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<GiaoAn?> GetByIdAsync(Guid id)
        {
            return await _context.GiaoAns
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task AddAsync(GiaoAn giaoAn)
        {
            _context.GiaoAns.Add(giaoAn);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GiaoAn giaoAn)
        {
            _context.GiaoAns.Update(giaoAn);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var giaoAn = await _context.GiaoAns.FindAsync(id);
            if (giaoAn != null)
            {
                _context.GiaoAns.Remove(giaoAn);
                await _context.SaveChangesAsync();
            }
        }

        // Teacher Isolation 
        public async Task<IEnumerable<GiaoAn>> GetByTeacherIdAsync(Guid teacherId)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId)
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<GiaoAn?> GetByIdAndTeacherIdAsync(Guid id, Guid teacherId)
        {
            return await _context.GiaoAns
                .FirstOrDefaultAsync(g => g.Id == id && g.GiaoVienId == teacherId);
        }

        // Search & Filter Methods 
        public async Task<IEnumerable<GiaoAn>> SearchAsync(string keyword, Guid teacherId)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId &&
                           (g.TieuDe.Contains(keyword) ||
                            (g.MucTieu != null && g.MucTieu.Contains(keyword))))
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiaoAn>> GetByTeacherIdAndSubjectAsync(Guid teacherId, string monHoc)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId && g.MonHoc == monHoc)
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiaoAn>> GetByTeacherIdAndGradeAsync(Guid teacherId, int khoi)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId && g.Lop == khoi)
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiaoAn>> GetByTeacherIdAndStatusAsync(Guid teacherId, string status)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId && g.TrangThai == status)
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiaoAn>> GetByTeacherIdAndTemplateIdAsync(Guid teacherId, Guid templateId)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId && g.MauGiaoAnId == templateId)
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiaoAn>> GetByTeacherIdAndTopicAsync(Guid teacherId, Guid topicId)
        {
            return await _context.GiaoAns
                .Where(g => g.GiaoVienId == teacherId && g.ChuDeId == topicId)
                .OrderByDescending(g => g.TaoLuc)
                .ToListAsync();
        }

        // Status Management
        public async Task<GiaoAn?> UpdateStatusAsync(Guid id, string status)
        {
            var giaoAn = await _context.GiaoAns.FindAsync(id);
            if (giaoAn != null)
            {
                giaoAn.TrangThai = status;
                giaoAn.CapNhatLuc = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return giaoAn;
        }
    }
}
