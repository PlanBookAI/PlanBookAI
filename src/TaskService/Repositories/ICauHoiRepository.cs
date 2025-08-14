using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    public interface ICauHoiRepository
    {
        Task<List<CauHoi>> GetAllAsync();
        Task<CauHoi?> GetByIdAsync(string id);
        Task<List<CauHoi>> GetByMonHocAndDoKhoAsync(string monHoc, string doKho);
        Task<CauHoi> CreateAsync(CauHoi cauHoi);
        Task<CauHoi> UpdateAsync(CauHoi cauHoi);
        Task DeleteAsync(string id);
    }
}