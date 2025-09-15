using ClassroomService.Models.Entities;

namespace ClassroomService.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Students entity operations
    /// </summary>
    public interface IStudentsRepository
    {
        /// <summary>
        /// Get student by ID with optional owner teacher filter
        /// </summary>
        Task<Students?> GetByIdAsync(Guid id, Guid? ownerTeacherId = null);
        
        /// <summary>
        /// Get students by class ID with pagination
        /// </summary>
        Task<IEnumerable<Students>> GetByClassIdAsync(Guid classId, Guid? ownerTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Get students by owner teacher ID
        /// </summary>
        Task<IEnumerable<Students>> GetByOwnerTeacherIdAsync(Guid ownerTeacherId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Create new student
        /// </summary>
        Task<Students> CreateAsync(Students entity);
        
        /// <summary>
        /// Update existing student
        /// </summary>
        Task<Students> UpdateAsync(Students entity);
        
        /// <summary>
        /// Delete student by ID
        /// </summary>
        Task<bool> DeleteAsync(Guid id, Guid ownerTeacherId);
        
        /// <summary>
        /// Check if student code exists
        /// </summary>
        Task<bool> ExistsByStudentCodeAsync(string studentCode, Guid? excludeId = null);
        
        /// <summary>
        /// Get total count of students
        /// </summary>
        Task<int> GetTotalCountAsync(Guid? classId = null, Guid? ownerTeacherId = null);
    }
}