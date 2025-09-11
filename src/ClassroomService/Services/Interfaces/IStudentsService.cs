using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    public interface IStudentsService
    {
        Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayDanhSachHocSinh(int? ownerTeacherId = null, int page = 1, int pageSize = 10);
        Task<StudentDto> LayHocSinhTheoId(int id, int? ownerTeacherId = null);
        Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayHocSinhTheoLop(int classId, int? ownerTeacherId = null, int page = 1, int pageSize = 10);
        Task<StudentDto> ThemHocSinh(CreateStudentDto dto);
        Task<StudentDto> CapNhatHocSinh(int id, UpdateStudentDto dto, int ownerTeacherId);
        Task<bool> XoaHocSinh(int id, int ownerTeacherId);
    }
}