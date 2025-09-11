using PlanService.Models.DTOs;
using PlanService.Models.Entities;

namespace PlanService.Services
{
    /// <summary>
    /// Interface định nghĩa các phương thức business logic cho Giáo Án
    /// Theo yêu cầu ban đầu: Service layer for business logic
    /// </summary>
    public interface IGiaoAnService
    {
        // CRUD Operations
        Task<ApiPhanHoi<IEnumerable<GiaoAn>>> GetAllAsync(Guid teacherId);
        Task<ApiPhanHoi<ThongTinGiaoAn?>> GetByIdAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<GiaoAn>> CreateAsync(YeuCauTaoGiaoAn request, Guid teacherId);
        Task<ApiPhanHoi<GiaoAn>> UpdateAsync(Guid id, YeuCauTaoGiaoAn request, Guid teacherId);
        Task<ApiPhanHoi<GiaoAn>> DeleteAsync(Guid id, Guid teacherId);

        // Status Management 
        Task<ApiPhanHoi<GiaoAn>> PheDuyetAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<GiaoAn>> XuatBanAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<GiaoAn>> LuuTruAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<GiaoAn>> CopyAsync(Guid id, Guid teacherId);

        // Search & Filter 
        Task<ApiPhanHoi<IEnumerable<GiaoAn>>> TimKiemAsync(string keyword, Guid teacherId);
        Task<ApiPhanHoi<IEnumerable<GiaoAn>>> LocTheoChuDeAsync(Guid chuDeId, Guid teacherId);
        Task<ApiPhanHoi<IEnumerable<GiaoAn>>> LocTheoMonHocAsync(string monHoc, Guid teacherId);
        Task<ApiPhanHoi<IEnumerable<GiaoAn>>> LocTheoKhoiAsync(int khoi, Guid teacherId);

        // Export 
        Task<ApiPhanHoi<byte[]>> XuatPDFAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<byte[]>> XuatWordAsync(Guid id, Guid teacherId);

        // Create from Template (theo yêu cầu: Tạo giáo án từ mẫu có sẵn)
        Task<ApiPhanHoi<GiaoAn>> TaoTuMauAsync(Guid templateId, YeuCauTaoGiaoAn request, Guid teacherId);
    }
}