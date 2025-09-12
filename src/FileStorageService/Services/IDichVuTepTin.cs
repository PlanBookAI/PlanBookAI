namespace PlanbookAI.FileStorageService.Services
{
    using PlanbookAI.FileStorageService.Models.DTOs;
    using PlanbookAI.FileStorageService.Models.Entities;
    
    public interface IDichVuTepTin
    {
        Task<PhanHoiTepTin> UploadAsync(YeuCauUploadTepTin request, Guid userId);
        Task<TepTin?> GetByIdAsync(Guid id, Guid? userId, bool isAdmin);
        Task<PhanHoiDanhSachTepTin> GetPagedAsync(int page, int size, string? fileType, string? status, Guid? userId, bool isAdmin);
        Task<List<PhanHoiTepTin>> SearchByMetadataAsync(string key, string value, Guid? userId, bool isAdmin);
        Task<bool> SoftDeleteAsync(Guid id, Guid userId, bool isAdmin);
        Task<bool> RestoreAsync(Guid id, Guid userId, bool isAdmin);
        Task<bool> PermanentDeleteAsync(Guid id, bool isAdmin);
        Task<bool> UpdateMetadataAsync(Guid id, Dictionary<string, string> metadata, Guid userId, bool isAdmin);
        bool IsValidMimeType(string mimeType, string? fileType);
        bool IsValidFileSize(long size);
        string ComputeFileHash(Stream stream);
    }
}