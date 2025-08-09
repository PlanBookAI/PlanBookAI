using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Data;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    // Lớp triển khai repository cho CauHoi
    public class CauHoiRepository : ICauHoiRepository
    {
        private readonly TaskDbContext _context;

        // Dependency Injection của TaskDbContext
        public CauHoiRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CauHoi>> GetAllAsync()
        {
            return await _context.CauHois.ToListAsync();
        }

        public async Task<CauHoi> GetByIdAsync(Guid id)
        {
            return await _context.CauHois.FindAsync(id);
        }

        public async Task AddAsync(CauHoi cauHoi)
        {
            await _context.CauHois.AddAsync(cauHoi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CauHoi cauHoi)
        {
            // Đánh dấu trạng thái của entity là đã được chỉnh sửa
            _context.Entry(cauHoi).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cauHoi = await _context.CauHois.FindAsync(id);
            if (cauHoi != null)
            {
                _context.CauHois.Remove(cauHoi);
                await _context.SaveChangesAsync();
            }
        }
    }
}
