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

```java
// Aggregate Root
public class NguoiDung {
    private NguoiDungId id;
    private Email email;
    private MatKhauMaHoa matKhau;
    private HoTen hoTen;
    private VaiTro vaiTro;
    private TrangThaiHoatDong trangThai;
    
    // Domain behaviors
    public void dangNhap(String matKhauGoc) { }
    public void capNhatThongTin(HoTen hoTenMoi) { }
    public void vohieuHoa() { }
}

// Value Objects
public class Email {
    private final String value;
    // Validation logic
}

public class MatKhauMaHoa {
    private final String hashedValue;
    // Encryption logic
}

// Domain Services
public class DichVuXacThuc {
    public KetQuaXacThuc xacThuc(Email email, String matKhau) { }
}
```

### 3.2 Educational Content Domain

```java
// Aggregate Root
public class GiaoAn {
    private GiaoAnId id;
    private TieuDe tieuDe;
    private MucTieu mucTieu;
    private NoiDung noiDung;
    private NguoiDungId giaoVienId;
    private TrangThaiGiaoAn trangThai;
    
    // Domain behaviors
    public void taoTuMau(MauGiaoAn mau) { }
    public void capNhatNoiDung(NoiDung noiDungMoi) { }
    public void pheduyet() { }
}

// Domain Services
public class DichVuTaoGiaoAn {
    public GiaoAn taoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau) { }
}
```

### 3.3 Assessment Domain

```java
// Aggregate Root
public class DeThi {
    private DeThiId id;
    private TieuDe tieuDe;
    private List<CauHoiTrongDeThi> danhSachCauHoi;
    private NguoiDungId giaoVienId;
    private ThoiGianLamBai thoiGianLam;
    
    // Domain behaviors
    public void themCauHoi(CauHoi cauHoi, Diem diem) { }
    public void taoNgauNhien(TieuChiTaoDe tieuChi) { }
    public KetQua chamDiem(BaiLam baiLam) { }
}

// Aggregate Root
public class CauHoi {
    private CauHoiId id;
    private NoiDungCauHoi noiDung;
    private List<LuaChon> danhSachLuaChon;
    private LuaChonDung dapAnDung;
    private MucDoKho mucDoKho;
    
    // Domain behaviors
    public boolean kiemTraDapAn(LuaChon luaChon) { }
}

// Domain Services
public class DichVuOCR {
    public KetQuaOCR xuLyBaiLam(AnhBaiLam anh) { }
    public ThongTinHocSinh trichXuatThongTin(KetQuaOCR ketQua) { }
}
```

### 3.4 Student Data Domain

```java
// Aggregate Root
public class HocSinh {
    private HocSinhId id;
    private HoTen hoTen;
    private MaSoHocSinh maSo;
    private NguoiDungId giaoVienSoHuuId;
    private List<KetQuaHocTap> lichSuKetQua;
    
    // Domain behaviors
    public void capNhatKetQua(KetQua ketQuaMoi) { }
    public MucDoProficiency tinhMucDoProficiency() { }
}

// Domain Services
public class DichVuImportHocSinh {
    public List<HocSinh> importTuExcel(FileExcel file, NguoiDungId giaoVienId) { }
}
```

## 4. Domain Events

### 4.1 User Domain Events

```java
public class NguoiDungDaDangNhap implements DomainEvent {
    private final NguoiDungId nguoiDungId;
    private final LocalDateTime thoiGianDangNhap;
}

public class NguoiDungDaCapNhatThongTin implements DomainEvent {
    private final NguoiDungId nguoiDungId;
    private final ThongTinCapNhat thongTinMoi;
}
```

### 4.2 Assessment Domain Events

```java
public class DeThiDaTao implements DomainEvent {
    private final DeThiId deThiId;
    private final NguoiDungId giaoVienId;
}

public class BaiLamDaCham implements DomainEvent {
    private final KetQuaId ketQuaId;
    private final HocSinhId hocSinhId;
    private final Diem diem;
}
```

## 5. Domain Services

### 5.1 AI Integration Services

```java
public interface DichVuTichHopAI {
    GiaoAn taoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau);
    List<CauHoi> taoNganHangCauHoi(TieuChiTaoCauHoi tieuChi);
    NoiDungGiaoAn caiThienNoiDung(NoiDungGiaoAn noiDungGoc);
}
```

