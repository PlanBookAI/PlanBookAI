using AuthService.Data;
using AuthService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthService.Repositories.NguoiDungRepository
{
    /// <summary>
    /// Triển khai INguoiDungRepository bằng Entity Framework Core.
    /// </summary>
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly AuthDbContext _context;

        public NguoiDungRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<NguoiDung> GetByTenDangNhapAsync(string tenDangNhap)
        {
            // Tải kèm thông tin VaiTro.
            return await _context.NguoiDungs.Include(nd => nd.VaiTro).FirstOrDefaultAsync(nd => nd.TenDangNhap == tenDangNhap);
        }

        public async Task<NguoiDung> GetByRefreshTokenAsync(string refreshToken)
        {
            // Tải kèm thông tin VaiTro.
            return await _context.NguoiDungs.Include(nd => nd.VaiTro).FirstOrDefaultAsync(nd => nd.RefreshToken == refreshToken);
        }

        public async Task AddAsync(NguoiDung nguoiDung)
        {
            await _context.NguoiDungs.AddAsync(nguoiDung);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào database.
        }

        public async Task UpdateAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDungs.Update(nguoiDung);
            await _context.SaveChangesAsync();
        }
    }
}