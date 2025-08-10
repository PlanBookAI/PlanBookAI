using Microsoft.EntityFrameworkCore;
using PlanService.Data;
using PlanService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanService.Repositories
{
    /// <summary>
    /// Triển khai interface IGiaoAnRepository, sử dụng Entity Framework Core để thao tác với database.
    /// </summary>
    public class GiaoAnRepository : IGiaoAnRepository
    {
        private readonly PlanDbContext _context;

        public GiaoAnRepository(PlanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiaoAn>> GetAllAsync()
        {
            // Tải cả NoiDungGiaoDuc và MucTieu kèm theo Giáo Án
            return await _context.GiaoAns
                                 .Include(g => g.NoiDungGiaoDucs)
                                 .Include(g => g.MucTieus)
                                 .ToListAsync();
        }

        public async Task<GiaoAn> GetByIdAsync(Guid id)
        {
            return await _context.GiaoAns
                                 .Include(g => g.NoiDungGiaoDucs)
                                 .Include(g => g.MucTieus)
                                 .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task AddAsync(GiaoAn giaoAn)
        {
            await _context.GiaoAns.AddAsync(giaoAn);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào database
        }

        public async Task UpdateAsync(GiaoAn giaoAn)
        {
            _context.GiaoAns.Update(giaoAn);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var giaoAn = await GetByIdAsync(id);
            if (giaoAn != null)
            {
                _context.GiaoAns.Remove(giaoAn);
                await _context.SaveChangesAsync();
            }
        }
    }
}