### 5.2 OCR Processing Services

```java
public interface DichVuXuLyOCR {
    KetQuaOCR nhanDangVanBan(AnhBaiLam anh);
    ThongTinHocSinh trichXuatThongTinHocSinh(KetQuaOCR ketQua);
    List<DapAn> trichXuatDapAn(KetQuaOCR ketQua, DeThi deThi);
}
```

## 6. Repositories (Domain Layer)

```java
public interface NguoiDungRepository {
    Optional<NguoiDung> findById(NguoiDungId id);
    Optional<NguoiDung> findByEmail(Email email);
    void save(NguoiDung nguoiDung);
}

public interface GiaoAnRepository {
    Optional<GiaoAn> findById(GiaoAnId id);
    List<GiaoAn> findByGiaoVienId(NguoiDungId giaoVienId);
    void save(GiaoAn giaoAn);
}

public interface CauHoiRepository {
    Optional<CauHoi> findById(CauHoiId id);
    List<CauHoi> findByTieuChi(TieuChiTimKiem tieuChi);
    void save(CauHoi cauHoi);
}

public interface HocSinhRepository {
    Optional<HocSinh> findById(HocSinhId id);
    List<HocSinh> findByGiaoVienId(NguoiDungId giaoVienId);
    void save(HocSinh hocSinh);
}
```

## 7. Value Objects

### 7.1 Common Value Objects

```java
public class TieuDe {
    private final String value;
    // Validation: không null, độ dài 1-200 ký tự
}

public class NoiDung {
    private final String value;
    // Validation: không null, có thể chứa HTML
}

public class Diem {
    private final double value;
    // Validation: 0.0 <= value <= 10.0
}

public class MucDoKho {
    public enum Level { DE, TRUNG_BINH, KHO, RAT_KHO }
    private final Level level;
}
```

### 7.2 Educational Specific Value Objects

```java
public class MonHoc {
    public enum Type { HOA_HOC, VAT_LY, SINH_HOC, TOAN }
    private final Type type;
}

public class LopHoc {
    private final int khoi; // 10, 11, 12
    private final String tenLop; // A1, B2, etc.
}

public class ThoiGianLamBai {
    private final int phut;
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

## 9. Anti-Corruption Layer

### 9.1 External Service Adapters

```java
// Gemini AI Adapter
public class GeminiAIAdapter implements DichVuTichHopAI {
    private final GeminiClient geminiClient;
    
    @Override
    public GiaoAn taoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau) {
        // Convert domain objects to Gemini API format
        // Call external service
        // Convert response back to domain objects
    }
}

// Google Vision OCR Adapter
public class GoogleVisionAdapter implements DichVuXuLyOCR {
    private final VisionClient visionClient;
    
