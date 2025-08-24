using ExamService.Models.Entities;

namespace ExamService.Interfaces
{
    public interface IMauDeThiRepository
    {
        IQueryable<MauDeThi> GetQueryable(); // Thêm phương thức này
        Task<MauDeThi?> GetByIdAsync(Guid id);
        Task<MauDeThi> CreateAsync(MauDeThi mauDeThi);
        Task UpdateAsync(MauDeThi mauDeThi);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsOwnerAsync(Guid id, Guid teacherId);
    }
}