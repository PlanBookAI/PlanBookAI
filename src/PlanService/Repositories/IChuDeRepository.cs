using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanService.Models.Entities;

namespace PlanService.Repositories
{
    // Định nghĩa các phương thức cơ bản cho việc truy cập dữ liệu của chủ đề.
    public interface IChuDeRepository
    {
        Task<IEnumerable<ChuDe>> GetAllAsync();
        Task<ChuDe?> GetByIdAsync(Guid id);
        Task<IEnumerable<ChuDe>> GetByMonHocAsync(string monHoc);
        Task<IEnumerable<ChuDe>> GetByParentIdAsync(Guid? parentId);
        Task AddAsync(ChuDe chuDe);
        Task UpdateAsync(ChuDe chuDe);
        Task DeleteAsync(Guid id);
    }
}
