using ClassroomService.Models.Entities;

namespace ClassroomService.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Classes entity operations
    /// </summary>
    public interface IClassesRepository
    {
        /// <summary>
        /// Get class by ID with optional homeroom teacher filter
        /// </summary>
        Task<Classes?> GetByIdAsync(Guid id, Guid? homeroomTeacherId = null);
        
        /// <summary>
        /// Get all classes with pagination
        /// </summary>
        Task<IEnumerable<Classes>> GetAllAsync(Guid? homeroomTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Get classes by homeroom teacher ID
        /// </summary>
        Task<IEnumerable<Classes>> GetByHomeroomTeacherIdAsync(Guid homeroomTeacherId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Create new class
        /// </summary>
        Task<Classes> CreateAsync(Classes entity);
        
        /// <summary>
        /// Update existing class
        /// </summary>
        Task<Classes> UpdateAsync(Classes entity);
        
        /// <summary>
        /// Delete class by ID
        /// </summary>
        Task<bool> DeleteAsync(Guid id, Guid homeroomTeacherId);
        
        /// <summary>
        /// Get total count of classes
        /// </summary>
        Task<int> GetTotalCountAsync(Guid? homeroomTeacherId = null);
    }
}