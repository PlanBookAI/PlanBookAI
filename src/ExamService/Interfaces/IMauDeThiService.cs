using ExamService.Models.DTOs;
namespace ExamService.Interfaces
{
    public interface IMauDeThiService
    {
        Task<ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO paging);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> CreateAsync(MauDeThiRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> UpdateAsync(Guid id, MauDeThiRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> CloneAsync(Guid id, Guid teacherId);
    }
}