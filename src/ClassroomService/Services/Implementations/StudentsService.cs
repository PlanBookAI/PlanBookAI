using AutoMapper;
using ClassroomService.Models.DTOs;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;
using ClassroomService.Services.Interfaces;

namespace ClassroomService.Services.Implementations
{
    public class StudentsService : IStudentsService
    {
        private readonly IStudentsRepository _studentsRepository;
        private readonly IClassesRepository _classesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentsService> _logger;

        public StudentsService(
            IStudentsRepository studentsRepository,
            IClassesRepository classesRepository,
            IMapper mapper,
            ILogger<StudentsService> logger)
        {
            _studentsRepository = studentsRepository;
            _classesRepository = classesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayDanhSachHocSinh(int? ownerTeacherId = null, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Lấy danh sách học sinh với owner teacher ID: {OwnerTeacherId}", ownerTeacherId);

            var students = await _studentsRepository.GetByOwnerTeacherIdAsync(ownerTeacherId ?? 0, page, pageSize);
            var totalCount = await _studentsRepository.GetTotalCountAsync(ownerTeacherId: ownerTeacherId);

            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);
            return (studentDtos, totalCount);
        }

        public async Task<StudentDto> LayHocSinhTheoId(int id, int? ownerTeacherId = null)
        {
            _logger.LogInformation("Lấy thông tin học sinh ID: {Id}", id);

            var student = await _studentsRepository.GetByIdAsync(id, ownerTeacherId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học sinh với ID: {id}");
            }

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayHocSinhTheoLop(int classId, int? ownerTeacherId = null, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Lấy danh sách học sinh theo lớp ID: {ClassId}", classId);

            // Kiểm tra lớp học có tồn tại và thuộc quyền quản lý không
            if (ownerTeacherId.HasValue)
            {
                var classEntity = await _classesRepository.GetByIdAsync(classId, ownerTeacherId);
                if (classEntity == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy lớp học với ID: {classId}");
                }
            }

            var students = await _studentsRepository.GetByClassIdAsync(classId, ownerTeacherId, page, pageSize);
            var totalCount = await _studentsRepository.GetTotalCountAsync(classId, ownerTeacherId);

            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);
            return (studentDtos, totalCount);
        }

        public async Task<StudentDto> ThemHocSinh(CreateStudentDto dto)
        {
            _logger.LogInformation("Thêm học sinh mới: {FullName}", dto.FullName);

            // Kiểm tra mã học sinh đã tồn tại chưa
            var existsByCode = await _studentsRepository.ExistsByStudentCodeAsync(dto.StudentCode);
            if (existsByCode)
            {
                throw new InvalidOperationException($"Mã học sinh '{dto.StudentCode}' đã tồn tại");
            }

            // Kiểm tra lớp học có tồn tại và thuộc quyền quản lý không
            var classEntity = await _classesRepository.GetByIdAsync(dto.ClassId, dto.OwnerTeacherId);
            if (classEntity == null)
            {
                throw new InvalidOperationException("Lớp học không tồn tại hoặc không thuộc quyền quản lý của bạn");
            }

            var student = _mapper.Map<Students>(dto);
            var createdStudent = await _studentsRepository.CreateAsync(student);

            // Cập nhật số lượng học sinh trong lớp
            await UpdateClassStudentCount(dto.ClassId);

            return _mapper.Map<StudentDto>(createdStudent);
        }

        async Task<StudentDto> IStudentsService.CapNhatHocSinh(int id, UpdateStudentDto dto, int ownerTeacherId)
        {
            _logger.LogInformation("Cập nhật học sinh ID: {Id}", id);

            var existingStudent = await _studentsRepository.GetByIdAsync(id, ownerTeacherId);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học sinh với ID: {id}");
            }

            // Kiểm tra mã học sinh đã tồn tại chưa (ngoại trừ học sinh hiện tại)
            if (dto.StudentCode != existingStudent.StudentCode)
            {
                var existsByCode = await _studentsRepository.ExistsByStudentCodeAsync(dto.StudentCode, id);
                if (existsByCode)
                {
                    throw new InvalidOperationException($"Mã học sinh '{dto.StudentCode}' đã tồn tại");
                }
            }

            // Kiểm tra lớp học mới có tồn tại và thuộc quyền quản lý không
            if (dto.ClassId != existingStudent.ClassId)
            {
                var newClassEntity = await _classesRepository.GetByIdAsync(dto.ClassId, ownerTeacherId);
                if (newClassEntity == null)
                {
                    throw new InvalidOperationException("Lớp học không tồn tại hoặc không thuộc quyền quản lý của bạn");
                }
            }

            var oldClassId = existingStudent.ClassId;
            _mapper.Map(dto, existingStudent);
            var updatedStudent = await _studentsRepository.UpdateAsync(existingStudent);

            // Cập nhật số lượng học sinh nếu đổi lớp
            if (oldClassId != dto.ClassId)
            {
                await UpdateClassStudentCount(oldClassId);
                await UpdateClassStudentCount(dto.ClassId);
            }

            return _mapper.Map<StudentDto>(updatedStudent);
        }

        public async Task<bool> XoaHocSinh(int id, int ownerTeacherId)
        {
            _logger.LogInformation("Xóa học sinh ID: {Id}", id);

            var existingStudent = await _studentsRepository.GetByIdAsync(id, ownerTeacherId);
            if (existingStudent == null)
            {
                return false;
            }

            var classId = existingStudent.ClassId;
            var result = await _studentsRepository.DeleteAsync(id, ownerTeacherId);

            if (result)
            {
                // Cập nhật số lượng học sinh trong lớp
                await UpdateClassStudentCount(classId);
            }

            return result;
        }

        private async Task UpdateClassStudentCount(int classId)
        {
            try
            {
                var studentCount = await _studentsRepository.GetTotalCountAsync(classId);
                var classEntity = await _classesRepository.GetByIdAsync(classId);
                
                if (classEntity != null)
                {
                    classEntity.StudentCount = studentCount;
                    await _classesRepository.UpdateAsync(classEntity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể cập nhật số lượng học sinh cho lớp {ClassId}", classId);
                // Không throw exception để không ảnh hưởng đến operation chính
            }
        }
    }
}