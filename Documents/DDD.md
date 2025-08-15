# Domain Driven Design (DDD) - PlanbookAI

## 1. Domain Overview

PlanbookAI là một nền tảng hỗ trợ giáo viên THPT thông qua các công cụ AI chuyên biệt, tập trung vào môn Hóa học trong phạm vi đồ án tốt nghiệp.

## 2. Bounded Contexts

### 2.1 User Management Context (Quản lý người dùng)

**Trách nhiệm**: Xử lý authentication, authorization, và user lifecycle

**Core Domain Objects**:

- `NguoiDung` (User)
- `VaiTro` (Role)
- `PhienDangNhap` (Session)

### 2.2 Educational Content Context (Nội dung giáo dục)

**Trách nhiệm**: Quản lý lesson plans, templates, và educational resources

**Core Domain Objects**:

- `GiaoAn` (Lesson Plan)
- `MauGiaoAn` (Lesson Template)
- `NoiDungGiaoDuc` (Educational Content)

### 2.3 Assessment Context (Đánh giá)

**Trách nhiệm**: Question bank, exam generation, và grading

**Core Domain Objects**:

- `CauHoi` (Question)
- `DeThi` (Exam)
- `KetQua` (Result)
- `BaiLam` (Answer Sheet)

### 2.4 Student Data Context (Dữ liệu học sinh)

**Trách nhiệm**: Student information management (teacher-owned)

**Core Domain Objects**:

- `HocSinh` (Student)
- `LopHoc` (Class)
- `KetQuaHocTap` (Academic Result)

## 3. Domain Models

### 3.1 User Management Domain

```csharp
// Aggregate Root
public class NguoiDung
{
    public NguoiDungId Id { get; private set; }
    public Email Email { get; private set; }
    public MatKhauMaHoa MatKhau { get; private set; }
    public HoTen HoTen { get; private set; }
    public VaiTro VaiTro { get; private set; }
    public TrangThaiHoatDong TrangThai { get; private set; }
    
    // Domain behaviors
    public void DangNhap(string matKhauGoc) { }
    public void CapNhatThongTin(HoTen hoTenMoi) { }
    public void VoHieuHoa() { }
}

// Value Objects
public class Email
{
    public string Value { get; }
    // Validation logic
}

public class MatKhauMaHoa
{
    public string HashedValue { get; }
    // Encryption logic
}

// Domain Services
public class DichVuXacThuc
{
    public KetQuaXacThuc XacThuc(Email email, string matKhau) { }
}
```

### 3.2 Educational Content Domain

```csharp
// Aggregate Root
public class GiaoAn
{
    public GiaoAnId Id { get; private set; }
    public TieuDe TieuDe { get; private set; }
    public MucTieu MucTieu { get; private set; }
    public NoiDung NoiDung { get; private set; }
    public NguoiDungId GiaoVienId { get; private set; }
    public TrangThaiGiaoAn TrangThai { get; private set; }
    
    // Domain behaviors
    public void TaoTuMau(MauGiaoAn mau) { }
    public void CapNhatNoiDung(NoiDung noiDungMoi) { }
    public void PheDuyet() { }
}

// Domain Services
public class DichVuTaoGiaoAn
{
    public GiaoAn TaoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau) { }
}
```

### 3.3 Assessment Domain

```csharp
// Aggregate Root
public class DeThi
{
    public DeThiId Id { get; private set; }
    public TieuDe TieuDe { get; private set; }
    public List<CauHoiTrongDeThi> DanhSachCauHoi { get; private set; }
    public NguoiDungId GiaoVienId { get; private set; }
    public ThoiGianLamBai ThoiGianLam { get; private set; }
    
    // Domain behaviors
    public void ThemCauHoi(CauHoi cauHoi, Diem diem) { }
    public void TaoNgauNhien(TieuChiTaoDe tieuChi) { }
    public KetQua ChamDiem(BaiLam baiLam) { }
}

// Aggregate Root
public class CauHoi
{
    public CauHoiId Id { get; private set; }
    public NoiDungCauHoi NoiDung { get; private set; }
    public List<LuaChon> DanhSachLuaChon { get; private set; }
    public LuaChonDung DapAnDung { get; private set; }
    public MucDoKho MucDoKho { get; private set; }
    
    // Domain behaviors
    public bool KiemTraDapAn(LuaChon luaChon) { }
}

// Domain Services
public class DichVuOCR
{
    public KetQuaOCR XuLyBaiLam(AnhBaiLam anh) { }
    public ThongTinHocSinh TrichXuatThongTin(KetQuaOCR ketQua) { }
}
```

