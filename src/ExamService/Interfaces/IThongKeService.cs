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
        Task<byte[]> ExportTeacherReportToExcelAsync(Guid teacherId);
    }
}