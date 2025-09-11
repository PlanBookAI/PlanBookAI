using Microsoft.EntityFrameworkCore;
using PlanService.Data;
using PlanService.Models.Entities;

namespace PlanService.Repositories
{
    public class ChuDeRepository : IChuDeRepository
    {
        private readonly PlanDbContext _context;
        
        public ChuDeRepository(PlanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChuDe>> GetAllAsync()
        {
            return await _context.ChuDes
                .OrderBy(c => c.TaoLuc)
                .ToListAsync();
        }

        public async Task<ChuDe?> GetByIdAsync(Guid id)
        {
            return await _context.ChuDes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<ChuDe>> GetByMonHocAsync(string monHoc)
        {
            return await _context.ChuDes
                .Where(c => c.MonHoc == monHoc)
                .OrderBy(c => c.TaoLuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChuDe>> GetByParentIdAsync(Guid? parentId)
        {
            return await _context.ChuDes
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.TaoLuc)
                .ToListAsync();
        }

        public async Task AddAsync(ChuDe chuDe)
        {
            await _context.ChuDes.AddAsync(chuDe);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChuDe chuDe)
        {
            _context.ChuDes.Update(chuDe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ChuDes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
