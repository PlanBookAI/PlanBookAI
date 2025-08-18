using UserService.Models.Entities;

namespace UserService.Repositories;

public interface INguoiDungRepository
{
    Task<NguoiDung?> GetByIdAsync(Guid id);
    Task<NguoiDung?> GetByEmailAsync(string email);
    Task<List<NguoiDung>> GetAllAsync();
    Task<List<NguoiDung>> GetAllExceptAsync(Guid excludeId);
    Task<NguoiDung> CreateAsync(NguoiDung nguoiDung);
    Task<NguoiDung> UpdateAsync(NguoiDung nguoiDung);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SoftDeleteAsync(Guid id);
    Task<bool> RestoreAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}
