using AutoMapper;
using ClassroomService.Models.DTOs;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;
using ClassroomService.Services.Interfaces;

namespace ClassroomService.Services.Implementations
{
    /// <summary>
    /// Service implementation for managing classes
    /// </summary>
    public class ClassesService : IClassesService
    {
        private readonly IClassesRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClassesService> _logger;

        /// <summary>
        /// Initializes a new instance of ClassesService
        /// </summary>
        public ClassesService(IClassesRepository repository, IMapper mapper, ILogger<ClassesService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new class
        /// </summary>
        public async Task<ClassDto> TaoLopHoc(CreateClassDto dto, int homeroomTeacherId)
        {
            _logger.LogInformation("Tạo lớp học mới: {Name}", dto.Name);
            
            var entity = _mapper.Map<Classes>(dto);
            entity.HomeroomTeacherId = homeroomTeacherId;
            
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<ClassDto>(created);
        }

        /// <summary>
        /// Updates an existing class
        /// </summary>
        public async Task<ClassDto> CapNhatLopHoc(int id, UpdateClassDto dto, int homeroomTeacherId)
        {
            _logger.LogInformation("Cập nhật lớp học: {Id}", id);
            
            var existing = await _repository.GetByIdAsync(id, homeroomTeacherId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp học với ID: {id}");
            }
            
            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<ClassDto>(updated);
        }

        /// <summary>
        /// Deletes a class
        /// </summary>
        public async Task<bool> XoaLopHoc(int id, int homeroomTeacherId)
        {
            _logger.LogInformation("Xóa lớp học: {Id}", id);
            return await _repository.DeleteAsync(id, homeroomTeacherId);
        }

        /// <summary>
        /// Gets paginated list of classes
        /// </summary>
        public async Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayDanhSachLopHoc(int? homeroomTeacherId = null, int page = 1, int pageSize = 10)
        {
            var items = await _repository.GetAllAsync(homeroomTeacherId, page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync(homeroomTeacherId);
            
            var dtos = _mapper.Map<IEnumerable<ClassDto>>(items);
            return (dtos, totalCount);
        }

        /// <summary>
        /// Gets a class by ID
        /// </summary>
        public async Task<ClassDto> LayLopHocTheoId(int id, int? homeroomTeacherId = null)
        {
            var entity = await _repository.GetByIdAsync(id, homeroomTeacherId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp học với ID: {id}");
            }
            
            return _mapper.Map<ClassDto>(entity);
        }

        /// <summary>
        /// Gets classes by homeroom teacher
        /// </summary>
        public async Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayLopHocTheoGiaoVien(int homeroomTeacherId, int page = 1, int pageSize = 10)
        {
            var items = await _repository.GetByHomeroomTeacherIdAsync(homeroomTeacherId, page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync(homeroomTeacherId);
            
            var dtos = _mapper.Map<IEnumerable<ClassDto>>(items);
            return (dtos, totalCount);
        }
    }
}