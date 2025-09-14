using ExamService.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface IDeThiService
    {
        Task<ApiPhanHoi<PagedResult<DeThiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO pagingParams);
        Task<ApiPhanHoi<DeThiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> CreateAsync(DeThiRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> UpdateAsync(Guid id, DeThiRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> AddQuestionToExamAsync(Guid deThiId, ThemCauHoiVaoDeThiDTO dto, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> RemoveQuestionFromExamAsync(Guid deThiId, Guid cauHoiId, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> UpdateQuestionInExamAsync(Guid deThiId, Guid cauHoiId, CapNhatCauHoiTrongDeThiDTO dto, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> PublishExamAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> UnpublishExamAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<byte[]>> ExportToPdfAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<byte[]>> ExportToWordAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<DeThiResponseDTO>> CloneExamAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<DeThiThongKeDTO>> GetExamStatisticsAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<PagedResult<DeThiResponseDTO>>> SearchExamsAsync(Guid teacherId, string keyword, PagingDTO pagingParams);
        Task<ApiPhanHoi<PagedResult<DeThiResponseDTO>>> FilterBySubjectAsync(Guid teacherId, string subject, PagingDTO pagingParams);
        Task<ApiPhanHoi<PagedResult<DeThiResponseDTO>>> FilterByGradeAsync(Guid teacherId, int grade, PagingDTO pagingParams);
        Task<ApiPhanHoi<PagedResult<DeThiResponseDTO>>> FilterByStatusAsync(Guid teacherId, string status, PagingDTO pagingParams);
    }
}