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
        Task<Students?> GetByIdAsync(int id, int? ownerTeacherId = null);
        
        /// <summary>
        /// Get students by class ID with pagination
        /// </summary>
        Task<IEnumerable<Students>> GetByClassIdAsync(int classId, int? ownerTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Get students by owner teacher ID
        /// </summary>
        Task<IEnumerable<Students>> GetByOwnerTeacherIdAsync(int ownerTeacherId, int page = 1, int pageSize = 10);
        
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
        Task<bool> DeleteAsync(int id, int ownerTeacherId);
        
        /// <summary>
        /// Check if student code exists
        /// </summary>
        Task<bool> ExistsByStudentCodeAsync(string studentCode, int? excludeId = null);
        
        /// <summary>
        /// Get total count of students
        /// </summary>
        Task<int> GetTotalCountAsync(int? classId = null, int? ownerTeacherId = null);
    }
}