    @Override
    public KetQuaOCR nhanDangVanBan(AnhBaiLam anh) {
        // Convert domain objects to Google Vision format
        // Process OCR
        // Return domain-specific result
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
│   └── HocSinh
├── Value Objects
│   ├── Email
│   ├── TieuDe
│   ├── Diem
│   └── MucDoKho
├── Domain Services
│   ├── DichVuXacThuc
│   ├── DichVuTaoGiaoAn
│   └── DichVuOCR
├── Domain Events
│   ├── NguoiDungDaDangNhap
│   └── BaiLamDaCham
├── Repositories (Interfaces)
│   ├── NguoiDungRepository
│   ├── GiaoAnRepository
│   └── CauHoiRepository
└── Specifications
    ├── CauHoiSpec
    └── GiaoAnSpec
```

## 11. Ubiquitous Language

### 11.1 Core Terms (Vietnamese)

| Term | Definition |
|------|------------|
| Giáo án | Lesson plan - kế hoạch bài giảng chi tiết |
| Đề thi | Exam - bộ câu hỏi để kiểm tra học sinh |
| Câu hỏi | Question - đơn vị cơ bản trong ngân hàng câu hỏi |
| Bài làm | Answer sheet - bài làm của học sinh |
| Chấm điểm | Grading - quá trình đánh giá kết quả |
| OCR | Optical Character Recognition - nhận dạng ký tự quang học |
| Proficiency | Trình độ - mức độ hiểu biết của học sinh |

### 11.2 Domain Verbs

- `tạo` (create) - tạo mới entity
- `cập nhật` (update) - thay đổi thông tin
- `xóa` (delete) - loại bỏ entity
- `chấm điểm` (grade) - đánh giá bài làm
- `tạo ngẫu nhiên` (randomize) - tạo đề thi ngẫu nhiên
- `nhận dạng` (recognize) - xử lý OCR
- `phân tích` (analyze) - phân tích kết quả học tập

## 12. Implementation Guidelines

### 12.1 Naming Conventions

- **Entities**: PascalCase, Vietnamese names (`NguoiDung`, `GiaoAn`)
- **Value Objects**: PascalCase, descriptive (`TieuDe`, `MucDoKho`)
- **Domain Services**: `DichVu` prefix (`DichVuXacThuc`)
- **Repository Interfaces**: Entity name + `Repository`
- **Domain Events**: Past tense, Vietnamese (`NguoiDungDaTao`)

### 12.2 Package Structure

```
com.planbookai.domain
├── nguoidung/           # User bounded context
├── noidunggiaoduc/      # Educational content context  
├── danhgia/             # Assessment context
├── hocsinh/             # Student data context
├── shared/              # Shared kernel
│   ├── valueobjects/
│   ├── events/
│   └── exceptions/
└── services/            # Domain services
```

### 12.3 Testing Strategy

- **Unit Tests**: Test domain logic in isolation
- **Domain Tests**: Test invariants and business rules
- **Integration Tests**: Test repository implementations
- **Contract Tests**: Test anti-corruption layers

## 13. Evolution Strategy

### 13.1 Phase 1 (MVP - Chemistry Focus)

- Core user management
- Basic lesson plan creation
- Simple question bank
- OCR-based grading

### 13.2 Phase 2 (Expansion)

- Multi-subject support
- Advanced AI features
- Student analytics
- Performance optimization

### 13.3 Phase 3 (Scale)

- Multi-tenant architecture
- Advanced reporting
- Mobile support
- Third-party integrations

---

*Tài liệu này sẽ được cập nhật theo evolution của domain model trong quá trình development.*

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

```java
// Aggregate Root
public class NguoiDung {
    private NguoiDungId id;
    private Email email;
    private MatKhauMaHoa matKhau;
    private HoTen hoTen;
    private VaiTro vaiTro;
    private TrangThaiHoatDong trangThai;
    
    // Domain behaviors
    public void dangNhap(String matKhauGoc) { }
    public void capNhatThongTin(HoTen hoTenMoi) { }
    public void vohieuHoa() { }
}

// Value Objects
public class Email {
    private final String value;
    // Validation logic
}

public class MatKhauMaHoa {
    private final String hashedValue;
    // Encryption logic
}

// Domain Services
public class DichVuXacThuc {
    public KetQuaXacThuc xacThuc(Email email, String matKhau) { }
}
```

### 3.2 Educational Content Domain

```java
// Aggregate Root
public class GiaoAn {
    private GiaoAnId id;
    private TieuDe tieuDe;
    private MucTieu mucTieu;
    private NoiDung noiDung;
    private NguoiDungId giaoVienId;
    private TrangThaiGiaoAn trangThai;
    
    // Domain behaviors
    public void taoTuMau(MauGiaoAn mau) { }
    public void capNhatNoiDung(NoiDung noiDungMoi) { }
    public void pheduyet() { }
}

// Domain Services
public class DichVuTaoGiaoAn {
    public GiaoAn taoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau) { }
}
```

### 3.3 Assessment Domain

```java
// Aggregate Root
public class DeThi {
    private DeThiId id;
    private TieuDe tieuDe;
    private List<CauHoiTrongDeThi> danhSachCauHoi;
    private NguoiDungId giaoVienId;
    private ThoiGianLamBai thoiGianLam;
    
    // Domain behaviors
    public void themCauHoi(CauHoi cauHoi, Diem diem) { }
    public void taoNgauNhien(TieuChiTaoDe tieuChi) { }
    public KetQua chamDiem(BaiLam baiLam) { }
}

// Aggregate Root
public class CauHoi {
    private CauHoiId id;
    private NoiDungCauHoi noiDung;
    private List<LuaChon> danhSachLuaChon;
    private LuaChonDung dapAnDung;
    private MucDoKho mucDoKho;
    
