using AutoMapper;
using ClassroomService.Models.DTOs;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;
using ClassroomService.Services.Interfaces;

namespace ClassroomService.Services.Implementations
{
    /// <summary>
    /// Service implementation for managing student results
    /// </summary>
    public class StudentResultsService : IStudentResultsService
    {
        private readonly IStudentResultsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentResultsService> _logger;

        /// <summary>
        /// Initializes a new instance of StudentResultsService
        /// </summary>
        public StudentResultsService(IStudentResultsRepository repository, IMapper mapper, ILogger<StudentResultsService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new student result
        /// </summary>
        public async Task<StudentResultDto> LuuKetQua(CreateStudentResultDto dto)
        {
            _logger.LogInformation("Lưu kết quả thi cho học sinh {StudentId}, đề thi {ExamId}", dto.StudentId, dto.ExamId);
            
            var entity = _mapper.Map<StudentResults>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<StudentResultDto>(created);
        }

        /// <summary>
        /// Gets student results by student ID
        /// </summary>
        public async Task<(IEnumerable<StudentResultDto> Items, int TotalCount)> LayKetQuaTheoHocSinh(Guid studentId, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Lấy kết quả thi cho học sinh {StudentId}, trang {Page}", studentId, page);
            
            var items = await _repository.GetByStudentIdAsync(studentId, page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync(studentId: studentId);
            
            var dtos = _mapper.Map<IEnumerable<StudentResultDto>>(items);
            return (dtos, totalCount);
        }

        /// <summary>
        /// Gets student results by exam ID
        /// </summary>
        public async Task<(IEnumerable<StudentResultDto> Items, int TotalCount)> LayKetQuaTheoDeThi(Guid examId, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Lấy kết quả thi cho đề thi {ExamId}, trang {Page}", examId, page);
            
            var items = await _repository.GetByExamIdAsync(examId, page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync(examId: examId);
            
            var dtos = _mapper.Map<IEnumerable<StudentResultDto>>(items);
            return (dtos, totalCount);
        }

        /// <summary>
        /// Updates an existing student result
        /// </summary>
        public async Task<StudentResultDto> CapNhatKetQua(Guid id, CreateStudentResultDto dto)
        {
            _logger.LogInformation("Cập nhật kết quả thi {Id}", id);
            
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new ArgumentException($"Không tìm thấy kết quả thi với ID {id}");
            }
            
            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<StudentResultDto>(updated);
        }

        /// <summary>
        /// Gets a student result by ID
        /// </summary>
        public async Task<StudentResultDto?> LayKetQuaTheoId(Guid id)
        {
            _logger.LogInformation("Lấy kết quả thi theo ID {Id}", id);
            
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<StudentResultDto>(entity) : null;
        }
    }
}
