using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanService.Models.Entities;

namespace PlanService.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức cơ bản cho việc truy cập dữ liệu của Giáo Án.
    /// Giúp tách biệt logic truy cập dữ liệu với logic nghiệp vụ.
    /// </summary>
    public interface IGiaoAnRepository
    {
        Task<IEnumerable<GiaoAn>> GetAllAsync(); // Lấy tất cả các giáo án.
        Task<GiaoAn> GetByIdAsync(Guid id); // Lấy một giáo án theo Id.
        Task AddAsync(GiaoAn giaoAn); 
        Task UpdateAsync(GiaoAn giaoAn); 
        Task DeleteAsync(Guid id); 
    }
}
