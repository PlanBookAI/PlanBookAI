using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models.Entities;
using Npgsql;

namespace UserService.Repositories;

public class LichSuDangNhapRepository : ILichSuDangNhapRepository
{
    private readonly UserDbContext _context;

    public LichSuDangNhapRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<LichSuDangNhap?> GetByIdAsync(Guid id)
    {
        // Sử dụng raw SQL để bypass EF Core issues
        var sql = "SELECT id, user_id, token, expires_at, created_at FROM auth.sessions WHERE id = @Id";
        var parameters = new[] { new NpgsqlParameter("@Id", id) };
        
        var result = await _context.Database.SqlQueryRaw<LichSuDangNhap>(sql, parameters).FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<LichSuDangNhap>> GetByUserIdAsync(Guid userId)
    {
        // Sử dụng raw SQL để bypass EF Core issues
        var sql = "SELECT id, user_id, token, expires_at, created_at FROM auth.sessions WHERE user_id = @UserId ORDER BY created_at DESC";
        var parameters = new[] { new NpgsqlParameter("@UserId", userId) };
        
        var result = await _context.Database.SqlQueryRaw<LichSuDangNhap>(sql, parameters).ToListAsync();
        return result;
    }

    public async Task<LichSuDangNhap> CreateAsync(LichSuDangNhap lichSuDangNhap)
    {
        // Sử dụng raw SQL để bypass EF Core issues
        var insertSql = @"
			INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at)
			VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt)";

        var parameters = new[]
        {
            new NpgsqlParameter("@Id", lichSuDangNhap.Id),
            new NpgsqlParameter("@UserId", lichSuDangNhap.UserId),
            new NpgsqlParameter("@Token", lichSuDangNhap.Token),
            new NpgsqlParameter("@ExpiresAt", lichSuDangNhap.ExpiresAt),
            new NpgsqlParameter("@CreatedAt", lichSuDangNhap.CreatedAt)
        };

        await _context.Database.ExecuteSqlRawAsync(insertSql, parameters);
        return lichSuDangNhap;
    }

    public async Task DeleteAsync(Guid id)
    {
        // Sử dụng raw SQL để bypass EF Core issues
        var sql = "DELETE FROM auth.sessions WHERE id = @Id";
        var parameters = new[] { new NpgsqlParameter("@Id", id) };
        
        await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        // Sử dụng raw SQL để bypass EF Core issues
        var sql = "DELETE FROM auth.sessions WHERE user_id = @UserId";
        var parameters = new[] { new NpgsqlParameter("@UserId", userId) };
        
        await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        // Sử dụng raw SQL để bypass EF Core issues
        var sql = "SELECT COUNT(*) FROM auth.sessions WHERE user_id = @UserId";
        var parameters = new[] { new NpgsqlParameter("@UserId", userId) };
        
        var result = await _context.Database.SqlQueryRaw<int>(sql, parameters).FirstOrDefaultAsync();
        return result;
    }
}
