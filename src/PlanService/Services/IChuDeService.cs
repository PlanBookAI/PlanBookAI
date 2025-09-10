using PlanService.Models.DTOs;
using PlanService.Models.Entities;

namespace PlanService.Services
{
    /// <summary>
    /// Interface định nghĩa business logic cho quản lý chủ đề giáo dục.
    /// Cung cấp các phương thức để tương tác với ChuDe entity thông qua Repository pattern.
    /// </summary>
    public interface IChuDeService
    {
        Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetAllAsync();
        Task<ApiPhanHoi<ChuDe?>> GetByIdAsync(Guid id);
        Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetByMonHocAsync(string monHoc);
        Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetByParentIdAsync(Guid? parentId);
        Task<ApiPhanHoi<ChuDe>> AddAsync(ChuDe chuDe);
        Task<ApiPhanHoi<ChuDe>> UpdateAsync(ChuDe chuDe);
        Task<ApiPhanHoi<ChuDe>> DeleteAsync(Guid id);
        Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetHierarchicalTreeAsync();
    }
}
