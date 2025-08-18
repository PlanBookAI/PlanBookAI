using UserService.Models.Entities;

namespace UserService.Repositories;

public interface IVaiTroRepository
{
    Task<VaiTro?> GetByIdAsync(int id);
    Task<List<VaiTro>> GetAllAsync();
    Task<VaiTro?> GetByNameAsync(string name);
}
