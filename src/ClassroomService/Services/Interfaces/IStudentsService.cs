using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing students
    /// </summary>
    public interface IStudentsService
    {
        /// <summary>
        /// Gets paginated list of students
        /// </summary>
        Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayDanhSachHocSinh(int? ownerTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets a student by ID
        /// </summary>
        Task<StudentDto> LayHocSinhTheoId(int id, int? ownerTeacherId = null);
        
        /// <summary>
        /// Gets students by class ID with pagination
        /// </summary>
        Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayHocSinhTheoLop(int classId, int? ownerTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Creates a new student
        /// </summary>
        Task<StudentDto> ThemHocSinh(CreateStudentDto dto);
        
        /// <summary>
        /// Updates an existing student
        /// </summary>
        Task<StudentDto> CapNhatHocSinh(int id, UpdateStudentDto dto, int ownerTeacherId);
        
        /// <summary>
        /// Deletes a student
        /// </summary>
        Task<bool> XoaHocSinh(int id, int ownerTeacherId);
    }
}