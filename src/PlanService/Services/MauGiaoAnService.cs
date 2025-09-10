using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Repositories;

namespace PlanService.Services
{
    /// <summary>
    /// Service hiện thực business logic cho quản lý mẫu giáo án (lesson_templates).
    /// </summary>
    public class MauGiaoAnService : IMauGiaoAnService
    {
        private readonly IMauGiaoAnRepository _repo;

        public MauGiaoAnService(IMauGiaoAnRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiPhanHoi<IEnumerable<MauGiaoAn>>> GetAllAsync(Guid teacherId)
        {
            var all = await _repo.GetAllAsync();
            var visible = all.Where(t =>
                t.NguoiTaoId == teacherId ||
                string.Equals(t.TrangThai, "ACTIVE", StringComparison.OrdinalIgnoreCase));

            return ApiPhanHoi<IEnumerable<MauGiaoAn>>.ThanhCongOk(visible, "Lấy danh sách mẫu giáo án thành công");
        }

        public async Task<ApiPhanHoi<MauGiaoAn?>> GetByIdAsync(Guid id, Guid teacherId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return ApiPhanHoi<MauGiaoAn?>.ThatBai("Không tìm thấy mẫu giáo án");

            if (item.NguoiTaoId != teacherId &&
                !string.Equals(item.TrangThai, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                return ApiPhanHoi<MauGiaoAn?>.ThatBai("Bạn không có quyền truy cập mẫu giáo án này");

            return ApiPhanHoi<MauGiaoAn?>.ThanhCongOk(item, "Lấy chi tiết mẫu giáo án thành công");
        }

        public async Task<ApiPhanHoi<MauGiaoAn>> CreateAsync(MauGiaoAn request, Guid teacherId)
        {
            request.Id = Guid.NewGuid();
            request.NguoiTaoId = teacherId;
            request.TrangThai = "ACTIVE";
            request.TaoLuc = DateTime.UtcNow;
            request.CapNhatLuc = DateTime.UtcNow;

            await _repo.AddAsync(request);
            return ApiPhanHoi<MauGiaoAn>.ThanhCongOk(request, "Tạo mẫu giáo án thành công");
        }

        public async Task<ApiPhanHoi<MauGiaoAn>> UpdateAsync(Guid id, MauGiaoAn request, Guid teacherId)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current == null)
                return ApiPhanHoi<MauGiaoAn>.ThatBai("Không tìm thấy mẫu giáo án");
            if (current.NguoiTaoId != teacherId)
                return ApiPhanHoi<MauGiaoAn>.ThatBai("Bạn không có quyền cập nhật mẫu này");

            current.TieuDe = request.TieuDe;
            current.MoTa = request.MoTa;
            current.MonHoc = request.MonHoc;
            current.Lop = request.Lop;
            current.NoiDungMau = request.NoiDungMau;
            current.TrangThai = request.TrangThai; // ACTIVE/INACTIVE/ARCHIVED theo schema
            current.CapNhatLuc = DateTime.UtcNow;

            await _repo.UpdateAsync(current);
            return ApiPhanHoi<MauGiaoAn>.ThanhCongOk(current, "Cập nhật mẫu giáo án thành công");
        }

        public async Task<ApiPhanHoi<MauGiaoAn>> DeleteAsync(Guid id, Guid teacherId)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current == null)
                return ApiPhanHoi<MauGiaoAn>.ThatBai("Không tìm thấy mẫu giáo án");
            if (current.NguoiTaoId != teacherId)
                return ApiPhanHoi<MauGiaoAn>.ThatBai("Bạn không có quyền xóa mẫu này");

            await _repo.DeleteAsync(id);
            return ApiPhanHoi<MauGiaoAn>.ThanhCongOk(current, "Xóa mẫu giáo án thành công");
        }

        public async Task<ApiPhanHoi<MauGiaoAn>> ChiaSeAsync(Guid id, bool chiaSe, Guid teacherId)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current == null)
                return ApiPhanHoi<MauGiaoAn>.ThatBai("Không tìm thấy mẫu giáo án");
            if (current.NguoiTaoId != teacherId)
                return ApiPhanHoi<MauGiaoAn>.ThatBai("Bạn không có quyền thay đổi chia sẻ mẫu này");

            current.TrangThai = chiaSe ? "ACTIVE" : "INACTIVE";
            current.CapNhatLuc = DateTime.UtcNow;

            await _repo.UpdateAsync(current);
            return ApiPhanHoi<MauGiaoAn>.ThanhCongOk(
                current,
                chiaSe ? "Chia sẻ mẫu giáo án thành công" : "Hủy chia sẻ mẫu giáo án thành công"
            );
        }
    }
}