using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanService.Models.Entities;

namespace PlanService.Repositories
{
    /// <summary>
    /// Interface cho việc truy cập dữ liệu của Mẫu Giáo Án.
    /// </summary>
    public interface IMauGiaoAnRepository
    {
        Task<IEnumerable<MauGiaoAn>> GetAllAsync();
        Task<MauGiaoAn?> GetByIdAsync(Guid id);
        Task AddAsync(MauGiaoAn mauGiaoAn);
        Task UpdateAsync(MauGiaoAn mauGiaoAn);
        Task DeleteAsync(Guid id);
    }
}