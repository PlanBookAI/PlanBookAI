using ExamService.Models.Entities;

namespace ExamService.Interfaces
{
    public interface IMauDeThiRepository
    {
        IQueryable<MauDeThi> GetQueryable();
        Task<MauDeThi?> GetByIdAsync(Guid id);
        Task<MauDeThi> CreateAsync(MauDeThi mauDeThi);
        Task<MauDeThi> UpdateAsync(MauDeThi mauDeThi);
        Task<bool> DeleteAsync(Guid id);
        Task<List<MauDeThi>> GetByCreatedByAsync(Guid createdBy);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsOwnerAsync(Guid id, Guid teacherId);
    }
}