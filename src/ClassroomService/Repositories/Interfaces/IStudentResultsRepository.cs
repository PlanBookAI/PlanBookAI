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
        Task<StudentResults?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Get student results by student ID with pagination
        /// </summary>
        Task<IEnumerable<StudentResults>> GetByStudentIdAsync(Guid studentId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Get student results by exam ID with pagination
        /// </summary>
        Task<IEnumerable<StudentResults>> GetByExamIdAsync(Guid examId, int page = 1, int pageSize = 10);
        
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
        Task<int> GetTotalCountAsync(Guid? studentId = null, Guid? examId = null);
    }
}
