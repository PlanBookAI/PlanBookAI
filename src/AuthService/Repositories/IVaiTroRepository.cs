using AuthService.Models.Entities;
using System.Threading.Tasks;

namespace AuthService.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức để thao tác với dữ liệu vai trò.
    /// </summary>
    public interface IVaiTroRepository
    {
        Task<VaiTro> GetByTenVaiTroAsync(string tenVaiTro); // Tìm vai trò theo tên.
    }
}
