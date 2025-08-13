using AuthService.Models.Entities;
using System;
using System.Threading.Tasks;

namespace AuthService.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức để thao tác với dữ liệu người dùng.
    /// </summary>
    public interface INguoiDungRepository
    {
        Task<NguoiDung?> GetByIdAsync(Guid id);
        Task<NguoiDung?> GetByEmailAsync(string email);
        Task AddAsync(NguoiDung nguoiDung);
        Task UpdateAsync(NguoiDung nguoiDung);
    }
}
