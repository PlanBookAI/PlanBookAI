using AuthService.Models.Entities;

namespace AuthService.Repositories;

public interface INguoiDungRepository
{
    Task<NguoiDung?> GetByIdAsync(Guid id);
    Task<NguoiDung?> GetByEmailAsync(string email);
    Task<IEnumerable<NguoiDung>> GetAllAsync();
    Task<NguoiDung> CreateAsync(NguoiDung nguoiDung);
    Task<NguoiDung> UpdateAsync(NguoiDung nguoiDung);
    Task DeleteAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateLastLoginAsync(Guid id);
}
