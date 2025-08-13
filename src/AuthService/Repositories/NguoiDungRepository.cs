using AuthService.Data;
using AuthService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthService.Repositories
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

        public async Task<NguoiDung?> GetByIdAsync(Guid id)
        {
            return await _context.NguoiDungs
                .Include(nd => nd.VaiTro)
                .FirstOrDefaultAsync(nd => nd.Id == id);
        }

        public async Task<NguoiDung?> GetByEmailAsync(string email)
        {
            return await _context.NguoiDungs
                .Include(nd => nd.VaiTro)
                .FirstOrDefaultAsync(nd => nd.Email == email);
        }

        public async Task AddAsync(NguoiDung nguoiDung)
        {
            await _context.NguoiDungs.AddAsync(nguoiDung);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDungs.Update(nguoiDung);
            await _context.SaveChangesAsync();
        }
    }
}