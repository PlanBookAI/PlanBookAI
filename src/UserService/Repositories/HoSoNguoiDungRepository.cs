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
            // Mock data cho development
            var mockData = new List<HoSoNguoiDung>
            {
                new HoSoNguoiDung
                {
                    UserId = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
                    NguoiDungId = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
                    HoTen = "Nguyen Van Admin",
                    SoDienThoai = "0123456789",
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = "Ha Noi, Vietnam",
                    AnhDaiDienUrl = "https://example.com/avatar1.jpg",
                    MoTaBanThan = "Admin user for testing",
                    TaoLuc = DateTime.UtcNow.AddDays(-30),
                    CapNhatLuc = DateTime.UtcNow
                },
                new HoSoNguoiDung
                {
                    UserId = Guid.Parse("b2c3d4e5-f6a7-b8c9-d0e1-f2a3b4c5d6e7"),
                    NguoiDungId = Guid.Parse("b2c3d4e5-f6a7-b8c9-d0e1-f2a3b4c5d6e7"),
                    HoTen = "Tran Thi Teacher",
                    SoDienThoai = "0987654321",
                    NgaySinh = new DateTime(1985, 5, 15),
                    DiaChi = "Ho Chi Minh City, Vietnam",
                    AnhDaiDienUrl = "https://example.com/avatar2.jpg",
                    MoTaBanThan = "Chemistry teacher",
                    TaoLuc = DateTime.UtcNow.AddDays(-20),
                    CapNhatLuc = DateTime.UtcNow
                }
            };

            // Nếu có database thì sử dụng database, không thì dùng mock data
            try
            {
                var dbData = await _context.HoSoNguoiDungs.ToListAsync();
                if (dbData.Any())
                {
                    return dbData;
                }
            }
            catch
            {
                // Database chưa sẵn sàng, sử dụng mock data
            }

            return mockData;
        }

        public async Task<HoSoNguoiDung?> GetByIdAsync(string id)
        {
            // Mock data cho development
            var mockData = new List<HoSoNguoiDung>
            {
                new HoSoNguoiDung
                {
                    UserId = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
                    NguoiDungId = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
                    HoTen = "Nguyen Van Admin",
                    SoDienThoai = "0123456789",
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = "Ha Noi, Vietnam",
                    AnhDaiDienUrl = "https://example.com/avatar1.jpg",
                    MoTaBanThan = "Admin user for testing",
                    TaoLuc = DateTime.UtcNow.AddDays(-30),
                    CapNhatLuc = DateTime.UtcNow
                },
                new HoSoNguoiDung
                {
                    UserId = Guid.Parse("b2c3d4e5-f6a7-b8c9-d0e1-f2a3b4c5d6e7"),
                    NguoiDungId = Guid.Parse("b2c3d4e5-f6a7-b8c9-d0e1-f2a3b4c5d6e7"),
                    HoTen = "Tran Thi Teacher",
                    SoDienThoai = "0987654321",
                    NgaySinh = new DateTime(1985, 5, 15),
                    DiaChi = "Ho Chi Minh City, Vietnam",
                    AnhDaiDienUrl = "https://example.com/avatar2.jpg",
                    MoTaBanThan = "Chemistry teacher",
                    TaoLuc = DateTime.UtcNow.AddDays(-20),
                    CapNhatLuc = DateTime.UtcNow
                }
            };

            // Tìm trong mock data trước
            var mockUser = mockData.FirstOrDefault(u => u.UserId.ToString() == id);
            if (mockUser != null)
            {
                return mockUser;
            }

            // Nếu không tìm thấy trong mock data, thử database
            try
            {
                return await _context.HoSoNguoiDungs.FindAsync(id);
            }
            catch
            {
                // Database chưa sẵn sàng
                return null;
            }
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
