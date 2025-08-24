using ExamService.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamService.Interfaces
{
    public interface ILuaChonService
    {
        Task<ApiPhanHoi<List<LuaChonResponseDTO>>> GetChoicesByQuestionIdAsync(Guid cauHoiId, Guid teacherId);
        Task<ApiPhanHoi<LuaChonResponseDTO>> CreateAsync(TaoLuaChonRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<LuaChonResponseDTO>> UpdateAsync(Guid luaChonId, LuaChonRequestDTO dto, Guid teacherId);
        Task<ApiPhanHoi<bool>> DeleteAsync(Guid luaChonId, Guid teacherId);
    }
}