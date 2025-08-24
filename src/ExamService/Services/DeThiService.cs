using AutoMapper;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ExamService.Models.Enums;
using ExamService.Documents; // Thêm using cho Document
using QuestPDF.Fluent;      // Thêm using cho QuestPDF
using ExamService.Helpers; // Thêm using cho Helper

namespace ExamService.Services
{
    public class DeThiService : IDeThiService
    {
        private readonly IDeThiRepository _deThiRepo;
        private readonly ICauHoiRepository _cauHoiRepo; // Thêm repository cho câu hỏi
        private readonly IMapper _mapper;

        public DeThiService(IDeThiRepository deThiRepo, ICauHoiRepository cauHoiRepo, IMapper mapper)
        {
            _deThiRepo = deThiRepo;
            _cauHoiRepo = cauHoiRepo;
            _mapper = mapper;
        }

        public async Task<ApiPhanHoi<PagedResult<DeThiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO pagingParams)
        {
            var query = _deThiRepo.GetQueryable().Where(d => d.NguoiTaoId == teacherId);

            // TODO: Filtering, Sorting
            query = query.OrderByDescending(d => d.CapNhatLuc);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
                                   .Take(pagingParams.PageSize)
                                   .Include(d => d.ExamQuestions)
                                   .AsNoTracking()
                                   .ToListAsync();

            var dtos = _mapper.Map<List<DeThiResponseDTO>>(items);
            var pagedResult = new PagedResult<DeThiResponseDTO>(dtos, totalItems, pagingParams.PageNumber, pagingParams.PageSize);

            return ApiPhanHoi<PagedResult<DeThiResponseDTO>>.ThanhCongVoiDuLieu(pagedResult);
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId)
        {
            var deThi = await _deThiRepo.GetByIdAsync(id);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            var responseDto = _mapper.Map<DeThiResponseDTO>(deThi);

            // Sắp xếp câu hỏi theo thứ tự
            responseDto.CauHois = responseDto.CauHois.OrderBy(q => q.ThuTu).ToList();

            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto);
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> CreateAsync(DeThiRequestDTO dto, Guid teacherId)
        {
            var deThi = _mapper.Map<DeThi>(dto);
            deThi.Id = Guid.NewGuid();
            deThi.NguoiTaoId = teacherId;
            deThi.TrangThai = "draft"; // Trạng thái mặc định

            var newDeThi = await _deThiRepo.CreateAsync(deThi);
            var responseDto = _mapper.Map<DeThiResponseDTO>(newDeThi);

            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Tạo đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> UpdateAsync(Guid id, DeThiRequestDTO dto, Guid teacherId)
        {
            var existingDeThi = await _deThiRepo.GetByIdAsync(id);
            if (existingDeThi == null || existingDeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            _mapper.Map(dto, existingDeThi);
            existingDeThi.CapNhatLuc = DateTime.UtcNow;

            await _deThiRepo.UpdateAsync(existingDeThi);
            var responseDto = _mapper.Map<DeThiResponseDTO>(existingDeThi);

            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Cập nhật đề thi thành công.");
        }

        public async Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId)
        {
            if (!await _deThiRepo.IsOwnerAsync(id, teacherId))
            {
                return ApiPhanHoi<bool>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            var result = await _deThiRepo.DeleteAsync(id);
            if (!result)
            {
                return ApiPhanHoi<bool>.ThatBai("Xóa đề thi thất bại.");
            }
            return ApiPhanHoi<bool>.ThanhCongVoiDuLieu(true, "Xóa đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> AddQuestionToExamAsync(Guid deThiId, ThemCauHoiVaoDeThiDTO dto, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // Không cho phép sửa đề thi đã xuất bản
            if (deThi.TrangThai != "draft")
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không thể thêm câu hỏi vào đề thi đã xuất bản.");
            }

            // 2. Lấy câu hỏi và kiểm tra quyền sở hữu
            var cauHoi = await _cauHoiRepo.GetByIdAsync(dto.CauHoiId);
            if (cauHoi == null || cauHoi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy câu hỏi hoặc không có quyền truy cập.");
            }

            // 3. Kiểm tra câu hỏi đã tồn tại trong đề thi chưa
            if (deThi.ExamQuestions.Any(eq => eq.CauHoiId == dto.CauHoiId))
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Câu hỏi này đã tồn tại trong đề thi.");
            }

            // 4. Tạo bản ghi liên kết mới
            var examQuestion = new ExamQuestion
            {
                DeThiId = deThiId,
                CauHoiId = dto.CauHoiId,
                Diem = dto.Diem,
                // Gán thứ tự là câu hỏi cuối cùng
                ThuTu = deThi.ExamQuestions.Any() ? deThi.ExamQuestions.Max(eq => eq.ThuTu) + 1 : 1
            };

            deThi.ExamQuestions.Add(examQuestion);

            // 5. Cập nhật lại đề thi
            deThi.CapNhatLuc = DateTime.UtcNow;
            await _deThiRepo.UpdateAsync(deThi);

            var responseDto = _mapper.Map<DeThiResponseDTO>(deThi);
            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Thêm câu hỏi vào đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> RemoveQuestionFromExamAsync(Guid deThiId, Guid cauHoiId, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // 2. Không cho phép sửa đề thi đã xuất bản
            if (deThi.TrangThai != "draft")
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không thể xóa câu hỏi khỏi đề thi đã xuất bản.");
            }

            // 3. Tìm câu hỏi cần xóa trong đề thi
            var examQuestionToRemove = deThi.ExamQuestions.FirstOrDefault(eq => eq.CauHoiId == cauHoiId);
            if (examQuestionToRemove == null)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Câu hỏi không tồn tại trong đề thi này.");
            }

            // 4. Gỡ bỏ bản ghi liên kết
            deThi.ExamQuestions.Remove(examQuestionToRemove);

            // 5. Cập nhật lại đề thi
            deThi.CapNhatLuc = DateTime.UtcNow;
            await _deThiRepo.UpdateAsync(deThi);

            var responseDto = _mapper.Map<DeThiResponseDTO>(deThi);
            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Xóa câu hỏi khỏi đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> UpdateQuestionInExamAsync(Guid deThiId, Guid cauHoiId, CapNhatCauHoiTrongDeThiDTO dto, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // 2. Không cho phép sửa đề thi đã xuất bản
            if (deThi.TrangThai != "draft")
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không thể cập nhật câu hỏi trong đề thi đã xuất bản.");
            }

            // 3. Tìm câu hỏi cần cập nhật trong đề thi
            var examQuestionToUpdate = deThi.ExamQuestions.FirstOrDefault(eq => eq.CauHoiId == cauHoiId);
            if (examQuestionToUpdate == null)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Câu hỏi không tồn tại trong đề thi này.");
            }

