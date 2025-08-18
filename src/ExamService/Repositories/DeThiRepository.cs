using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Data;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    // Lớp triển khai repository cho DeThi
    public class DeThiRepository : IDeThiRepository
    {
        private readonly TaskDbContext _context;

        // Dependency Injection của TaskDbContext
        public DeThiRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeThi>> GetAllAsync()
        {
            return await _context.DeThis.ToListAsync();
        }

        public async Task<DeThi?> GetByIdAsync(string id)
        {
            return await _context.DeThis.FindAsync(id);
        }

        public async Task<DeThi> CreateAsync(DeThi deThi)
        {
            await _context.DeThis.AddAsync(deThi);
            await _context.SaveChangesAsync();
            return deThi;
        }

        public async Task<DeThi> UpdateAsync(DeThi deThi)
        {
            _context.Entry(deThi).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return deThi;
        }

        public async Task DeleteAsync(string id)
        {
            var deThi = await _context.DeThis.FindAsync(id);
            if (deThi != null)
            {
                _context.DeThis.Remove(deThi);
                await _context.SaveChangesAsync();
            }
        }
    }
}
