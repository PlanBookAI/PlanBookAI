using AutoMapper;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using ExamService.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamService.Services
{
    public class TaoDeThiService : ITaoDeThiService
    {
        private readonly IDeThiRepository _deThiRepo;
        private readonly ICauHoiRepository _cauHoiRepo;
        private readonly IMauDeThiRepository _mauDeThiRepo;
        private readonly IMapper _mapper;

        public TaoDeThiService(
            IDeThiRepository deThiRepo,
            ICauHoiRepository cauHoiRepo,
            IMauDeThiRepository mauDeThiRepo,
            IMapper mapper)
        {
            _deThiRepo = deThiRepo;
            _cauHoiRepo = cauHoiRepo;
            _mauDeThiRepo = mauDeThiRepo;
            _mapper = mapper;
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> CreateFromBankAsync(TaoDeThiTuNganHangDTO dto, Guid teacherId)
        {
            var deThi = _mapper.Map<DeThi>(dto);
            deThi.Id = Guid.NewGuid();
            deThi.NguoiTaoId = teacherId;
            deThi.TrangThai = TrangThaiDeThi.Draft.ToString();

            var questions = _cauHoiRepo.GetQueryable()
                                       .Where(q => q.NguoiTaoId == teacherId && dto.DanhSachCauHoiId.Contains(q.Id))
                                       .ToList();

            if (questions.Count != dto.DanhSachCauHoiId.Count)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Một hoặc nhiều câu hỏi không tồn tại hoặc không thuộc sở hữu của bạn.");
            }

            deThi.ExamQuestions = questions.Select((q, index) => new ExamQuestion
            {
                CauHoiId = q.Id,
                ThuTu = index + 1,
                Diem = 1 // Mặc định
            }).ToList();

            await _deThiRepo.CreateAsync(deThi);
            return ApiPhanHoi<DeThiResponseDTO>.ThanhCongVoiDuLieu(_mapper.Map<DeThiResponseDTO>(deThi));
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> CreateRandomAsync(TaoDeThiNgauNhienDTO dto, Guid teacherId)
        {
            var query = _cauHoiRepo.GetQueryable()
                .Where(q => q.NguoiTaoId == teacherId && q.MonHoc == dto.MonHoc);

            if (!string.IsNullOrEmpty(dto.ChuDe)) query = query.Where(q => q.ChuDe == dto.ChuDe);
            if (!string.IsNullOrEmpty(dto.DoKho)) query = query.Where(q => q.DoKho == dto.DoKho);

            var availableQuestions = await query.Select(q => q.Id).ToListAsync();

            if (availableQuestions.Count < dto.SoLuongCauHoi)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai($"Không đủ câu hỏi trong ngân hàng để tạo đề thi. Chỉ có {availableQuestions.Count} câu hỏi phù hợp.");
            }

            var random = new Random();
            var selectedIds = availableQuestions.OrderBy(x => random.Next()).Take(dto.SoLuongCauHoi).ToList();

            var createFromBankDto = _mapper.Map<TaoDeThiTuNganHangDTO>(dto);
            createFromBankDto.DanhSachCauHoiId = selectedIds;

            return await CreateFromBankAsync(createFromBankDto, teacherId);
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> CreateAutomaticAsync(TaoDeThiTuDongDTO dto, Guid teacherId)
        {
            var allRequiredQuestions = new List<Guid>();
            var questionBankQuery = _cauHoiRepo.GetQueryable()
                .Where(q => q.NguoiTaoId == teacherId && q.MonHoc == dto.MonHoc);

            // 1. Lặp qua ma trận và lấy câu hỏi
            foreach (var requirement in dto.MaTranCauHoi)
            {
                var availableQuestions = await questionBankQuery
                    .Where(q => q.ChuDe == requirement.ChuDe && q.DoKho == requirement.DoKho)
                    .Select(q => q.Id)
                    .ToListAsync();

                if (availableQuestions.Count < requirement.SoLuong)
                {
                    return ApiPhanHoi<DeThiResponseDTO>.ThatBai($"Không đủ câu hỏi cho chủ đề '{requirement.ChuDe}' với độ khó '{requirement.DoKho}'. Yêu cầu {requirement.SoLuong} nhưng chỉ có {availableQuestions.Count}.");
                }

                // Lấy ngẫu nhiên và đảm bảo không trùng lặp
                var random = new Random();
                var selectedIds = availableQuestions
                                    .Except(allRequiredQuestions) // Loại bỏ các câu đã chọn
                                    .OrderBy(x => random.Next())
                                    .Take(requirement.SoLuong)
                                    .ToList();

                if (selectedIds.Count < requirement.SoLuong)
                {
                    // Trường hợp hiếm gặp khi câu hỏi bị trùng giữa các yêu cầu
                    return ApiPhanHoi<DeThiResponseDTO>.ThatBai($"Không đủ câu hỏi không trùng lặp cho chủ đề '{requirement.ChuDe}' với độ khó '{requirement.DoKho}'.");
                }

                allRequiredQuestions.AddRange(selectedIds);
            }

            // 2. Tái sử dụng logic của CreateFromBankAsync
            var createFromBankDto = _mapper.Map<TaoDeThiTuNganHangDTO>(dto);
            createFromBankDto.DanhSachCauHoiId = allRequiredQuestions;

            return await CreateFromBankAsync(createFromBankDto, teacherId);
        }

        public async Task<ApiPhanHoi<DeThiResponseDTO>> CreateFromTemplateAsync(TaoDeThiTuMauDTO dto, Guid teacherId)
        {
            // 1. Lấy mẫu đề thi
            var template = await _mauDeThiRepo.GetByIdAsync(dto.MauDeThiId);
            if (template == null || template.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiResponseDTO>.ThatBai("Không tìm thấy mẫu đề thi hoặc không có quyền truy cập.");
            }

            // 2. Chuyển đổi cấu trúc từ mẫu thành ma trận câu hỏi
            var cauTruc = _mapper.Map<List<YeuCauCauHoiDTO>>(template.CauTruc);
            var automaticDto = new TaoDeThiTuDongDTO
            {
                TieuDe = dto.TieuDe,
                MonHoc = dto.MonHoc,
                KhoiLop = dto.KhoiLop,
                ThoiGianLamBai = dto.ThoiGianLamBai,
                MaTranCauHoi = cauTruc
            };

            // 3. Tái sử dụng logic của CreateAutomaticAsync
            return await CreateAutomaticAsync(automaticDto, teacherId);
        }
    }
}