            // 4. Cập nhật các thuộc tính được cung cấp
            if (dto.Diem.HasValue)
            {
                examQuestionToUpdate.Diem = dto.Diem.Value;
            }

            // 5. Cập nhật lại đề thi
            deThi.CapNhatLuc = DateTime.UtcNow;
            await _deThiRepo.UpdateAsync(deThi);

            var responseDto = _mapper.Map<DeThiResponseDTO>(deThi);
            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Cập nhật thông tin câu hỏi trong đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> PublishExamAsync(Guid deThiId, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // 2. Kiểm tra trạng thái hiện tại
            if (deThi.TrangThai != TrangThaiDeThi.Draft.ToString())
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai($"Chỉ có thể xuất bản đề thi ở trạng thái '{TrangThaiDeThi.Draft}'. Trạng thái hiện tại: '{deThi.TrangThai}'.");
            }

            // 3. Kiểm tra điều kiện nghiệp vụ
            if (!deThi.ExamQuestions.Any())
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không thể xuất bản đề thi rỗng. Vui lòng thêm ít nhất một câu hỏi.");
            }

            // TODO: Có thể thêm các kiểm tra khác ở đây, ví dụ: tổng điểm phải bằng 10.

            // 4. Thay đổi trạng thái
            deThi.TrangThai = TrangThaiDeThi.Published.ToString();
            deThi.CapNhatLuc = DateTime.UtcNow;

            // 5. Lưu thay đổi
            await _deThiRepo.UpdateAsync(deThi);

            var responseDto = _mapper.Map<DeThiResponseDTO>(deThi);

            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Xuất bản đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> UnpublishExamAsync(Guid deThiId, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // 2. Kiểm tra trạng thái hiện tại
            if (deThi.TrangThai != TrangThaiDeThi.Published.ToString())
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai($"Chỉ có thể hủy xuất bản đề thi ở trạng thái '{TrangThaiDeThi.Published}'. Trạng thái hiện tại: '{deThi.TrangThai}'.");
            }

            // 3. Kiểm tra điều kiện nghiệp vụ
            // Giả sử chúng ta có bảng `BaiLam` (AnswerSheet) để kiểm tra xem đã có ai làm bài chưa.
            // `deThi.BaiLams` là navigation property.
            if (deThi.BaiLams != null && deThi.BaiLams.Any())
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không thể hủy xuất bản. Đề thi này đã có học sinh làm bài.");
            }

            // 4. Thay đổi trạng thái
            deThi.TrangThai = TrangThaiDeThi.Draft.ToString();
            deThi.CapNhatLuc = DateTime.UtcNow;

            // 5. Lưu thay đổi
            await _deThiRepo.UpdateAsync(deThi);

            var responseDto = _mapper.Map<DeThiResponseDTO>(deThi);
            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Hủy xuất bản đề thi thành công. Bạn có thể tiếp tục chỉnh sửa.");
        }

        public async Task<ApiPhanHoi<byte[]>> ExportToPdfAsync(Guid deThiId, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<byte[]>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // 2. Kiểm tra điều kiện nghiệp vụ
            if (!deThi.ExamQuestions.Any())
            {
                return ApiPhanHoi<byte[]>.ThatBai("Không thể xuất PDF cho đề thi rỗng.");
            }

            // 3. Tạo document và sinh file PDF
            var document = new DeThiDocument(deThi);
            byte[] pdfBytes = document.GeneratePdf();

            return ApiPhanHoi<byte[]>.ThanhCongVoiDuLieu(pdfBytes);
        }

        public async Task<ApiPhanHoi<byte[]>> ExportToWordAsync(Guid deThiId, Guid teacherId)
        {
            // 1. Lấy đề thi và kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<byte[]>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            // 2. Kiểm tra điều kiện nghiệp vụ
            if (!deThi.ExamQuestions.Any())
            {
                return ApiPhanHoi<byte[]>.ThatBai("Không thể xuất Word cho đề thi rỗng.");
            }

            // 3. Gọi helper để tạo file Word
            byte[] wordBytes = WordExportHelper.CreateDeThiDocument(deThi);

            return ApiPhanHoi<byte[]>.ThanhCongVoiDuLieu(wordBytes);
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> CloneExamAsync(Guid deThiId, Guid teacherId)
        {
            // 1. Lấy đề thi gốc và kiểm tra quyền sở hữu
            var originalDeThi = await _deThiRepo.GetByIdAsync(deThiId);
            if (originalDeThi == null || originalDeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy đề thi gốc hoặc không có quyền truy cập.");
            }

            // 2. Tạo một đối tượng DeThi mới (bản sao)
            var clonedDeThi = new DeThi
            {
                Id = Guid.NewGuid(),
                // 3. Sao chép các thuộc tính
                TieuDe = $"{originalDeThi.TieuDe} - Bản sao",
                MonHoc = originalDeThi.MonHoc,
                KhoiLop = originalDeThi.KhoiLop,
                ThoiGianLamBai = originalDeThi.ThoiGianLamBai,
                HuongDan = originalDeThi.HuongDan,
                NguoiTaoId = teacherId,
                TrangThai = TrangThaiDeThi.Draft.ToString(), // Bản sao luôn là bản nháp
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow,
                ExamQuestions = new List<ExamQuestion>() // Khởi tạo danh sách câu hỏi mới
            };

            // 4. Sao chép danh sách câu hỏi từ đề thi gốc
            if (originalDeThi.ExamQuestions != null)
            {
                foreach (var originalEq in originalDeThi.ExamQuestions.OrderBy(q => q.ThuTu))
                {
                    clonedDeThi.ExamQuestions.Add(new ExamQuestion
                    {
                        // Không cần gán DeThiId, EF Core sẽ tự động làm điều đó
                        CauHoiId = originalEq.CauHoiId,
                        Diem = originalEq.Diem,
                        ThuTu = originalEq.ThuTu
                    });
                }
            }

            // 5. Lưu đề thi mới vào database
            var newDeThi = await _deThiRepo.CreateAsync(clonedDeThi);

            var responseDto = _mapper.Map<DeThiResponseDTO>(newDeThi);
            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Sao chép đề thi thành công.");
        }

        public async Task<ApiPhanHoi<DeThiThongKeDTO>> GetExamStatisticsAsync(Guid deThiId, Guid teacherId)
        {
            // 1. Lấy đề thi và kết quả, kiểm tra quyền sở hữu
            var deThi = await _deThiRepo.GetByIdWithResultsAsync(deThiId);
            if (deThi == null || deThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiThongKeDTO>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            var results = deThi.BaiLams.Select(bl => bl.KetQua).Where(kq => kq != null).ToList();

            if (!results.Any())
            {
                return ApiPhanHoi<DeThiThongKeDTO>.ThatBai("Chưa có học sinh nào nộp bài cho đề thi này.");
            }

            // 2. Tính toán các chỉ số chung
            var thongKe = new DeThiThongKeDTO
            {
                DeThiId = deThi.Id,
                TieuDe = deThi.TieuDe,
                SoLuongNopBai = results.Count,
                DiemTrungBinh = results.Average(r => r.Diem),
                DiemCaoNhat = results.Max(r => r.Diem),
                DiemThapNhat = results.Min(r => r.Diem)
            };

            // 3. Phân tích từng câu hỏi
            // (Đây là phần giả định, vì logic chấm bài và lưu chi tiết câu trả lời chưa được triển khai)
            // Giả sử `KetQua` có một thuộc tính `ChiTietTraLoi` là Dictionary<Guid, bool> (CauHoiId -> IsCorrect)

            // --- PHẦN GIẢ ĐỊNH LOGIC CHẤM BÀI ---
            // Trong thực tế, bạn sẽ cần một cấu trúc dữ liệu để lưu câu trả lời chi tiết của từng học sinh.
            // Ví dụ: một bảng `AnswerDetails` (BaiLamId, CauHoiId, DapAnChon, IsCorrect)
            // Vì chưa có, chúng ta sẽ tạo dữ liệu giả để minh họa.
            var random = new Random();

            foreach (var eq in deThi.ExamQuestions.OrderBy(q => q.ThuTu))
            {
                var cauHoiThongKe = new CauHoiThongKeDTO
                {
                    CauHoiId = eq.CauHoiId,
                    NoiDungCauHoi = eq.CauHoi.NoiDung,
                    ThuTuTrongDe = eq.ThuTu,
                    SoLanTraLoiDung = random.Next(0, thongKe.SoLuongNopBai + 1) // Dữ liệu giả
                };
                cauHoiThongKe.SoLanTraLoiSai = thongKe.SoLuongNopBai - cauHoiThongKe.SoLanTraLoiDung;
                cauHoiThongKe.TyLeTraLoiDung = thongKe.SoLuongNopBai > 0
                    ? Math.Round((double)cauHoiThongKe.SoLanTraLoiDung / thongKe.SoLuongNopBai, 2)
                    : 0;

                thongKe.ThongKeCauHoi.Add(cauHoiThongKe);
            }
            // --- KẾT THÚC PHẦN GIẢ ĐỊNH ---

            return ApiPhanHoi<DeThiThongKeDTO>.ThanhCongVoiDuLieu(thongKe);
        }

    }
}