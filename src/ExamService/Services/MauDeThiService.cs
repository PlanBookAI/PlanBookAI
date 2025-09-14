using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Mapster;

namespace ExamService.Services
{
    public class MauDeThiService : IMauDeThiService
    {
        private readonly IMauDeThiRepository _repo;
        public MauDeThiService(IMauDeThiRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiPhanHoi<MauDeThiResponseDTO>> CreateAsync(MauDeThiRequestDTO dto, Guid teacherId)
        {
            var mauDeThi = dto.Adapt<MauDeThi>();
            mauDeThi.Id = Guid.NewGuid();
            mauDeThi.NguoiTaoId = teacherId;

            // Sửa lại để gọi Repository
            await _repo.CreateAsync(mauDeThi);

            return ApiPhanHoi<MauDeThiResponseDTO>.ThanhCongVoiDuLieu(
                mauDeThi.Adapt<MauDeThiResponseDTO>(), "Tạo mẫu đề thi thành công.");
        }

        public async Task<ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO paging)
        {
            var query = _repo.GetQueryable().Where(m => m.NguoiTaoId == teacherId);

            // Sắp xếp (mặc định theo ngày cập nhật mới nhất)
            query = query.OrderByDescending(m => m.CapNhatLuc);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((paging.PageNumber - 1) * paging.PageSize)
                                   .Take(paging.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            var dtos = items.Adapt<List<MauDeThiResponseDTO>>();
            var pagedResult = new PagedResult<MauDeThiResponseDTO>(dtos, totalItems, paging.PageNumber, paging.PageSize);

            return ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>.ThanhCongVoiDuLieu(pagedResult);
        }

        public async Task<ApiPhanHoi<MauDeThiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId)
        {
            var mauDeThi = await _repo.GetByIdAsync(id);

            // Kiểm tra xem mẫu có tồn tại và thuộc sở hữu của giáo viên không
            if (mauDeThi == null || mauDeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<MauDeThiResponseDTO>.ThatBai("Không tìm thấy mẫu đề thi hoặc không có quyền truy cập.");
            }

            var dto = mauDeThi.Adapt<MauDeThiResponseDTO>();
            return ApiPhanHoi<MauDeThiResponseDTO>.ThanhCongVoiDuLieu(dto);
        }

        public async Task<ApiPhanHoi<MauDeThiResponseDTO>> UpdateAsync(Guid id, MauDeThiRequestDTO dto, Guid teacherId)
        {
            // Phải get entity ra để EF Core theo dõi thay đổi
            var existingMauDeThi = await _repo.GetByIdAsync(id);

            // 1. Kiểm tra tồn tại và quyền sở hữu
            if (existingMauDeThi == null || existingMauDeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<MauDeThiResponseDTO>.ThatBai("Không tìm thấy mẫu đề thi hoặc không có quyền truy cập.");
            }

            // 2. Map các thay đổi từ DTO vào entity đã có
            dto.Adapt(existingMauDeThi);
            existingMauDeThi.CapNhatLuc = DateTime.UtcNow;

            // 3. Gọi Repository để lưu
            await _repo.UpdateAsync(existingMauDeThi);

            var responseDto = existingMauDeThi.Adapt<MauDeThiResponseDTO>();
            return ApiPhanHoi<MauDeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Cập nhật mẫu đề thi thành công.");
        }

        public async Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId)
        {
            // 1. Kiểm tra quyền sở hữu trước khi xóa
            if (!await _repo.IsOwnerAsync(id, teacherId))
            {
                return ApiPhanHoi<bool>.ThatBai("Không tìm thấy mẫu đề thi hoặc không có quyền truy cập.");
            }

            // 2. Gọi Repository để thực hiện xóa
            var deleteResult = await _repo.DeleteAsync(id);

            if (!deleteResult)
            {
                // Trường hợp này hiếm khi xảy ra nếu IsOwnerAsync đã thành công,
                // nhưng vẫn nên có để xử lý race condition.
                return ApiPhanHoi<bool>.ThatBai("Xóa mẫu đề thi thất bại.");
            }

            return ApiPhanHoi<bool>.ThanhCongVoiDuLieu(true, "Xóa mẫu đề thi thành công.");
        }

        public async Task<ApiPhanHoi<MauDeThiResponseDTO>> CloneAsync(Guid id, Guid teacherId)
        {
            // 1. Lấy mẫu đề thi gốc
            // Dùng GetByIdAsync đã bỏ AsNoTracking() để EF có thể theo dõi entity này nếu cần,
            // nhưng trong trường hợp này chúng ta tạo mới nên không sao.
            var originalMauDeThi = await _repo.GetByIdAsync(id);

            // 2. Kiểm tra tồn tại và quyền sở hữu
            if (originalMauDeThi == null || originalMauDeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<MauDeThiResponseDTO>.ThatBai("Không tìm thấy mẫu đề thi hoặc không có quyền truy cập.");
            }

            // 3. Tạo đối tượng mới và sao chép thuộc tính
            var clonedMauDeThi = new MauDeThi
            {
                Id = Guid.NewGuid(),
                TieuDe = $"{originalMauDeThi.TieuDe} - Bản sao",
                MoTa = originalMauDeThi.MoTa,
                MonHoc = originalMauDeThi.MonHoc,
                KhoiLop = originalMauDeThi.KhoiLop,
                CauTruc = originalMauDeThi.CauTruc, // Cấu trúc JSON chỉ là chuỗi, có thể copy trực tiếp
                NguoiTaoId = teacherId, // Người sao chép trở thành chủ sở hữu mới
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            };

            // 4. Lưu bản sao vào database
            var newMauDeThi = await _repo.CreateAsync(clonedMauDeThi);

            var responseDto = newMauDeThi.Adapt<MauDeThiResponseDTO>();
            return ApiPhanHoi<MauDeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Sao chép mẫu đề thi thành công.");
        }
    }
}