using Microsoft.EntityFrameworkCore;
using ClassroomService.Data;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;

namespace ClassroomService.Repositories.Implementations
{
    // Lớp repository để quản lý các thao tác với bảng Classes trong cơ sở dữ liệu.
    public class ClassesRepository : IClassesRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Lấy một lớp học dựa trên ID, có thể lọc thêm theo ID của giáo viên chủ nhiệm.
        public async Task<Classes> GetByIdAsync(int id, int? homeroomTeacherId = null)
        {
            var query = _context.Classes.AsQueryable();
            
            if (homeroomTeacherId.HasValue)
            {
                query = query.Where(c => c.HomeroomTeacherId == homeroomTeacherId);
            }
            
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// Lấy danh sách tất cả các lớp học, hỗ trợ phân trang và lọc theo giáo viên chủ nhiệm.
        public async Task<IEnumerable<Classes>> GetAllAsync(int? homeroomTeacherId = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Classes.AsQueryable();
            
            if (homeroomTeacherId.HasValue)
            {
                query = query.Where(c => c.HomeroomTeacherId == homeroomTeacherId);
            }
            
            return await query
                .OrderBy(c => c.Grade)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        
        /// Lấy danh sách các lớp học do một giáo viên cụ thể làm chủ nhiệm, hỗ trợ phân trang.
        
        public async Task<IEnumerable<Classes>> GetByHomeroomTeacherIdAsync(int homeroomTeacherId, int page = 1, int pageSize = 10)
        {
            return await _context.Classes
                .Where(c => c.HomeroomTeacherId == homeroomTeacherId)
                .OrderBy(c => c.Grade)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        
        /// Thêm một lớp học mới vào cơ sở dữ liệu.
        
        public async Task<Classes> CreateAsync(Classes entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Classes.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        
        /// Cập nhật thông tin của một lớp học hiện có.
        
        public async Task<Classes> UpdateAsync(Classes entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Classes.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        
        /// Xóa một lớp học dựa trên ID, đảm bảo chỉ giáo viên chủ nhiệm mới có quyền xóa.
        
        public async Task<bool> DeleteAsync(int id, int homeroomTeacherId)
        {
            var entity = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.HomeroomTeacherId == homeroomTeacherId);
                
            if (entity == null) return false;
            
            _context.Classes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        
        /// Đếm tổng số lớp học, có thể lọc theo ID của giáo viên chủ nhiệm.
        
        public async Task<int> GetTotalCountAsync(int? homeroomTeacherId = null)
        {
            var query = _context.Classes.AsQueryable();
            
            if (homeroomTeacherId.HasValue)
            {
                query = query.Where(c => c.HomeroomTeacherId == homeroomTeacherId);
            }
            
            return await query.CountAsync();
        }
    }
}