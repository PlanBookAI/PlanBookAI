using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing classes
    /// </summary>
    public interface IClassesService
    {
        /// <summary>
        /// Creates a new class
        /// </summary>
        Task<ClassDto> TaoLopHoc(CreateClassDto dto, int homeroomTeacherId);
        
        /// <summary>
        /// Updates an existing class
        /// </summary>
        Task<ClassDto> CapNhatLopHoc(int id, UpdateClassDto dto, int homeroomTeacherId);
        
        /// <summary>
        /// Deletes a class
        /// </summary>
        Task<bool> XoaLopHoc(int id, int homeroomTeacherId);
        
        /// <summary>
        /// Gets paginated list of classes
        /// </summary>
        Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayDanhSachLopHoc(int? homeroomTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets a class by ID
        /// </summary>
        Task<ClassDto> LayLopHocTheoId(int id, int? homeroomTeacherId = null);
        
        /// <summary>
        /// Gets classes by homeroom teacher
        /// </summary>
        Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayLopHocTheoGiaoVien(int homeroomTeacherId, int page = 1, int pageSize = 10);
    }
}