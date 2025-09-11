using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanService.Models.Entities;

namespace PlanService.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức cơ bản cho việc truy cập dữ liệu của Giáo Án.
    /// Giúp tách biệt logic truy cập dữ liệu với logic nghiệp vụ.
    /// </summary>
    public interface IGiaoAnRepository
    {
        // Basic CRUD Operations
        Task<IEnumerable<GiaoAn>> GetAllAsync();
        Task<GiaoAn?> GetByIdAsync(Guid id);
        Task AddAsync(GiaoAn giaoAn);
        Task UpdateAsync(GiaoAn giaoAn);
        Task DeleteAsync(Guid id);

        // Teacher Isolation 
        Task<IEnumerable<GiaoAn>> GetByTeacherIdAsync(Guid teacherId);
        Task<GiaoAn?> GetByIdAndTeacherIdAsync(Guid id, Guid teacherId);

        // Search & Filter Methods 
        Task<IEnumerable<GiaoAn>> SearchAsync(string keyword, Guid teacherId);
        Task<IEnumerable<GiaoAn>> GetByTeacherIdAndSubjectAsync(Guid teacherId, string monHoc);
        Task<IEnumerable<GiaoAn>> GetByTeacherIdAndGradeAsync(Guid teacherId, int khoi);
        Task<IEnumerable<GiaoAn>> GetByTeacherIdAndStatusAsync(Guid teacherId, string status);
        Task<IEnumerable<GiaoAn>> GetByTeacherIdAndTemplateIdAsync(Guid teacherId, Guid templateId);
        Task<IEnumerable<GiaoAn>> GetByTeacherIdAndTopicAsync(Guid teacherId, Guid topicId);
        // Status Management
        Task<GiaoAn?> UpdateStatusAsync(Guid id, string status);
    }
}