### 3.4 Student Data Domain

```csharp
// Aggregate Root
public class HocSinh
{
    public HocSinhId Id { get; private set; }
    public HoTen HoTen { get; private set; }
    public MaSoHocSinh MaSo { get; private set; }
    public NguoiDungId GiaoVienSoHuuId { get; private set; }
    public List<KetQuaHocTap> LichSuKetQua { get; private set; }
    
    // Domain behaviors
    public void CapNhatKetQua(KetQua ketQuaMoi) { }
    public MucDoProficiency TinhMucDoProficiency() { }
}

// Domain Services
public class DichVuImportHocSinh
{
    public List<HocSinh> ImportTuExcel(FileExcel file, NguoiDungId giaoVienId) { }
}
```

## 4. Domain Events

### 4.1 User Domain Events

```csharp
public class NguoiDungDaDangNhap : IDomainEvent
{
    public NguoiDungId NguoiDungId { get; }
    public DateTime ThoiGianDangNhap { get; }
}

public class NguoiDungDaCapNhatThongTin : IDomainEvent
{
    public NguoiDungId NguoiDungId { get; }
    public ThongTinCapNhat ThongTinMoi { get; }
}
```

### 4.2 Assessment Domain Events

```csharp
public class DeThiDaTao : IDomainEvent
{
    public DeThiId DeThiId { get; }
    public NguoiDungId GiaoVienId { get; }
}

public class BaiLamDaCham : IDomainEvent
{
    public KetQuaId KetQuaId { get; }
    public HocSinhId HocSinhId { get; }
    public Diem Diem { get; }
}
```

## 5. Domain Services

### 5.1 AI Integration Services

```csharp
public interface IDichVuTichHopAI
{
    // Existing methods
    GiaoAn TaoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau);
    List<CauHoi> TaoNganHangCauHoi(TieuChiTaoCauHoi tieuChi);
    NoiDungGiaoAn CaiThienNoiDung(NoiDungGiaoAn noiDungGoc);
    
    // New PDF-based lesson plan generation flow
    Task<PhienGiaoAnAI> TaoGiaoAnTuGiaoTrinhPDF(IFormFile pdfFile, YeuCauTaoGiaoAnTuPDF yeuCau);
    Task<GiaoAnAI> ChinhSuaGiaoAnAI(Guid phienId, YeuCauChinhSuaGiaoAn yeuCau);
    Task<GiaoAn> LuuGiaoAnCuoiCung(Guid phienId, YeuCauLuuGiaoAn yeuCau);
}

// New DTOs for PDF-based workflow
public class YeuCauTaoGiaoAnTuPDF
{
    public string MonHoc { get; set; }
    public int KhoiLop { get; set; }
    public string ChuDe { get; set; }
    public string MucTieu { get; set; }
    public int ThoiGianDay { get; set; } // phút
}

public class PhienGiaoAnAI
{
    public Guid Id { get; set; }
    public string DuongDanPDF { get; set; }
    public string VanBanTrichXuat { get; set; }
    public GiaoAnAI GiaoAnAIGenerated { get; set; }
    public TrangThaiPhien TrangThai { get; set; }
    public DateTime ThoiGianTao { get; set; }
    public DateTime? ThoiGianHoanThanh { get; set; }
}

public class GiaoAnAI
{
    public string TieuDe { get; set; }
    public string MucTieu { get; set; }
    public string NoiDung { get; set; }
    public string ChuChuan { get; set; }
    public string ThietBiDayHoc { get; set; }
    public string HinhThucToChuc { get; set; }
    public string TienTrinhBaiGiang { get; set; }
    public string CuCoBaiGiang { get; set; }
    public string DanhGiaKetQua { get; set; }
}

public enum TrangThaiPhien
{
    DangXuLy,
    DaHoanThanh,
    DaLuu,
    DaHuy
}
```

### 5.2 PDF Processing Services

