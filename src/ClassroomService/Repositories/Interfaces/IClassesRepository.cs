using ClassroomService.Models.Entities;

namespace ClassroomService.Repositories.Interfaces
{
    public interface IClassesRepository
    {
        Task<Classes> GetByIdAsync(int id, int? homeroomTeacherId = null);
        Task<IEnumerable<Classes>> GetAllAsync(int? homeroomTeacherId = null, int page = 1, int pageSize = 10);
        Task<IEnumerable<Classes>> GetByHomeroomTeacherIdAsync(int homeroomTeacherId, int page = 1, int pageSize = 10);
        Task<Classes> CreateAsync(Classes entity);
        Task<Classes> UpdateAsync(Classes entity);
        Task<bool> DeleteAsync(int id, int homeroomTeacherId);
        Task<int> GetTotalCountAsync(int? homeroomTeacherId = null);
    }
}