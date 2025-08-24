using ExamService.Models.Entities;

namespace ExamService.Interfaces
{
    public interface IDeThiRepository
    {
        IQueryable<DeThi> GetQueryable();
        Task<DeThi?> GetByIdAsync(Guid id);
        Task<DeThi> CreateAsync(DeThi deThi);
        Task UpdateAsync(DeThi deThi);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsOwnerAsync(Guid id, Guid teacherId);
        Task<DeThi?> GetByIdWithResultsAsync(Guid id);
    }
}