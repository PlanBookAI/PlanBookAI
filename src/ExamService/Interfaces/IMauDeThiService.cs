using ExamService.Models.DTOs;
namespace ExamService.Interfaces
{
    public interface IMauDeThiService
    {
        // Legacy methods (giữ để backward compatibility)
        Task<ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO paging);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> CreateAsync(MauDeThiRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> UpdateAsync(Guid id, MauDeThiRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<MauDeThiResponseDTO>> CloneAsync(Guid id, Guid teacherId);

        // New methods for controller
        Task<PagedResult<MauDeThiResponseDTO>> LayDanhSachMauDeThiAsync(
            Guid teacherId, int pageNumber, int pageSize, string? monHoc = null, 
            int? khoiLop = null, string? trangThai = null);
        Task<MauDeThiResponseDTO?> LayChiTietMauDeThiAsync(Guid id, Guid teacherId);
        Task<MauDeThiResponseDTO> TaoMauDeThiAsync(MauDeThiRequestDTO request, Guid teacherId);
        Task<MauDeThiResponseDTO> CapNhatMauDeThiAsync(Guid id, MauDeThiRequestDTO request, Guid teacherId);
        Task<bool> XoaMauDeThiAsync(Guid id, Guid teacherId);
        Task<MauDeThiResponseDTO> SaoChepMauDeThiAsync(Guid id, SaoChepMauDeThiDTO request, Guid teacherId);
    }
}