```csharp
public interface IDichVuXuLyPDF
{
    Task<string> TrichXuatVanBanTuPDF(IFormFile pdfFile);
    Task<string> TrichXuatVanBanTuPDFAsync(string duongDanFile);
    Task<bool> KiemTraChatLuongPDF(IFormFile pdfFile);
}

public class DichVuXuLyPDF : IDichVuXuLyPDF
{
    private readonly ILogger<DichVuXuLyPDF> _logger;
    
    public async Task<string> TrichXuatVanBanTuPDF(IFormFile pdfFile)
    {
        // Implementation using iTextSharp or similar PDF library
        // Extract text content from PDF
        // Handle different PDF formats and quality
        // Return extracted text
    }
    
    public async Task<bool> KiemTraChatLuongPDF(IFormFile pdfFile)
    {
        // Check PDF quality for OCR processing
        // Validate file format, size, and readability
        // Return quality assessment
    }
}
```

### 5.3 Lesson Plan AI Generation Service

```csharp
public interface IDichVuTaoGiaoAnAI
{
    Task<GiaoAnAI> TaoGiaoAnTuVanBanGiaoTrinh(string vanBanGiaoTrinh, YeuCauTaoGiaoAnTuPDF yeuCau);
    Task<GiaoAnAI> ChinhSuaGiaoAnTheoYeuCau(GiaoAnAI giaoAnGoc, string yeuCauChinhSua);
    Task<GiaoAnAI> TaoGiaoAnTheoMau(GiaoAnAI giaoAnGoc, MauGiaoAn mau);
}

public class DichVuTaoGiaoAnAI : IDichVuTaoGiaoAnAI
{
    private readonly IGeminiAIClient _geminiClient;
    private readonly ILogger<DichVuTaoGiaoAnAI> _logger;
    
    public async Task<GiaoAnAI> TaoGiaoAnTuVanBanGiaoTrinh(string vanBanGiaoTrinh, YeuCauTaoGiaoAnTuPDF yeuCau)
    {
        var prompt = XayDungPromptTaoGiaoAn(vanBanGiaoTrinh, yeuCau);
        
        try
        {
            var phanHoi = await _geminiClient.TaoNoiDungAsync(prompt);
            return XuLyPhanHoiAI(phanHoi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo giáo án từ AI");
            throw new AIServiceException("Không thể tạo giáo án từ AI", ex);
        }
    }
    
    private string XayDungPromptTaoGiaoAn(string vanBanGiaoTrinh, YeuCauTaoGiaoAnTuPDF yeuCau)
    {
        return $"""
            Dựa trên giáo trình sau đây, hãy tạo một giáo án hoàn chỉnh:
            
            GIÁO TRÌNH:
            {vanBanGiaoTrinh}
            
            YÊU CẦU:
            - Môn học: {yeuCau.MonHoc}
            - Khối lớp: {yeuCau.KhoiLop}
            - Chủ đề: {yeuCau.ChuDe}
            - Mục tiêu: {yeuCau.MucTieu}
            - Thời gian dạy: {yeuCau.ThoiGianDay} phút
            
            CẤU TRÚC GIÁO ÁN:
            1. Tiêu đề bài giảng
            2. Mục tiêu bài học
            3. Chuẩn kiến thức, kỹ năng
            4. Thiết bị dạy học
            5. Hình thức tổ chức
            6. Tiến trình bài giảng
            7. Củng cố bài giảng
            8. Đánh giá kết quả
            
            Yêu cầu:
            - Ngôn ngữ: Tiếng Việt
            - Theo chương trình giáo dục Việt Nam
            - Phù hợp với khối lớp {yeuCau.KhoiLop}
            - Thời gian phù hợp với {yeuCau.ThoiGianDay} phút
            - Nội dung chi tiết, dễ hiểu
            """;
    }
    
    private GiaoAnAI XuLyPhanHoiAI(string phanHoiAI)
    {
        // Parse AI response and structure into GiaoAnAI object
        // Handle different response formats
        // Extract structured information
        return new GiaoAnAI
        {
            // Parse and populate fields from AI response
        };
    }
}
```

### 5.4 Session Management Service

