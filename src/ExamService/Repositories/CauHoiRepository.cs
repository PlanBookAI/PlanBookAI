using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamService.Repositories
{
    public class CauHoiRepository : ICauHoiRepository // Đảm bảo kế thừa từ interface đúng
    {
        private readonly ExamDbContext _context;

        public CauHoiRepository(ExamDbContext context)
        {
            _context = context;
        }

        // Triển khai đúng GetAllAsync(Guid teacherId)
        public async Task<List<CauHoi>> GetAllAsync(Guid teacherId)
        {
            return await _context.CauHois
                .Where(c => c.NguoiTaoId == teacherId)
                .Include(c => c.LuaChons)
                .AsNoTracking()
                .ToListAsync();
        }

        // Triển khai đúng GetByIdAsync(Guid id)
        public async Task<CauHoi?> GetByIdAsync(Guid id)
        {
            return await _context.CauHois
                .Include(c => c.LuaChons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CauHoi> CreateAsync(CauHoi cauHoi)
        {
            await _context.CauHois.AddAsync(cauHoi);
            await _context.SaveChangesAsync();
            return cauHoi;
        }

        public async Task<CauHoi> UpdateAsync(CauHoi cauHoi)
        {
            await _context.SaveChangesAsync();
            return cauHoi;
        }

        // Triển khai đúng DeleteAsync(Guid id)
        public async Task<bool> DeleteAsync(Guid id)
        {
            var cauHoi = await _context.CauHois.FindAsync(id);
            if (cauHoi == null)
            {
                return false;
            }

            _context.CauHois.Remove(cauHoi);
            var changedEntries = await _context.SaveChangesAsync();
            return changedEntries > 0;
        }

        // Triển khai các phương thức mới
        public async Task<bool> IsOwnerAsync(Guid id, Guid teacherId)
        {
            return await _context.CauHois.AnyAsync(c => c.Id == id && c.NguoiTaoId == teacherId);
        }

        public IQueryable<CauHoi> GetQueryable()
        {
            return _context.CauHois.AsQueryable();
        }
    }
}