using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Models.Entities;

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
            if (Guid.TryParse(id, out Guid guidId))
            {
                return await _context.HoSoNguoiDungs.FindAsync(guidId);
            }
            return null;
        }

        public async Task<HoSoNguoiDung?> GetByEmailAsync(string email)
        {
            // Tìm profile theo email bằng cách sử dụng NpgsqlParameter
            // Sử dụng đúng tên bảng từ migration: users.user_profiles
            var sql = @"
                SELECT h.* FROM users.user_profiles h
                WHERE h.user_id = (SELECT id FROM users.users WHERE email = @email)";
            
            var parameter = new Npgsql.NpgsqlParameter("@email", email);
            var profile = await _context.HoSoNguoiDungs
                .FromSqlRaw(sql, parameter)
                .FirstOrDefaultAsync();
            
            return profile;
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
