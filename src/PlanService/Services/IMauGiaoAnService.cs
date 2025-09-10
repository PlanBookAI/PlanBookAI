using PlanService.Models.DTOs;
using PlanService.Models.Entities;

namespace PlanService.Services
{
    /// <summary>
    /// Interface định nghĩa business logic cho quản lý mẫu giáo án (lesson_templates).
    /// Cung cấp các phương thức tương tác với entity MauGiaoAn theo Repository pattern.
    /// </summary>
    public interface IMauGiaoAnService
    {
        Task<ApiPhanHoi<IEnumerable<MauGiaoAn>>> GetAllAsync(Guid teacherId);
        Task<ApiPhanHoi<MauGiaoAn?>> GetByIdAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<MauGiaoAn>> CreateAsync(MauGiaoAn request, Guid teacherId);
        Task<ApiPhanHoi<MauGiaoAn>> UpdateAsync(Guid id, MauGiaoAn request, Guid teacherId);
        Task<ApiPhanHoi<MauGiaoAn>> DeleteAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<MauGiaoAn>> ChiaSeAsync(Guid id, bool chiaSe, Guid teacherId);
    }
}