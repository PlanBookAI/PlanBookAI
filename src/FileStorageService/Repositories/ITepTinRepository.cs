namespace PlanbookAI.FileStorageService.Repositories
{
    using PlanbookAI.FileStorageService.Models.Entities;
    
    public interface ITepTinRepository
    {
        Task<TepTin?> GetByIdAsync(Guid id);
        Task<TepTin?> GetByHashAndUserAsync(string hash, Guid userId, string originalName);
        Task<(List<TepTin> files, int total)> GetPagedAsync(int page, int size, string? fileType, string? status, Guid? userId);
        Task<List<TepTin>> SearchByMetadataAsync(string key, string value, Guid? userId);
        Task<TepTin> CreateAsync(TepTin tepTin);
        Task UpdateAsync(TepTin tepTin);
        Task DeleteAsync(Guid id);
        Task<List<TepTinMetadata>> GetMetadataAsync(Guid fileId);
        Task UpsertMetadataAsync(Guid fileId, string key, string value);
        Task DeleteMetadataAsync(Guid fileId, string key);
    }
}