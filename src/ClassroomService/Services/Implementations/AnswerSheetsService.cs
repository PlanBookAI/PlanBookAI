using AutoMapper;
using ClassroomService.Models.DTOs;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;
using ClassroomService.Services.Interfaces;

namespace ClassroomService.Services.Implementations
{
    /// <summary>
    /// Service implementation for managing answer sheets
    /// </summary>
    public class AnswerSheetsService : IAnswerSheetsService
    {
        private readonly IAnswerSheetsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AnswerSheetsService> _logger;

        /// <summary>
        /// Initializes a new instance of AnswerSheetsService
        /// </summary>
        public AnswerSheetsService(IAnswerSheetsRepository repository, IMapper mapper, ILogger<AnswerSheetsService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new answer sheet
        /// </summary>
        public async Task<AnswerSheetDto> LuuBaiLam(CreateAnswerSheetDto dto)
        {
            _logger.LogInformation("Lưu bài làm cho học sinh {StudentId}, đề thi {ExamId}", dto.StudentId, dto.ExamId);
            
            var entity = _mapper.Map<AnswerSheets>(dto);
            entity.OcrStatus = OcrStatus.PENDING;
            entity.OcrResult = "{}";
            
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<AnswerSheetDto>(created);
        }

        /// <summary>
        /// Gets answer sheets by student ID
        /// </summary>
        public async Task<(IEnumerable<AnswerSheetDto> Items, int TotalCount)> LayBaiLamTheoHocSinh(int studentId, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Lấy bài làm cho học sinh {StudentId}, trang {Page}", studentId, page);
            
            var items = await _repository.GetByStudentIdAsync(studentId, page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync(studentId: studentId);
            
            var dtos = _mapper.Map<IEnumerable<AnswerSheetDto>>(items);
            return (dtos, totalCount);
        }

        /// <summary>
        /// Gets answer sheets by exam ID
        /// </summary>
        public async Task<(IEnumerable<AnswerSheetDto> Items, int TotalCount)> LayBaiLamTheoDeThi(int examId, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Lấy bài làm cho đề thi {ExamId}, trang {Page}", examId, page);
            
            var items = await _repository.GetByExamIdAsync(examId, page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync(examId: examId);
            
            var dtos = _mapper.Map<IEnumerable<AnswerSheetDto>>(items);
            return (dtos, totalCount);
        }

        /// <summary>
        /// Updates an existing answer sheet
        /// </summary>
        public async Task<AnswerSheetDto> CapNhatBaiLam(int id, CreateAnswerSheetDto dto)
        {
            _logger.LogInformation("Cập nhật bài làm {Id}", id);
            
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new ArgumentException($"Không tìm thấy bài làm với ID {id}");
            }
            
            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<AnswerSheetDto>(updated);
        }

        /// <summary>
        /// Gets an answer sheet by ID
        /// </summary>
        public async Task<AnswerSheetDto?> LayBaiLamTheoId(int id)
        {
            _logger.LogInformation("Lấy bài làm theo ID {Id}", id);
            
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<AnswerSheetDto>(entity) : null;
        }
    }
}