```csharp
public interface IDichVuQuanLyPhienGiaoAn
{
    Task<PhienGiaoAnAI> TaoPhienMoi(IFormFile pdfFile, YeuCauTaoGiaoAnTuPDF yeuCau);
    Task<PhienGiaoAnAI> LayPhienTheoId(Guid phienId);
    Task<bool> CapNhatTrangThaiPhien(Guid phienId, TrangThaiPhien trangThaiMoi);
    Task<bool> XoaPhien(Guid phienId);
}

public class DichVuQuanLyPhienGiaoAn : IDichVuQuanLyPhienGiaoAn
{
    private readonly IPhienGiaoAnRepository _phienRepository;
    private readonly IDichVuXuLyPDF _pdfService;
    private readonly IDichVuTaoGiaoAnAI _aiService;
    private readonly IFileStorageService _fileStorage;
    
    public async Task<PhienGiaoAnAI> TaoPhienMoi(IFormFile pdfFile, YeuCauTaoGiaoAnTuPDF yeuCau)
    {
        // 1. Validate PDF file
        if (!await _pdfService.KiemTraChatLuongPDF(pdfFile))
        {
            throw new ValidationException("PDF không đủ chất lượng để xử lý");
        }
        
        // 2. Upload PDF to storage
        var duongDanPDF = await _fileStorage.LuuFileAsync(pdfFile, "giao-trinh");
        
        // 3. Extract text from PDF
        var vanBanTrichXuat = await _pdfService.TrichXuatVanBanTuPDF(pdfFile);
        
        // 4. Generate lesson plan using AI
        var giaoAnAI = await _aiService.TaoGiaoAnTuVanBanGiaoTrinh(vanBanTrichXuat, yeuCau);
        
        // 5. Create session
        var phien = new PhienGiaoAnAI
        {
            Id = Guid.NewGuid(),
            DuongDanPDF = duongDanPDF,
            VanBanTrichXuat = vanBanTrichXuat,
            GiaoAnAIGenerated = giaoAnAI,
            TrangThai = TrangThaiPhien.DangXuLy,
            ThoiGianTao = DateTime.UtcNow
        };
        
        await _phienRepository.LuuAsync(phien);
        return phien;
    }
}
```

### 5.5 OCR Processing Services

```csharp
public interface IDichVuXuLyOCR
{
    KetQuaOCR NhanDangVanBan(AnhBaiLam anh);
    ThongTinHocSinh TrichXuatThongTinHocSinh(KetQuaOCR ketQua);
    List<DapAn> TrichXuatDapAn(KetQuaOCR ketQua, DeThi deThi);
}
```

## 6. Repositories (Domain Layer)

```csharp
public interface INguoiDungRepository
{
    Task<NguoiDung?> GetByIdAsync(NguoiDungId id);
    Task<NguoiDung?> GetByEmailAsync(Email email);
    Task SaveAsync(NguoiDung nguoiDung);
}

public interface IGiaoAnRepository
{
    Task<GiaoAn?> GetByIdAsync(GiaoAnId id);
    Task<List<GiaoAn>> GetByGiaoVienIdAsync(NguoiDungId giaoVienId);
    Task SaveAsync(GiaoAn giaoAn);
}

public interface ICauHoiRepository
{
    Task<CauHoi?> GetByIdAsync(CauHoiId id);
    Task<List<CauHoi>> GetByTieuChiAsync(TieuChiTimKiem tieuChi);
    Task SaveAsync(CauHoi cauHoi);
}

public interface IHocSinhRepository
{
    Task<HocSinh?> GetByIdAsync(HocSinhId id);
    Task<List<HocSinh>> GetByGiaoVienIdAsync(NguoiDungId giaoVienId);
    Task SaveAsync(HocSinh hocSinh);
}

// New repository for AI session management
public interface IPhienGiaoAnRepository
{
    Task<PhienGiaoAnAI?> GetByIdAsync(Guid phienId);
    Task<List<PhienGiaoAnAI>> GetByGiaoVienIdAsync(NguoiDungId giaoVienId);
    Task SaveAsync(PhienGiaoAnAI phien);
    Task<bool> DeleteAsync(Guid phienId);
}
```

## 7. Value Objects

### 7.1 Common Value Objects

