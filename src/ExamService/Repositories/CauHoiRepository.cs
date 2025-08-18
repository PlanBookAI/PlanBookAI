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

        public async Task<List<CauHoi>> GetAllAsync()
        {
            return await _context.CauHois.ToListAsync();
        }

        public async Task<CauHoi?> GetByIdAsync(string id)
        {
            if (Guid.TryParse(id, out Guid guidId))
            {
                return await _context.CauHois.FindAsync(guidId);
            }
            return null;
        }

        public async Task<List<CauHoi>> GetByMonHocAndDoKhoAsync(string monHoc, string doKho)
        {
            return await _context.CauHois
                .Where(c => c.MonHoc == monHoc && c.DoKho == doKho)
                .ToListAsync();
        }

        public async Task<CauHoi> CreateAsync(CauHoi cauHoi)
        {
            await _context.CauHois.AddAsync(cauHoi);
            await _context.SaveChangesAsync();
            return cauHoi;
        }

        public async Task<CauHoi> UpdateAsync(CauHoi cauHoi)
        {
            // Đánh dấu trạng thái của entity là đã được chỉnh sửa
            _context.Entry(cauHoi).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return cauHoi;
        }

        public async Task DeleteAsync(string id)
        {
            if (Guid.TryParse(id, out Guid guidId))
            {
                var cauHoi = await _context.CauHois.FindAsync(guidId);
                if (cauHoi != null)
                {
                    _context.CauHois.Remove(cauHoi);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
