using Microsoft.EntityFrameworkCore;
using PlanService.Data;
using PlanService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanService.Repositories
{
    /// <summary>
    /// Triển khai interface IMauGiaoAnRepository.
    /// </summary>
    public class MauGiaoAnRepository : IMauGiaoAnRepository
    {
        private readonly PlanDbContext _context;

        public MauGiaoAnRepository(PlanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MauGiaoAn>> GetAllAsync()
        {
            return await _context.MauGiaoAns.ToListAsync();
        }

        public async Task<MauGiaoAn> GetByIdAsync(Guid id)
        {
            return await _context.MauGiaoAns.FindAsync(id);
        }

        public async Task AddAsync(MauGiaoAn mauGiaoAn)
        {
            await _context.MauGiaoAns.AddAsync(mauGiaoAn);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MauGiaoAn mauGiaoAn)
        {
            _context.MauGiaoAns.Update(mauGiaoAn);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var mauGiaoAn = await GetByIdAsync(id);
            if (mauGiaoAn != null)
            {
                _context.MauGiaoAns.Remove(mauGiaoAn);
                await _context.SaveChangesAsync();
            }
        }
    }
}