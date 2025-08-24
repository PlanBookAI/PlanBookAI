using ExamService.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface ITaoDeThiService
    {
        Task<ApiPhanHoi<DeThiResponseDTO>> CreateFromBankAsync(TaoDeThiTuNganHangDTO dto, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> CreateRandomAsync(TaoDeThiNgauNhienDTO dto, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> CreateAutomaticAsync(TaoDeThiTuDongDTO dto, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> CreateFromTemplateAsync(TaoDeThiTuMauDTO dto, Guid teacherId);
    }
}