    // Domain behaviors
    public boolean kiemTraDapAn(LuaChon luaChon) { }
}

// Domain Services
public class DichVuOCR {
    public KetQuaOCR xuLyBaiLam(AnhBaiLam anh) { }
    public ThongTinHocSinh trichXuatThongTin(KetQuaOCR ketQua) { }
}
```

### 3.4 Student Data Domain

```java
// Aggregate Root
public class HocSinh {
    private HocSinhId id;
    private HoTen hoTen;
    private MaSoHocSinh maSo;
    private NguoiDungId giaoVienSoHuuId;
    private List<KetQuaHocTap> lichSuKetQua;
    
    // Domain behaviors
    public void capNhatKetQua(KetQua ketQuaMoi) { }
    public MucDoProficiency tinhMucDoProficiency() { }
}

// Domain Services
public class DichVuImportHocSinh {
    public List<HocSinh> importTuExcel(FileExcel file, NguoiDungId giaoVienId) { }
}
```

## 4. Domain Events

### 4.1 User Domain Events

```java
public class NguoiDungDaDangNhap implements DomainEvent {
    private final NguoiDungId nguoiDungId;
    private final LocalDateTime thoiGianDangNhap;
}

public class NguoiDungDaCapNhatThongTin implements DomainEvent {
    private final NguoiDungId nguoiDungId;
    private final ThongTinCapNhat thongTinMoi;
}
```

### 4.2 Assessment Domain Events

```java
public class DeThiDaTao implements DomainEvent {
    private final DeThiId deThiId;
    private final NguoiDungId giaoVienId;
}

public class BaiLamDaCham implements DomainEvent {
    private final KetQuaId ketQuaId;
    private final HocSinhId hocSinhId;
    private final Diem diem;
}
```

## 5. Domain Services

### 5.1 AI Integration Services

```java
public interface DichVuTichHopAI {
    GiaoAn taoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau);
    List<CauHoi> taoNganHangCauHoi(TieuChiTaoCauHoi tieuChi);
    NoiDungGiaoAn caiThienNoiDung(NoiDungGiaoAn noiDungGoc);
}
```

### 5.2 OCR Processing Services

```java
public interface DichVuXuLyOCR {
    KetQuaOCR nhanDangVanBan(AnhBaiLam anh);
    ThongTinHocSinh trichXuatThongTinHocSinh(KetQuaOCR ketQua);
    List<DapAn> trichXuatDapAn(KetQuaOCR ketQua, DeThi deThi);
}
```

## 6. Repositories (Domain Layer)

```java
public interface NguoiDungRepository {
    Optional<NguoiDung> findById(NguoiDungId id);
    Optional<NguoiDung> findByEmail(Email email);
    void save(NguoiDung nguoiDung);
}

public interface GiaoAnRepository {
    Optional<GiaoAn> findById(GiaoAnId id);
    List<GiaoAn> findByGiaoVienId(NguoiDungId giaoVienId);
    void save(GiaoAn giaoAn);
}

public interface CauHoiRepository {
    Optional<CauHoi> findById(CauHoiId id);
    List<CauHoi> findByTieuChi(TieuChiTimKiem tieuChi);
    void save(CauHoi cauHoi);
}

public interface HocSinhRepository {
    Optional<HocSinh> findById(HocSinhId id);
    List<HocSinh> findByGiaoVienId(NguoiDungId giaoVienId);
    void save(HocSinh hocSinh);
}
```

## 7. Value Objects

### 7.1 Common Value Objects

```java
public class TieuDe {
    private final String value;
    // Validation: không null, độ dài 1-200 ký tự
}

public class NoiDung {
    private final String value;
    // Validation: không null, có thể chứa HTML
}

public class Diem {
    private final double value;
    // Validation: 0.0 <= value <= 10.0
}

public class MucDoKho {
    public enum Level { DE, TRUNG_BINH, KHO, RAT_KHO }
    private final Level level;
}
```

### 7.2 Educational Specific Value Objects

```java
public class MonHoc {
    public enum Type { HOA_HOC, VAT_LY, SINH_HOC, TOAN }
    private final Type type;
}

public class LopHoc {
    private final int khoi; // 10, 11, 12
    private final String tenLop; // A1, B2, etc.
}

