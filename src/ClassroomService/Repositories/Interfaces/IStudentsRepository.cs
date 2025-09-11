using ClassroomService.Models.Entities;

namespace ClassroomService.Repositories.Interfaces
{
    public interface IStudentsRepository
    {
        Task<Students> GetByIdAsync(int id, int? ownerTeacherId = null);
        Task<IEnumerable<Students>> GetByClassIdAsync(int classId, int? ownerTeacherId = null, int page = 1, int pageSize = 10);
        Task<IEnumerable<Students>> GetByOwnerTeacherIdAsync(int ownerTeacherId, int page = 1, int pageSize = 10);
        Task<Students> CreateAsync(Students entity);
        Task<Students> UpdateAsync(Students entity);
        Task<bool> DeleteAsync(int id, int ownerTeacherId);
        Task<bool> ExistsByStudentCodeAsync(string studentCode, int? excludeId = null);
        Task<int> GetTotalCountAsync(int? classId = null, int? ownerTeacherId = null);
    }
}