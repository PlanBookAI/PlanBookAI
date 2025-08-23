using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace ExamService.Interfaces;

public interface ICauHoiService
{
    Task<ApiPhanHoi<PagedResult<CauHoiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO pagingParams);
    Task<ApiPhanHoi<CauHoiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId);
    Task<ApiPhanHoi<CauHoiResponseDTO>> CreateAsync(CauHoiRequestDTO dto, Guid teacherId);
    Task<ApiPhanHoi<CauHoiResponseDTO>> UpdateAsync(Guid id, CauHoiRequestDTO dto, Guid teacherId);
    Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId);
    Task<ApiPhanHoi<PagedResult<CauHoiResponseDTO>>> SearchAsync(Guid teacherId, CauHoiSearchParametersDTO searchParams);
    Task<ApiPhanHoi<ImportResultDTO>> ImportFromExcelAsync(IFormFile file, Guid teacherId);
    Task<byte[]> ExportToExcelAsync(Guid teacherId);
}