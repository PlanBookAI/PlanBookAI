using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models.Entities;

namespace AuthService.Repositories;

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
            .Include(n => n.VaiTro)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<NguoiDung?> GetByEmailAsync(string email)
    {
        return await _context.NguoiDungs
            .Include(n => n.VaiTro)
            .FirstOrDefaultAsync(n => n.Email == email);
    }

    public async Task<IEnumerable<NguoiDung>> GetAllAsync()
    {
        return await _context.NguoiDungs
            .Include(n => n.VaiTro)
            .Where(n => n.HoatDong)
            .ToListAsync();
    }

    public async Task<NguoiDung> CreateAsync(NguoiDung nguoiDung)
    {
        _context.NguoiDungs.Add(nguoiDung);
        await _context.SaveChangesAsync();
        return nguoiDung;
    }

    public async Task<NguoiDung> UpdateAsync(NguoiDung nguoiDung)
    {
        _context.NguoiDungs.Update(nguoiDung);
        await _context.SaveChangesAsync();
        return nguoiDung;
    }

    public async Task DeleteAsync(Guid id)
    {
        var nguoiDung = await GetByIdAsync(id);
        if (nguoiDung != null)
        {
            nguoiDung.HoatDong = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.NguoiDungs
            .AnyAsync(n => n.Email == email);
    }

    public async Task UpdateLastLoginAsync(Guid id)
    {
        var nguoiDung = await GetByIdAsync(id);
        if (nguoiDung != null)
        {
            nguoiDung.LanDangNhapCuoi = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
