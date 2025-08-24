using ExamService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface ICauHoiRepository
    {
        Task<List<CauHoi>> GetAllAsync(Guid teacherId);
        Task<CauHoi?> GetByIdAsync(Guid id);
        Task<CauHoi> CreateAsync(CauHoi cauHoi);
        Task<CauHoi> UpdateAsync(CauHoi cauHoi);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsOwnerAsync(Guid id, Guid teacherId);
        IQueryable<CauHoi> GetQueryable();
    }
}