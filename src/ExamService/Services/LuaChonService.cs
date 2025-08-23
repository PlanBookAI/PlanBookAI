using AutoMapper;
using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamService.Services
{
    public class LuaChonService : ILuaChonService
    {
        private readonly ExamDbContext _context;
        private readonly ICauHoiRepository _cauHoiRepo;
        private readonly IMapper _mapper;

        public LuaChonService(ExamDbContext context, ICauHoiRepository cauHoiRepo, IMapper mapper)
        {
            _context = context;
            _cauHoiRepo = cauHoiRepo;
            _mapper = mapper;
        }

        public async Task<ApiPhanHoi<List<LuaChonResponseDTO>>> GetChoicesByQuestionIdAsync(Guid cauHoiId, Guid teacherId)
        {
            if (!await _cauHoiRepo.IsOwnerAsync(cauHoiId, teacherId))
            {
                return ApiPhanHoi<List<LuaChonResponseDTO>>.ThatBai("Không tìm thấy câu hỏi hoặc không có quyền truy cập.");
            }

            var choices = await _context.LuaChons
                .Where(lc => lc.CauHoiId == cauHoiId)
                .OrderBy(lc => lc.ThuTu)
                .AsNoTracking()
                .ToListAsync();

            var dtos = _mapper.Map<List<LuaChonResponseDTO>>(choices);
            return ApiPhanHoi<List<LuaChonResponseDTO>>.ThanhCongVoiDuLieu(dtos);
        }

        public async Task<ApiPhanHoi<LuaChonResponseDTO>> CreateAsync(TaoLuaChonRequestDTO dto, Guid teacherId)
        {
            if (!await _cauHoiRepo.IsOwnerAsync(dto.CauHoiId, teacherId))
            {
                return ApiPhanHoi<LuaChonResponseDTO>.ThatBai("Không thể thêm lựa chọn vào câu hỏi không tồn tại hoặc không thuộc sở hữu của bạn.");
            }

            var luaChon = _mapper.Map<LuaChon>(dto);
            luaChon.Id = Guid.NewGuid();

            await _context.LuaChons.AddAsync(luaChon);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<LuaChonResponseDTO>(luaChon);
            return ApiPhanHoi<LuaChonResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Tạo lựa chọn thành công.");
        }

        public async Task<ApiPhanHoi<LuaChonResponseDTO>> UpdateAsync(Guid luaChonId, LuaChonRequestDTO dto, Guid teacherId)
        {
            var luaChon = await _context.LuaChons
                .Include(lc => lc.CauHoi) // Tải câu hỏi cha để kiểm tra owner
                .FirstOrDefaultAsync(lc => lc.Id == luaChonId);

            if (luaChon == null || luaChon.CauHoi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<LuaChonResponseDTO>.ThatBai("Không tìm thấy lựa chọn hoặc không có quyền truy cập.");
            }

            _mapper.Map(dto, luaChon);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<LuaChonResponseDTO>(luaChon);
            return ApiPhanHoi<LuaChonResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Cập nhật lựa chọn thành công.");
        }

        public async Task<ApiPhanHoi<bool>> DeleteAsync(Guid luaChonId, Guid teacherId)
        {
            var luaChon = await _context.LuaChons
                .Include(lc => lc.CauHoi)
                .FirstOrDefaultAsync(lc => lc.Id == luaChonId);

            if (luaChon == null || luaChon.CauHoi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<bool>.ThatBai("Không tìm thấy lựa chọn hoặc không có quyền truy cập.");
            }

            _context.LuaChons.Remove(luaChon);
            await _context.SaveChangesAsync();

            return ApiPhanHoi<bool>.ThanhCongVoiDuLieu(true, "Xóa lựa chọn thành công.");
        }
    }
}