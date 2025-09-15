using ClassroomService.Models.Entities;

namespace ClassroomService.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for StudentResults entity operations
    /// </summary>
    public interface IStudentResultsRepository
    {
        /// <summary>
        /// Get student result by ID
        /// </summary>
        Task<StudentResults?> GetByIdAsync(int id);
        
        /// <summary>
        /// Get student results by student ID with pagination
        /// </summary>
        Task<IEnumerable<StudentResults>> GetByStudentIdAsync(int studentId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Get student results by exam ID with pagination
        /// </summary>
        Task<IEnumerable<StudentResults>> GetByExamIdAsync(int examId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Create new student result
        /// </summary>
        Task<StudentResults> CreateAsync(StudentResults entity);
        
        /// <summary>
        /// Update existing student result
        /// </summary>
        Task<StudentResults> UpdateAsync(StudentResults entity);
        
        /// <summary>
        /// Get total count of student results
        /// </summary>
        Task<int> GetTotalCountAsync(int? studentId = null, int? examId = null);
    }
}