public class ThoiGianLamBai {
    private final int phut;
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

## 9. Anti-Corruption Layer

### 9.1 External Service Adapters

```java
// Gemini AI Adapter
public class GeminiAIAdapter implements DichVuTichHopAI {
    private final GeminiClient geminiClient;
    
    @Override
    public GiaoAn taoGiaoAnTuAI(YeuCauTaoGiaoAn yeuCau) {
        // Convert domain objects to Gemini API format
        // Call external service
        // Convert response back to domain objects
    }
}

// Google Vision OCR Adapter
public class GoogleVisionAdapter implements DichVuXuLyOCR {
    private final VisionClient visionClient;
    
    @Override
    public KetQuaOCR nhanDangVanBan(AnhBaiLam anh) {
        // Convert domain objects to Google Vision format
        // Process OCR
        // Return domain-specific result
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
│   └── HocSinh
├── Value Objects
│   ├── Email
│   ├── TieuDe
│   ├── Diem
│   └── MucDoKho
├── Domain Services
│   ├── DichVuXacThuc
│   ├── DichVuTaoGiaoAn
│   └── DichVuOCR
├── Domain Events
│   ├── NguoiDungDaDangNhap
│   └── BaiLamDaCham
├── Repositories (Interfaces)
│   ├── NguoiDungRepository
│   ├── GiaoAnRepository
│   └── CauHoiRepository
└── Specifications
    ├── CauHoiSpec
    └── GiaoAnSpec
```

## 11. Ubiquitous Language

### 11.1 Core Terms (Vietnamese)

| Term | Definition |
|------|------------|
| Giáo án | Lesson plan - kế hoạch bài giảng chi tiết |
| Đề thi | Exam - bộ câu hỏi để kiểm tra học sinh |
| Câu hỏi | Question - đơn vị cơ bản trong ngân hàng câu hỏi |
| Bài làm | Answer sheet - bài làm của học sinh |
| Chấm điểm | Grading - quá trình đánh giá kết quả |
| OCR | Optical Character Recognition - nhận dạng ký tự quang học |
| Proficiency | Trình độ - mức độ hiểu biết của học sinh |

### 11.2 Domain Verbs

- `tạo` (create) - tạo mới entity
- `cập nhật` (update) - thay đổi thông tin
- `xóa` (delete) - loại bỏ entity
- `chấm điểm` (grade) - đánh giá bài làm
- `tạo ngẫu nhiên` (randomize) - tạo đề thi ngẫu nhiên
- `nhận dạng` (recognize) - xử lý OCR
- `phân tích` (analyze) - phân tích kết quả học tập

## 12. Implementation Guidelines

### 12.1 Naming Conventions

- **Entities**: PascalCase, Vietnamese names (`NguoiDung`, `GiaoAn`)
- **Value Objects**: PascalCase, descriptive (`TieuDe`, `MucDoKho`)
- **Domain Services**: `DichVu` prefix (`DichVuXacThuc`)
- **Repository Interfaces**: Entity name + `Repository`
- **Domain Events**: Past tense, Vietnamese (`NguoiDungDaTao`)

### 12.2 Package Structure

```
com.planbookai.domain
├── nguoidung/           # User bounded context
├── noidunggiaoduc/      # Educational content context  
├── danhgia/             # Assessment context
├── hocsinh/             # Student data context
├── shared/              # Shared kernel
│   ├── valueobjects/
│   ├── events/
│   └── exceptions/
└── services/            # Domain services
```

### 12.3 Testing Strategy

- **Unit Tests**: Test domain logic in isolation
- **Domain Tests**: Test invariants and business rules
- **Integration Tests**: Test repository implementations
- **Contract Tests**: Test anti-corruption layers

## 13. Evolution Strategy

### 13.1 Phase 1 (MVP - Chemistry Focus)

- Core user management
- Basic lesson plan creation
- Simple question bank
- OCR-based grading

### 13.2 Phase 2 (Expansion)

- Multi-subject support
- Advanced AI features
- Student analytics
- Performance optimization

### 13.3 Phase 3 (Scale)

- Multi-tenant architecture
- Advanced reporting
- Mobile support
- Third-party integrations

---

*Tài liệu này sẽ được cập nhật theo evolution của domain model trong quá trình development.*
