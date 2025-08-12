using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Models;

namespace UserService.Repositories
{
    // Lớp triển khai repository cho HoSoNguoiDung
    public class HoSoNguoiDungRepository : IHoSoNguoiDungRepository
    {
        private readonly UserDbContext _context;

        // Dependency Injection của UserDbContext
        public HoSoNguoiDungRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HoSoNguoiDung>> GetAllAsync()
        {
            return await _context.HoSoNguoiDungs.ToListAsync();
        }

        public async Task<HoSoNguoiDung?> GetByIdAsync(string id)
        {
            return await _context.HoSoNguoiDungs.FindAsync(id);
        }

        public async Task AddAsync(HoSoNguoiDung hoSo)
        {
            await _context.HoSoNguoiDungs.AddAsync(hoSo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HoSoNguoiDung hoSo)
        {
            _context.Entry(hoSo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var hoSo = await _context.HoSoNguoiDungs.FindAsync(id);
            if (hoSo != null)
            {
                _context.HoSoNguoiDungs.Remove(hoSo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