```csharp
public class TieuDe
{
    public string Value { get; }
    // Validation: không null, độ dài 1-200 ký tự
}

public class NoiDung
{
    public string Value { get; }
    // Validation: không null, có thể chứa HTML
}

public class Diem
{
    public double Value { get; }
    // Validation: 0.0 <= value <= 10.0
}

public class MucDoKho
{
    public enum Level { De, TrungBinh, Kho, RatKho }
    public Level Level { get; }
}
```

### 7.2 Educational Specific Value Objects

```csharp
public class MonHoc
{
    public enum Type { HoaHoc, VatLy, SinhHoc, Toan }
    public Type Type { get; }
}

public class LopHoc
{
    public int Khoi { get; } // 10, 11, 12
    public string TenLop { get; } // A1, B2, etc.
}

public class ThoiGianLamBai
{
    public int Phut { get; }
    // Validation: 15 <= phut <= 180
}
```

## 8. Domain Rules & Invariants

### 8.1 User Domain Rules

- Một email chỉ có thể thuộc về một người dùng
- Mật khẩu phải được mã hóa trước khi lưu trữ
- Chỉ ADMIN có thể tạo tài khoản MANAGER
- TEACHER chỉ có thể truy cập dữ liệu của học sinh mình quản lý

### 8.2 Assessment Domain Rules

- Một đề thi phải có ít nhất 10 câu hỏi
- Tổng điểm của đề thi phải bằng 10.0
- Câu hỏi trắc nghiệm phải có đúng 4 lựa chọn
- Thời gian làm bài tối thiểu 15 phút, tối đa 180 phút

### 8.3 Student Data Rules

- Học sinh chỉ thuộc về một giáo viên
- Mã số học sinh phải unique trong phạm vi một giáo viên
- Kết quả học tập phải được mã hóa (PII protection)

### 8.4 Lesson Plan AI Generation Rules

- PDF phải có chất lượng đủ để trích xuất văn bản
- Session timeout sau 24 giờ nếu không hoàn thành
- Chỉ giáo viên có thể chỉnh sửa giáo án AI-generated
- Giáo án cuối cùng phải được lưu với đầy đủ thông tin bắt buộc

## 9. Anti-Corruption Layer

### 9.1 External Service Adapters

```csharp
// Gemini AI Adapter
public class GeminiAIAdapter : IDichVuTichHopAI
{
    private readonly IGeminiClient _geminiClient;
    
    public async Task<GiaoAn> TaoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau)
    {
        // Convert domain objects to Gemini API format
        // Call external service
        // Convert response back to domain objects
    }
}

// Google Vision OCR Adapter
public class GoogleVisionAdapter : IDichVuXuLyOCR
{
    private readonly IVisionClient _visionClient;
    
    public async Task<KetQuaOCR> NhanDangVanBan(AnhBaiLam anh)
    {
        // Convert domain objects to Google Vision format
        // Process OCR
        // Return domain-specific result
    }
}

// File Storage Adapter
public class FileStorageAdapter : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    
    public async Task<string> LuuFileAsync(IFormFile file, string folder)
    {
        // Upload file to S3 or Azure Blob
        // Return file path/URL
    }
}
```

## 10. Domain Layer Architecture

```
Domain Layer
├── Entities (Aggregate Roots)
│   ├── NguoiDung
│   ├── GiaoAn
│   ├── DeThi
│   ├── CauHoi
│   ├── HocSinh
│   └── PhienGiaoAnAI (NEW)
├── Value Objects
│   ├── Email
│   ├── TieuDe
│   ├── Diem
│   └── MucDoKho
├── Domain Services
│   ├── DichVuXacThuc
│   ├── DichVuTaoGiaoAn
│   ├── DichVuXuLyPDF (NEW)
│   ├── DichVuTaoGiaoAnAI (NEW)
│   ├── DichVuQuanLyPhienGiaoAn (NEW)
│   └── DichVuOCR
├── Domain Events
│   ├── NguoiDungDaDangNhap
│   ├── BaiLamDaCham
│   ├── PhienGiaoAnAIDaTao (NEW)
│   ├── GiaoAnAIDaHoanThanh (NEW)
│   └── GiaoAnDaDuocLuu (NEW)
├── Repositories (Interfaces)
│   ├── INguoiDungRepository
│   ├── IGiaoAnRepository
│   ├── ICauHoiRepository
│   ├── IHocSinhRepository
│   └── IPhienGiaoAnRepository (NEW)
└── Specifications
    ├── CauHoiSpec
    └── GiaoAnSpec
```

