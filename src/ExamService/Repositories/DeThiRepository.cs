using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamService.Repositories
{
    public class DeThiRepository : IDeThiRepository
    {
        private readonly ExamDbContext _context;

        public DeThiRepository(ExamDbContext context)
        {
            _context = context;
        }

        public IQueryable<DeThi> GetQueryable()
        {
            return _context.DeThis.AsQueryable();
        }

        public async Task<DeThi?> GetByIdAsync(Guid id)
        {
            return await _context.DeThis
                .Include(d => d.ExamQuestions)
                    .ThenInclude(eq => eq.CauHoi)
                        .ThenInclude(q => q.LuaChons)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DeThi> CreateAsync(DeThi deThi)
        {
            await _context.DeThis.AddAsync(deThi);
            await _context.SaveChangesAsync();
            return deThi;
        }

        public async Task UpdateAsync(DeThi deThi)
        {
            _context.DeThis.Update(deThi);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deThi = await _context.DeThis.FindAsync(id);
            if (deThi == null) return false;

            _context.DeThis.Remove(deThi);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsOwnerAsync(Guid id, Guid teacherId)
        {
            return await _context.DeThis.AnyAsync(d => d.Id == id && d.NguoiTaoId == teacherId);
        }

        public async Task<DeThi?> GetByIdWithResultsAsync(Guid id)
        {
            // Tải kèm tất cả các dữ liệu liên quan cần thiết cho việc thống kê
            return await _context.DeThis
                .Include(d => d.ExamQuestions)
                    .ThenInclude(eq => eq.CauHoi)
                .Include(d => d.BaiLams)
                    .ThenInclude(bl => bl.KetQua)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

    }
}