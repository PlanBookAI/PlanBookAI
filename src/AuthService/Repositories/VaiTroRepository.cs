using AuthService.Data;
using AuthService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthService.Repositories
{
    /// <summary>
    /// Triển khai IVaiTroRepository.
    /// </summary>
    public class VaiTroRepository : IVaiTroRepository
    {
        private readonly AuthDbContext _context;

        public VaiTroRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<VaiTro> GetByTenVaiTroAsync(string tenVaiTro)
        {
            return await _context.VaiTros.FirstOrDefaultAsync(v => v.Ten == tenVaiTro);
        }
    }
}