## 11. Ubiquitous Language

### 11.1 Core Terms (Vietnamese)

| Term | Definition |
|---|---|
| Giáo án | Lesson plan - kế hoạch bài giảng chi tiết |
| Đề thi | Exam - bộ câu hỏi để kiểm tra học sinh |
| Câu hỏi | Question - đơn vị cơ bản trong ngân hàng câu hỏi |
| Bài làm | Answer sheet - bài làm của học sinh |
| Chấm điểm | Grading - quá trình đánh giá kết quả |
| OCR | Optical Character Recognition - nhận dạng ký tự quang học |
| Proficiency | Trình độ - mức độ hiểu biết của học sinh |
| **PhienGiaoAnAI** | **AI Lesson Plan Session - phiên làm việc tạo giáo án bằng AI** |
| **GiaoTrinhPDF** | **Curriculum PDF - file giáo trình dạng PDF** |

### 11.2 Domain Verbs

- `tạo` (create) - tạo mới entity
- `cập nhật` (update) - thay đổi thông tin
- `xóa` (delete) - loại bỏ entity
- `chấm điểm` (grade) - đánh giá bài làm
- `tạo ngẫu nhiên` (randomize) - tạo đề thi ngẫu nhiên
- `nhận dạng` (recognize) - xử lý OCR
- `phân tích` (analyze) - phân tích kết quả học tập
- **`upload`** - **tải lên file PDF giáo trình**
- **`trich xuat`** - **trích xuất văn bản từ PDF**
- **`tao giao an AI`** - **tạo giáo án bằng AI**
- **`chinh sua`** - **chỉnh sửa giáo án AI-generated**

## 12. Implementation Guidelines

### 12.1 Naming Conventions

- **Entities**: PascalCase, Vietnamese names (`NguoiDung`, `GiaoAn`, `PhienGiaoAnAI`)
- **Value Objects**: PascalCase, descriptive (`TieuDe`, `MucDoKho`)
- **Domain Services**: `DichVu` prefix (`DichVuXacThuc`, `DichVuXuLyPDF`)
- **Repository Interfaces**: `I` + Entity name + `Repository`
- **Domain Events**: Past tense, Vietnamese (`NguoiDungDaTao`, `PhienGiaoAnAIDaTao`)

### 12.2 Package Structure

```
PlanbookAI.Domain
├── NguoiDung/           # User bounded context
├── NoiDungGiaoDuc/      # Educational content context  
│   ├── GiaoAn/          # Lesson plans
│   ├── PhienGiaoAnAI/   # AI session management (NEW)
│   └── PDFProcessing/   # PDF handling (NEW)
├── DanhGia/             # Assessment context
├── HocSinh/             # Student data context
├── Shared/              # Shared kernel
│   ├── ValueObjects/
│   ├── Events/
│   └── Exceptions/
└── Services/            # Domain services
    ├── AI/              # AI integration services
    ├── PDF/             # PDF processing services
    └── OCR/             # OCR services
```

### 12.3 Testing Strategy

- **Unit Tests**: Test domain logic in isolation
- **Domain Tests**: Test invariants and business rules
- **Integration Tests**: Test repository implementations
- **Contract Tests**: Test anti-corruption layers
- **AI Service Tests**: Mock AI responses for testing
- **PDF Processing Tests**: Test with sample PDF files

## 13. Evolution Strategy

### 13.1 Phase 1 (MVP - Chemistry Focus)

- Core user management
- Basic lesson plan creation
- Simple question bank
- OCR-based grading
- **PDF-based AI lesson plan generation (NEW)**

### 13.2 Phase 2 (Expansion)

- Multi-subject support
- Advanced AI features
- Student analytics
- Performance optimization
- **Advanced PDF processing features**
- **AI lesson plan templates**

### 13.3 Phase 3 (Scale)

- Multi-tenant architecture
- Advanced reporting
- Mobile support
- Third-party integrations
- **Multi-language support for AI generation**
- **Advanced AI models integration**

---

*Tài liệu này sẽ được cập nhật theo evolution của domain model trong quá trình development.* 