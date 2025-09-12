using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    public interface IClassesService
    {
        Task<ClassDto> TaoLopHoc(CreateClassDto dto, int homeroomTeacherId);
        Task<ClassDto> CapNhatLopHoc(int id, UpdateClassDto dto, int homeroomTeacherId);
        Task<bool> XoaLopHoc(int id, int homeroomTeacherId);
        Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayDanhSachLopHoc(int? homeroomTeacherId = null, int page = 1, int pageSize = 10);
        Task<ClassDto> LayLopHocTheoId(int id, int? homeroomTeacherId = null);
        Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayLopHocTheoGiaoVien(int homeroomTeacherId, int page = 1, int pageSize = 10);
    }
}