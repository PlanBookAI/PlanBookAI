#if false
using PlanService.Models.DTOs;

namespace PlanService.Services.Export
{
    /// <summary>
    /// Interface xuất giáo án ra PDF với định dạng chuẩn (A4, header/footer, số trang, bookmarks).
    /// Chỉ chịu trách nhiệm export (SRP), không chứa nghiệp vụ giáo án.
    /// </summary>
    public interface IXuatGiaoAnPdfService
    {
        Task<ApiPhanHoi<byte[]>> XuatPdfAsync(Guid giaoAnId, Guid teacherId);
    }
}
#endif