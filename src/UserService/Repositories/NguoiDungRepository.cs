using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models.Entities;
using Npgsql;

namespace UserService.Repositories;

public class NguoiDungRepository : INguoiDungRepository
{
    private readonly UserDbContext _context;

    public NguoiDungRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<NguoiDung?> GetByIdAsync(Guid id)
    {
        return await _context.NguoiDung
            .Include(n => n.VaiTro)
            .Include(n => n.HoSoNguoiDung)
            .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);
    }

    public async Task<NguoiDung?> GetByEmailAsync(string email)
    {
        return await _context.NguoiDung
            .Include(n => n.VaiTro)
            .Include(n => n.HoSoNguoiDung)
            .FirstOrDefaultAsync(n => n.Email == email && !n.IsDeleted);
    }

    public async Task<List<NguoiDung>> GetAllAsync()
    {
        return await _context.NguoiDung
            .Include(n => n.VaiTro)
            .Include(n => n.HoSoNguoiDung)
            .Where(n => !n.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<NguoiDung>> GetAllExceptAsync(Guid excludeId)
    {
        return await _context.NguoiDung
            .Include(n => n.VaiTro)
            .Include(n => n.HoSoNguoiDung)
            .Where(n => n.Id != excludeId && !n.IsDeleted)
            .ToListAsync();
    }

    public async Task<NguoiDung> CreateAsync(NguoiDung nguoiDung)
    {
        _context.NguoiDung.Add(nguoiDung);
        await _context.SaveChangesAsync();
        return nguoiDung;
    }

    public async Task<NguoiDung> UpdateAsync(NguoiDung nguoiDung)
    {
        // Sử dụng raw SQL để tránh DateTime Kind issue
        if (nguoiDung.HoSoNguoiDung != null)
        {
            var existingProfile = await _context.HoSoNguoiDung
                .FirstOrDefaultAsync(p => p.UserId == nguoiDung.Id);

            if (existingProfile == null)
            {
                // Tạo mới profile bằng raw SQL
                var insertSql = @"
					INSERT INTO users.user_profiles (id, user_id, full_name, phone, address, bio, avatar_url, created_at, updated_at)
					VALUES (@Id, @UserId, @HoTen, @SoDienThoai, @DiaChi, @MoTaBanThan, @AnhDaiDienUrl, @CreatedAt, @UpdatedAt)";

                var parameters = new[]
                {
                    new NpgsqlParameter("@Id", Guid.NewGuid()),
                    new NpgsqlParameter("@UserId", nguoiDung.Id),
                    new NpgsqlParameter("@HoTen", nguoiDung.HoSoNguoiDung.HoTen),
                    new NpgsqlParameter("@SoDienThoai", (object?)nguoiDung.HoSoNguoiDung.SoDienThoai ?? DBNull.Value),
                    new NpgsqlParameter("@DiaChi", (object?)nguoiDung.HoSoNguoiDung.DiaChi ?? DBNull.Value),
                    new NpgsqlParameter("@MoTaBanThan", (object?)nguoiDung.HoSoNguoiDung.MoTaBanThan ?? DBNull.Value),
                    new NpgsqlParameter("@AnhDaiDienUrl", (object?)nguoiDung.HoSoNguoiDung.AnhDaiDienUrl ?? DBNull.Value),
                    new NpgsqlParameter("@CreatedAt", DateTime.UtcNow),
                    new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow)
                };

                await _context.Database.ExecuteSqlRawAsync(insertSql, parameters);
            }
            else
            {
                // Cập nhật profile hiện có bằng raw SQL
                var updateSql = @"
					UPDATE users.user_profiles 
					SET full_name = @HoTen, phone = @SoDienThoai, address = @DiaChi, 
						bio = @MoTaBanThan, avatar_url = @AnhDaiDienUrl, updated_at = @UpdatedAt
					WHERE user_id = @UserId";

                var parameters = new[]
                {
                    new NpgsqlParameter("@HoTen", nguoiDung.HoSoNguoiDung.HoTen),
                    new NpgsqlParameter("@SoDienThoai", (object?)nguoiDung.HoSoNguoiDung.SoDienThoai ?? DBNull.Value),
                    new NpgsqlParameter("@DiaChi", (object?)nguoiDung.HoSoNguoiDung.DiaChi ?? DBNull.Value),
                    new NpgsqlParameter("@MoTaBanThan", (object?)nguoiDung.HoSoNguoiDung.MoTaBanThan ?? DBNull.Value),
                    new NpgsqlParameter("@AnhDaiDienUrl", (object?)nguoiDung.HoSoNguoiDung.AnhDaiDienUrl ?? DBNull.Value),
                    new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow),
                    new NpgsqlParameter("@UserId", nguoiDung.Id)
                };

                await _context.Database.ExecuteSqlRawAsync(updateSql, parameters);
            }
        }

        // Cập nhật UpdatedAt của user
        var updateUserSql = "UPDATE auth.users SET updated_at = @UpdatedAt WHERE id = @UserId";
        var userParameters = new[]
        {
            new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow),
            new NpgsqlParameter("@UserId", nguoiDung.Id)
        };

        await _context.Database.ExecuteSqlRawAsync(updateUserSql, userParameters);

        // Trả về entity đã cập nhật
        return await GetByIdAsync(nguoiDung.Id) ?? nguoiDung;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var nguoiDung = await _context.NguoiDung.FindAsync(id);
        if (nguoiDung == null) return false;

        _context.NguoiDung.Remove(nguoiDung);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var nguoiDung = await _context.NguoiDung.FindAsync(id);
        if (nguoiDung == null) return false;

        nguoiDung.IsDeleted = true;
        nguoiDung.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        nguoiDung.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        var nguoiDung = await _context.NguoiDung.FindAsync(id);
        if (nguoiDung == null) return false;

        nguoiDung.IsDeleted = false;
        nguoiDung.DeletedAt = null;
        nguoiDung.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.NguoiDung.AnyAsync(n => n.Id == id && !n.IsDeleted);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.NguoiDung.AnyAsync(n => n.Email == email && !n.IsDeleted);
    }

    // BỔ SUNG: Các phương thức cho tính năng Reset Password

    public async Task<OtpCode> SaveOtpCodeAsync(OtpCode otpCode)
    {
        _context.OtpCodes.Add(otpCode);
        await _context.SaveChangesAsync();
        return otpCode;
    }

    public async Task<OtpCode?> GetValidOtpByUserIdAndHashAsync(Guid userId, string otpHash)
    {
        return await _context.OtpCodes
            .Where(o => o.UserId == userId && o.OtpHash == otpHash && o.IsUsed == false && o.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task InvalidateOtpAsync(Guid otpId)
    {
        var otpCode = await _context.OtpCodes.FindAsync(otpId);
        if (otpCode != null)
        {
            otpCode.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task SavePasswordHistoryAsync(PasswordHistory history)
    {
        _context.PasswordHistory.Add(history);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PasswordHistory>> GetPasswordHistoryByUserIdAsync(Guid userId)
    {
        return await _context.PasswordHistory
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}
