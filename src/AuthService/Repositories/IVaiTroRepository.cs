using AuthService.Models.Entities;

namespace AuthService.Repositories;

public interface IVaiTroRepository
{
    Task<VaiTro?> GetByIdAsync(int id);
    Task<VaiTro?> GetByNameAsync(string ten);
    Task<IEnumerable<VaiTro>> GetAllAsync();
    Task<VaiTro> CreateAsync(VaiTro vaiTro);
    Task<VaiTro> UpdateAsync(VaiTro vaiTro);
    Task DeleteAsync(int id);
}
