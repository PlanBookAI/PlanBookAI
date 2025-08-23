using ExamService.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface IThongKeService
    {
        Task<ApiPhanHoi<ThongKeTongQuanDTO>> GetGeneralStatisticsAsync(Guid teacherId);
        Task<ApiPhanHoi<CauHoiThongKeTongQuanDTO>> GetQuestionStatisticsAsync(Guid teacherId);
        Task<ApiPhanHoi<DeThiThongKeTongQuanDTO>> GetExamStatisticsOverviewAsync(Guid teacherId);
        Task<ApiPhanHoi<List<ThongKeTheoNhomDTO>>> GetStatsByDifficultyAsync(Guid teacherId);
        Task<ApiPhanHoi<List<ThongKeTheoNhomDTO>>> GetStatsByTopicAsync(Guid teacherId);
        Task<ApiPhanHoi<List<ThongKeTheoNhomDTO>>> GetStatsBySubjectAsync(Guid teacherId);
        Task<ApiPhanHoi<NguoiDungThongKeDTO>> GetUserStatisticsAsync(Guid teacherId);
    }
}