using ExamService.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface IDeThiCauHoiService
    {
        Task<ApiPhanHoi<List<DeThiCauHoiResponseDTO>>> GetQuestionsByExamIdAsync(Guid deThiId, Guid teacherId);
        Task<ApiPhanHoi<DeThiCauHoiResponseDTO>> UpdateOrderAsync(Guid examQuestionId, int newOrder, Guid teacherId);
        Task<ApiPhanHoi<DeThiCauHoiResponseDTO>> UpdatePointsAsync(Guid examQuestionId, decimal newPoints, Guid teacherId);
    }
}