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
        private readonly ILogger<MauDeThiService> _logger;
        
        public MauDeThiService(IMauDeThiRepository repo, ILogger<MauDeThiService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ApiPhanHoi<MauDeThiResponseDTO>> CreateAsync(MauDeThiRequestDTO dto, Guid teacherId)
        {
            var mauDeThi = dto.Adapt<MauDeThi>();
            mauDeThi.Id = Guid.NewGuid();
            mauDeThi.NguoiTaoId = teacherId;
            
            // Set default values for nullable fields
            if (!dto.KhoiLop.HasValue) mauDeThi.KhoiLop = 10; // Default grade
            if (!dto.ThoiGianLam.HasValue) mauDeThi.ThoiGianLam = 45; // Default duration
            if (!dto.TongDiem.HasValue) mauDeThi.TongDiem = 10; // Default total score

            // Serialize CauTruc object to JSON string
            if (dto.CauTruc != null)
            {
                mauDeThi.CauTruc = JsonSerializer.Serialize(dto.CauTruc);
            }

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

            var dtos = items.Select(item => 
            {
                var dto = item.Adapt<MauDeThiResponseDTO>();
                // Deserialize CauTruc JSON string to object
                if (!string.IsNullOrEmpty(item.CauTruc))
                {
                    try
                    {
                        dto.CauTruc = JsonSerializer.Deserialize<object>(item.CauTruc) ?? item.CauTruc;
                    }
                    catch (JsonException)
                    {
                        dto.CauTruc = item.CauTruc; // Fallback to string if deserialization fails
                    }
                }
                return dto;
            }).ToList();
            
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

        // Các methods mới cho controller
        public async Task<PagedResult<MauDeThiResponseDTO>> LayDanhSachMauDeThiAsync(
            Guid teacherId, int pageNumber, int pageSize, string? monHoc = null, 
            int? khoiLop = null, string? trangThai = null)
        {
            try
            {
                var query = _repo.GetQueryable().Where(m => m.NguoiTaoId == teacherId);

                // Apply filters
                if (!string.IsNullOrEmpty(monHoc))
                    query = query.Where(m => m.MonHoc == monHoc);
                
                if (khoiLop.HasValue)
                    query = query.Where(m => m.KhoiLop == khoiLop.Value);
                
                if (!string.IsNullOrEmpty(trangThai))
                    query = query.Where(m => m.TrangThai == trangThai);

                // Order by updated date
                query = query.OrderByDescending(m => m.CapNhatLuc);

                var totalItems = await query.CountAsync();
                var items = await query.Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .AsNoTracking()
                                       .ToListAsync();

                var dtos = items.Select(item => 
                {
                    var dto = item.Adapt<MauDeThiResponseDTO>();
                    // Deserialize CauTruc JSON string to object
                    if (!string.IsNullOrEmpty(item.CauTruc))
                    {
                        try
                        {
                            dto.CauTruc = JsonSerializer.Deserialize<object>(item.CauTruc);
                        }
                        catch
                        {
                            dto.CauTruc = new { };
                        }
                    }
                    return dto;
                }).ToList();

                return new PagedResult<MauDeThiResponseDTO>(dtos, totalItems, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách mẫu đề thi: {ex.Message}");
            }
        }

        public async Task<MauDeThiResponseDTO?> LayChiTietMauDeThiAsync(Guid id, Guid teacherId)
        {
            var mauDeThi = await _repo.GetByIdAsync(id);
            
            if (mauDeThi == null || mauDeThi.NguoiTaoId != teacherId)
                return null;

            return mauDeThi.Adapt<MauDeThiResponseDTO>();
        }

        public async Task<MauDeThiResponseDTO> TaoMauDeThiAsync(MauDeThiRequestDTO request, Guid teacherId)
        {
            var mauDeThi = request.Adapt<MauDeThi>();
            mauDeThi.Id = Guid.NewGuid();
            mauDeThi.NguoiTaoId = teacherId;
            mauDeThi.TaoLuc = DateTime.UtcNow;
            mauDeThi.CapNhatLuc = DateTime.UtcNow;

            // Serialize CauTruc object to JSON string
            if (request.CauTruc != null)
            {
                mauDeThi.CauTruc = JsonSerializer.Serialize(request.CauTruc);
            }

            var createdMauDeThi = await _repo.CreateAsync(mauDeThi);
            return createdMauDeThi.Adapt<MauDeThiResponseDTO>();
        }

        public async Task<MauDeThiResponseDTO> CapNhatMauDeThiAsync(Guid id, MauDeThiRequestDTO request, Guid teacherId)
        {
            var existingMauDeThi = await _repo.GetByIdAsync(id);
            
            if (existingMauDeThi == null)
                throw new KeyNotFoundException("Không tìm thấy mẫu đề thi");
            
            if (existingMauDeThi.NguoiTaoId != teacherId)
                throw new UnauthorizedAccessException("Không có quyền cập nhật mẫu đề thi này");

            // Update properties
            request.Adapt(existingMauDeThi);
            existingMauDeThi.CapNhatLuc = DateTime.UtcNow;

            var updatedMauDeThi = await _repo.UpdateAsync(existingMauDeThi);
            return updatedMauDeThi.Adapt<MauDeThiResponseDTO>();
        }

        public async Task<bool> XoaMauDeThiAsync(Guid id, Guid teacherId)
        {
            if (!await _repo.IsOwnerAsync(id, teacherId))
                return false;

            return await _repo.DeleteAsync(id);
        }

        public async Task<MauDeThiResponseDTO> SaoChepMauDeThiAsync(Guid id, SaoChepMauDeThiDTO request, Guid teacherId)
        {
            var originalMauDeThi = await _repo.GetByIdAsync(id);
            
            if (originalMauDeThi == null)
                throw new KeyNotFoundException("Không tìm thấy mẫu đề thi");
            
            if (originalMauDeThi.NguoiTaoId != teacherId)
                throw new UnauthorizedAccessException("Không có quyền sao chép mẫu đề thi này");

            var clonedMauDeThi = new MauDeThi
            {
                Id = Guid.NewGuid(),
                TieuDe = request.TieuDeMoi,
                MoTa = request.MoTaMoi ?? originalMauDeThi.MoTa,
                MonHoc = originalMauDeThi.MonHoc,
                KhoiLop = originalMauDeThi.KhoiLop,
                ThoiGianLam = originalMauDeThi.ThoiGianLam,
                TongDiem = originalMauDeThi.TongDiem,
                CauTruc = originalMauDeThi.CauTruc,
                NguoiTaoId = teacherId,
                TrangThai = "ACTIVE",
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            };

            var newMauDeThi = await _repo.CreateAsync(clonedMauDeThi);
            return newMauDeThi.Adapt<MauDeThiResponseDTO>();
        }
    }
}