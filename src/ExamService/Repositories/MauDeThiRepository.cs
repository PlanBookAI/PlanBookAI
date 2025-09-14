using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamService.Repositories
{
    public class MauDeThiRepository : IMauDeThiRepository
    {
        private readonly ExamDbContext _context;

        public MauDeThiRepository(ExamDbContext context)
        {
            _context = context;
        }

        public IQueryable<MauDeThi> GetQueryable()
        {
            return _context.MauDeThis.AsQueryable();
        }

        public async Task<MauDeThi?> GetByIdAsync(Guid id)
        {
            return await _context.MauDeThis.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MauDeThi> CreateAsync(MauDeThi mauDeThi)
        {
            await _context.MauDeThis.AddAsync(mauDeThi);
            await _context.SaveChangesAsync();
            return mauDeThi;
        }

        public async Task<MauDeThi> UpdateAsync(MauDeThi mauDeThi)
        {
            _context.MauDeThis.Update(mauDeThi);
            await _context.SaveChangesAsync();
            return mauDeThi;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var mauDeThi = await _context.MauDeThis.FindAsync(id);
            if (mauDeThi == null) return false;
            _context.MauDeThis.Remove(mauDeThi);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<MauDeThi>> GetByCreatedByAsync(Guid createdBy)
        {
            return await _context.MauDeThis
                .Where(m => m.NguoiTaoId == createdBy)
                .OrderByDescending(m => m.TaoLuc)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.MauDeThis.AnyAsync(m => m.Id == id);
        }

        public async Task<bool> IsOwnerAsync(Guid id, Guid teacherId)
        {
            return await _context.MauDeThis.AnyAsync(m => m.Id == id && m.NguoiTaoId == teacherId);
        }
    }
}