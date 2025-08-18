using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models.Entities;

namespace UserService.Repositories;

public class VaiTroRepository : IVaiTroRepository
{
    private readonly UserDbContext _context;

    public VaiTroRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<VaiTro?> GetByIdAsync(int id)
    {
        return await _context.VaiTro.FindAsync(id);
    }

    public async Task<List<VaiTro>> GetAllAsync()
    {
        return await _context.VaiTro.ToListAsync();
    }

    public async Task<VaiTro?> GetByNameAsync(string name)
    {
        return await _context.VaiTro.FirstOrDefaultAsync(v => v.Ten == name);
    }
}
