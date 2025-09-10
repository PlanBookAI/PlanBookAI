#if false
using PlanService.Models.DTOs;

namespace PlanService.Services.Export
{
    /// <summary>
    /// Interface xuất giáo án ra Word (.docx) theo định dạng chuẩn.
    /// Chỉ chịu trách nhiệm export (SRP), không chứa nghiệp vụ giáo án.
    /// </summary>
    public interface IXuatGiaoAnWordService
    {
        Task<ApiPhanHoi<byte[]>> XuatWordAsync(Guid giaoAnId, Guid teacherId);
    }
}
#endif