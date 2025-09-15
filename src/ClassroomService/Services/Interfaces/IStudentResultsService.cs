using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing student results
    /// </summary>
    public interface IStudentResultsService
    {
        /// <summary>
        /// Creates a new student result
        /// </summary>
        Task<StudentResultDto> LuuKetQua(CreateStudentResultDto dto);
        
        /// <summary>
        /// Gets student results by student ID
        /// </summary>
        Task<(IEnumerable<StudentResultDto> Items, int TotalCount)> LayKetQuaTheoHocSinh(int studentId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets student results by exam ID
        /// </summary>
        Task<(IEnumerable<StudentResultDto> Items, int TotalCount)> LayKetQuaTheoDeThi(int examId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Updates an existing student result
        /// </summary>
        Task<StudentResultDto> CapNhatKetQua(int id, CreateStudentResultDto dto);
        
        /// <summary>
        /// Gets a student result by ID
        /// </summary>
        Task<StudentResultDto?> LayKetQuaTheoId(int id);
    }
}
