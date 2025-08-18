using UserService.Models.Entities;

namespace UserService.Repositories;

public interface ILichSuDangNhapRepository
{
    Task<LichSuDangNhap?> GetByIdAsync(Guid id);
    Task<List<LichSuDangNhap>> GetByUserIdAsync(Guid userId);
    Task<LichSuDangNhap> CreateAsync(LichSuDangNhap lichSuDangNhap);
    Task DeleteAsync(Guid id);
    Task DeleteByUserIdAsync(Guid userId);
    Task<int> CountByUserIdAsync(Guid userId);
}
