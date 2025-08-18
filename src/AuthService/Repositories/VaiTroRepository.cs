using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models.Entities;

namespace AuthService.Repositories;

public class VaiTroRepository : IVaiTroRepository
{
    private readonly AuthDbContext _context;

    public VaiTroRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<VaiTro?> GetByIdAsync(int id)
    {
        return await _context.VaiTros
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<VaiTro?> GetByNameAsync(string ten)
    {
        return await _context.VaiTros
            .FirstOrDefaultAsync(v => v.Ten == ten);
    }

    public async Task<IEnumerable<VaiTro>> GetAllAsync()
    {
        return await _context.VaiTros
            .Where(v => v.HoatDong)
            .ToListAsync();
    }

    public async Task<VaiTro> CreateAsync(VaiTro vaiTro)
    {
        _context.VaiTros.Add(vaiTro);
        await _context.SaveChangesAsync();
        return vaiTro;
    }

    public async Task<VaiTro> UpdateAsync(VaiTro vaiTro)
    {
        _context.VaiTros.Update(vaiTro);
        await _context.SaveChangesAsync();
        return vaiTro;
    }

    public async Task DeleteAsync(int id)
    {
        var vaiTro = await GetByIdAsync(id);
        if (vaiTro != null)
        {
            vaiTro.HoatDong = false;
            await _context.SaveChangesAsync();
        }
    }
}
