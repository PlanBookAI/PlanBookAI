using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    public interface IDeThiRepository
    {
        Task<List<DeThi>> GetAllAsync();
        Task<DeThi?> GetByIdAsync(string id);
        Task<DeThi> CreateAsync(DeThi deThi);
        Task<DeThi> UpdateAsync(DeThi deThi);
        Task DeleteAsync(string id);
    }
}