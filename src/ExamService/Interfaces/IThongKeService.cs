using ExamService.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface IThongKeService
    {
        /// <summary>
        /// Lấy báo cáo thống kê tổng hợp cho một giáo viên.
        /// </summary>
        Task<ApiPhanHoi<BaoCaoThongKeGiaoVienDTO>> GetTeacherReportAsync(Guid teacherId);

        /// <summary>
        /// Xuất báo cáo thống kê của một giáo viên ra file Excel.
        /// </summary>
        Task<ApiPhanHoi<byte[]>> ExportTeacherReportAsync(Guid teacherId);

        /// <summary>
        /// Lấy thống kê câu hỏi.
        /// </summary>
        Task<ApiPhanHoi<object>> GetQuestionStatsAsync(Guid teacherId);

        /// <summary>
        /// Lấy thống kê đề thi.
        /// </summary>
        Task<ApiPhanHoi<object>> GetExamStatsAsync(Guid teacherId);

        /// <summary>
        /// Lấy thống kê theo độ khó.
        /// </summary>
        Task<ApiPhanHoi<object>> GetStatsByDifficultyAsync(Guid teacherId);

        /// <summary>
        /// Lấy thống kê theo chủ đề.
        /// </summary>
        Task<ApiPhanHoi<object>> GetStatsByTopicAsync(Guid teacherId);

        /// <summary>
        /// Lấy thống kê theo môn học.
        /// </summary>
        Task<ApiPhanHoi<object>> GetStatsBySubjectAsync(Guid teacherId);
    }
}