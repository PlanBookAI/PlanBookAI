using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;

namespace ExamService.Services
{
    public class DeThiCauHoiService : IDeThiCauHoiService
    {
        private readonly ExamDbContext _context;
        private readonly IDeThiRepository _deThiRepo;
        public DeThiCauHoiService(ExamDbContext context, IDeThiRepository deThiRepo)
        {
            _context = context;
            _deThiRepo = deThiRepo;
        }

        public async Task<ApiPhanHoi<List<DeThiCauHoiResponseDTO>>> GetQuestionsByExamIdAsync(Guid deThiId, Guid teacherId)
        {
            if (!await _deThiRepo.IsOwnerAsync(deThiId, teacherId))
            {
                return ApiPhanHoi<List<DeThiCauHoiResponseDTO>>.ThatBai("Không tìm thấy đề thi hoặc không có quyền truy cập.");
            }

            var examQuestions = await _context.ExamQuestions
                .Where(eq => eq.DeThiId == deThiId)
                .Include(eq => eq.CauHoi)
                    .ThenInclude(c => c.LuaChons)
                .OrderBy(eq => eq.ThuTu)
                .AsNoTracking()
                .ToListAsync();

            var dtos = examQuestions.Adapt<List<DeThiCauHoiResponseDTO>>();
            return ApiPhanHoi<List<DeThiCauHoiResponseDTO>>.ThanhCongVoiDuLieu(dtos);
        }

        public async Task<ApiPhanHoi<DeThiCauHoiResponseDTO>> UpdateOrderAsync(Guid examQuestionId, int newOrder, Guid teacherId)
        {
            var examQuestion = await _context.ExamQuestions.Include(eq => eq.DeThi)
                                   .FirstOrDefaultAsync(eq => eq.Id == examQuestionId);

            if (examQuestion == null || examQuestion.DeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiCauHoiResponseDTO>.ThatBai("Không tìm thấy câu hỏi trong đề thi hoặc không có quyền truy cập.");
            }
            if (examQuestion.DeThi.TrangThai != "draft")
            {
                return ApiPhanHoi<DeThiCauHoiResponseDTO>.ThatBai("Không thể thay đổi đề thi đã xuất bản.");
            }

            examQuestion.ThuTu = newOrder;
            await _context.SaveChangesAsync();

            var dto = examQuestion.Adapt<DeThiCauHoiResponseDTO>();
            return ApiPhanHoi<DeThiCauHoiResponseDTO>.ThanhCongVoiDuLieu(dto, "Cập nhật thứ tự thành công.");
        }

        public async Task<ApiPhanHoi<DeThiCauHoiResponseDTO>> UpdatePointsAsync(Guid examQuestionId, decimal newPoints, Guid teacherId)
        {
            var examQuestion = await _context.ExamQuestions.Include(eq => eq.DeThi)
                                   .FirstOrDefaultAsync(eq => eq.Id == examQuestionId);

            if (examQuestion == null || examQuestion.DeThi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<DeThiCauHoiResponseDTO>.ThatBai("Không tìm thấy câu hỏi trong đề thi hoặc không có quyền truy cập.");
            }
            if (examQuestion.DeThi.TrangThai != "draft")
            {
                return ApiPhanHoi<DeThiCauHoiResponseDTO>.ThatBai("Không thể thay đổi đề thi đã xuất bản.");
            }

            examQuestion.Diem = newPoints;
            await _context.SaveChangesAsync();

            var dto = examQuestion.Adapt<DeThiCauHoiResponseDTO>();
            return ApiPhanHoi<DeThiCauHoiResponseDTO>.ThanhCongVoiDuLieu(dto, "Cập nhật điểm thành công.");
        }
    }
}