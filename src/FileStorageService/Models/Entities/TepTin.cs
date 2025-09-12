namespace PlanbookAI.FileStorageService.Models.Entities
{
    public class TepTin
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public Guid UploadedBy { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string Status { get; set; } = "ACTIVE";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<TepTinMetadata> Metadata { get; set; } = new List<TepTinMetadata>();
    }
    
    public class TepTinMetadata
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public TepTin File { get; set; } = null!;
    }
}