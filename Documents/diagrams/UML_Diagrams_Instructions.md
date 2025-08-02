# HƯỚNG DẪN VẼ UML DIAGRAMS

**Hạn chót**: 5/8 | **Xem lại**: 6/8 | **Công cụ**: draw.io

---

## 📁 CẤU TRÚC FILE

**Tên file**: `XDPMHDT.drawio`
**Link**: <https://drive.google.com/file/d/1GjMuz0on3FUFTG1JTo5xbxhi-zxCrns-/view?usp=sharing>
**Link vẽ sequence diagram**: <https://sequencediagram.org/>
**14 Pages trong file**:

```
Page 1: UC01_TongQuan
Page 2: UC02_Admin  
Page 3: UC03_Manager
Page 4: UC04_Staff
Page 5: UC05_Teacher
Page 6: SEQ01_DangNhap
Page 7: SEQ02_ChamDiem_OCR
Page 8: SEQ03_TaoGiaoAn
Page 9: SEQ04_QuanLyCauHoi
Page 10: ACT01_DangKy
Page 11: ACT02_TaoDeThi
Page 12: ACT03_XuLy_OCR
Page 13: CLASS01_DomainModel
Page 14: CLASS02_DatabaseModel
```

---

## 🎯 CÁC LOẠI SƠ ĐỒ CẦN VẼ

### **SƠ ĐỒ USE CASE (Pages 1-5)**

#### Dữ liệu tham khảo

**Các vai trò chính (4 vai trò)**:

- **Admin**: Quản lý người dùng, cấu hình hệ thống, quản lý khung chương trình, theo dõi doanh thu
- **Manager**: Quản lý gói dịch vụ, quản lý đơn hàng, phê duyệt nội dung
- **Staff**: Tạo giáo án mẫu, xây dựng ngân hàng câu hỏi, quản lý mẫu prompt AI
- **Teacher**: Tạo giáo án & đề thi, sử dụng OCR, chấm điểm & phản hồi, xem kết quả học sinh

#### UC01_TongQuan Yêu cầu

- 4 actors ở 4 góc
- Use cases chính ở giữa: "Quản lý Người dùng", "Quản lý Nội dung", "Tạo & Chấm Đề thi", "Quản lý Gói dịch vụ"
- Relationships: include, extend nếu cần
- System boundary rõ ràng

#### UC02_Admin Yêu cầu

- Actor: Admin (bên trái)
- Use cases:
  - Tạo tài khoản người dùng
  - Cập nhật thông tin người dùng  
  - Quản lý vai trò người dùng
  - Cấu hình hệ thống
  - Thiết kế mẫu giáo án
  - Xem báo cáo doanh thu
- Include relationships với "Xác thực" use case

#### UC03_Manager Yêu cầu

- Actor: Manager
- Use cases:
  - Tạo gói dịch vụ
  - Quản lý gói đăng ký
  - Xem đơn hàng khách hàng
  - Theo dõi đăng ký
  - Xem xét nội dung Staff
  - Phê duyệt nội dung

#### UC04_Staff Yêu cầu

- Actor: Staff  
- Use cases:
  - Tạo giáo án mẫu
  - Xây dựng ngân hàng câu hỏi
  - Tạo mẫu prompt AI
  - Cập nhật mẫu prompt AI
  - Xóa mẫu prompt AI

#### UC05_Teacher Yêu cầu

- Actor: Teacher
- Use cases:
  - Tạo giáo án cá nhân
  - Tạo nội dung đề thi
  - Quét tài liệu OCR
  - Chấm điểm tự động
  - Cung cấp phản hồi
  - Xem tiến độ học sinh
  - Phân tích kết quả
  - Quản lý không gian làm việc

---

### **SƠ ĐỒ SEQUENCE (Pages 6-9)**

#### SEQ01_DangNhap Yêu cầu

**Participants**: User, Frontend, Gateway, auth-service, Database
**Flow**:

1. User → Frontend: Nhập email/password
2. Frontend → Gateway: POST /api/auth/login
3. Gateway → auth-service: Forward request
4. auth-service → Database: Kiểm tra credentials
5. Database → auth-service: Return user data
6. auth-service → Gateway: Generate JWT token
7. Gateway → Frontend: Return token + user info
8. Frontend → User: Hiển thị dashboard

**Alt flows**:

- Invalid credentials
- Database connection error

#### SEQ02_ChamDiem_OCR Yêu cầu

**Participants**: Teacher, Frontend, Gateway, task-service, Google Vision API, AWS S3, Database
**Flow**:

1. Teacher → Frontend: Upload ảnh bài làm
2. Frontend → Gateway: POST /api/tasks/ocr-grading
3. Gateway → task-service: Forward file
4. task-service → AWS S3: Upload ảnh
5. task-service → Google Vision API: OCR processing
6. Google Vision API → task-service: Return text data
7. task-service → task-service: Extract answers & student info
8. task-service → Database: Save results
9. task-service → Gateway: Return grading results
10. Gateway → Frontend: Display results
11. Frontend → Teacher: Show score & feedback

#### SEQ03_TaoGiaoAn Yêu cầu

**Participants**: Teacher, Frontend, Gateway, plan-service, Gemini AI, Database
**Flow**:

1. Teacher → Frontend: Nhập thông tin giáo án (môn, chủ đề, mục tiêu)
2. Frontend → Gateway: POST /api/plans/create
3. Gateway → plan-service: Forward request
4. plan-service → Gemini AI: Generate content với prompt
5. Gemini AI → plan-service: Return generated content
6. plan-service → Database: Save draft lesson plan
7. plan-service → Gateway: Return draft
8. Gateway → Frontend: Display preview
9. Frontend → Teacher: Show editable content
10. Teacher → Frontend: Edit & save
11. Frontend → Gateway: PUT /api/plans/{id}
12. Gateway → plan-service: Update lesson plan
13. plan-service → Database: Save final version

#### SEQ04_QuanLyCauHoi Yêu cầu

**Participants**: Staff/Teacher, Frontend, Gateway, plan-service, Database
**Flow**:

1. User → Frontend: Access question bank
2. Frontend → Gateway: GET /api/questions
3. Gateway → plan-service: Forward request  
4. plan-service → Database: Query questions
5. Database → plan-service: Return question list
6. plan-service → Gateway: Return formatted data
7. Gateway → Frontend: Display questions
8. Frontend → User: Show question bank interface

**Add Question Flow**:
9. User → Frontend: Add new question
10. Frontend → Gateway: POST /api/questions
11. Gateway → plan-service: Validate & save
12. plan-service → Database: Insert question
13. Database → plan-service: Confirm insert
14. plan-service → Gateway: Return success
15. Gateway → Frontend: Update UI
16. Frontend → User: Show success message

---

### **SƠ ĐỒ ACTIVITY (Pages 10-12)**

#### ACT01_DangKy Yêu cầu

**Start**: Khách truy cập website
**Activities**:

1. Xem thông tin gói dịch vụ
2. Chọn gói phù hợp
3. Điền form đăng ký (email, password, họ tên, vai trò mong muốn)
4. **Decision**: Thông tin hợp lệ?
   - No → Hiển thị lỗi → Quay lại bước 3
   - Yes → Tiếp tục
5. Gửi email xác thực
6. **Decision**: Đã xác thực email?
   - No → Chờ xác thực
   - Yes → Tiếp tục
7. Admin phê duyệt tài khoản (nếu cần)
8. **Decision**: Được phê duyệt?
   - No → Thông báo từ chối
   - Yes → Kích hoạt tài khoản
9. Gửi email chào mừng
**End**: Tài khoản được tạo thành công

#### ACT02_TaoDeThi Yêu cầu

**Start**: Teacher đăng nhập
**Activities**:

1. Truy cập chức năng tạo đề thi
2. Chọn thông số đề thi:
   - Môn học (Hóa học)
   - Chủ đề
   - Cấp lớp (10, 11, 12)
   - Số câu hỏi
   - Thời gian làm bài
3. **Decision**: Tạo thủ công hay tự động?
   - Thủ công → Chọn từng câu hỏi từ ngân hàng
   - Tự động → Hệ thống chọn ngẫu nhiên theo tiêu chí
4. Xem trước đề thi
5. **Decision**: Hài lòng với đề thi?
   - No → Chỉnh sửa câu hỏi → Quay lại bước 4
   - Yes → Tiếp tục
6. Thiết lập cấu hình:
   - Xáo trộn câu hỏi
   - Tạo nhiều phiên bản
   - Hướng dẫn làm bài
7. Lưu và xuất bản đề thi
8. **Parallel**:
   - Tạo file PDF để in
   - Tạo đáp án chuẩn
   - Cập nhật ngân hàng đề thi
**End**: Đề thi sẵn sàng sử dụng

#### ACT03_XuLy_OCR Yêu cầu

**Start**: Teacher có bài làm của học sinh
**Activities**:

1. Chụp ảnh/scan bài làm
2. Upload ảnh lên hệ thống
3. **Decision**: Chất lượng ảnh tốt?
   - No → Thông báo lỗi → Quay lại bước 1
   - Yes → Tiếp tục
4. **Parallel Processing**:
   - **Branch 1**: OCR nhận dạng thông tin học sinh
     - Trích xuất họ tên
     - Trích xuất MSSV/mã số
     - **Decision**: Tìm thấy thông tin?
       - No → Yêu cầu nhập thủ công
       - Yes → Lưu thông tin
   - **Branch 2**: OCR nhận dạng đáp án
     - Phát hiện vùng đáp án A, B, C, D
     - Nhận dạng lựa chọn đã chọn
     - **Decision**: Nhận dạng thành công?
       - No → Đánh dấu cần review thủ công
       - Yes → Lưu đáp án
5. **Merge**: Kết hợp thông tin học sinh + đáp án
6. So sánh với đáp án chuẩn
7. Tính điểm tự động
8. **Decision**: Có câu cần review thủ công?
   - Yes → Hiển thị cho teacher xem lại
   - No → Tiếp tục
9. Lưu kết quả vào database
10. Tạo báo cáo chi tiết
11. **Parallel**:
    - Cập nhật điểm trung bình học sinh
    - Tạo thống kê lớp
    - Gửi thông báo kết quả
**End**: Hoàn thành chấm điểm

---

### **SƠ ĐỒ CLASS (Pages 13-14)**

#### CLASS01_DomainModel Yêu cầu

**Core Classes** (với attributes và methods chính):

**NguoiDung**:

- Attributes: id, email, matKhauMaHoa, hoTen, vaiTro, trangThaiHoatDong
- Methods: dangNhap(), capNhatThongTin(), vohieuHoa()

**GiaoAn**:

- Attributes: id, tieuDe, mucTieu, noiDung, giaoVienId, trangThai
- Methods: taoTuMau(), capNhatNoiDung(), pheduyet()

**CauHoi**:

- Attributes: id, noiDung, dapAnA, dapAnB, dapAnC, dapAnD, dapAnDung, mucDoKho
- Methods: kiemTraDapAn(), capNhatNoiDung()

**DeThi**:

- Attributes: id, tieuDe, monHoc, thoiGianLam, tongDiem
- Methods: themCauHoi(), taoNgauNhien(), chamDiem()

**HocSinh**:

- Attributes: id, hoTen, maSo, giaoVienId, diemTrungBinh
- Methods: capNhatKetQua(), tinhProficiency()

**KetQua**:

- Attributes: id, hocSinhId, deThiId, diem, soCauDung, chiTietDapAn
- Methods: tinhDiem(), taobaoCao()

**Relationships**:

- NguoiDung (1) → (0..*) GiaoAn
- NguoiDung (1) → (0..*) CauHoi  
- NguoiDung (1) → (0..*) DeThi
- NguoiDung (1) → (0..*) HocSinh
- DeThi (1) → (0..*) CauHoi (many-to-many qua CauHoiTrongDeThi)
- HocSinh (1) → (0..*) KetQua
- DeThi (1) → (0..*) KetQua

#### CLASS02_DatabaseModel Yêu cầu

**Database Entities** (theo ERD trong DDD):

**NGUOI_DUNG**:

- id: bigint (PK)
- email: varchar (UK)
- mat_khau_ma_hoa: varchar
- ho_ten: varchar
- vai_tro: enum
- trang_thai_hoat_dong: boolean
- thoi_gian_tao: timestamp
- thoi_gian_cap_nhat: timestamp

**HOC_SINH**:

- id: bigint (PK)
- ho_ten: varchar
- ma_so_hoc_sinh: varchar
- lop: varchar
- giao_vien_id: bigint (FK)
- diem_trung_binh: double
- so_lan_thi: int
- thoi_gian_tao: timestamp

**GIAO_AN**:

- id: bigint (PK)
- tieu_de: varchar
- muc_tieu: text
- noi_dung: text
- hoat_dong: text
- danh_gia: text
- giao_vien_id: bigint (FK)
- mau_giao_an_id: bigint (FK)
- mon_hoc: varchar
- lop: varchar
- thoi_gian_day: int
- trang_thai: enum
- thoi_gian_tao: timestamp

**CAU_HOI**:

- id: bigint (PK)
- noi_dung_cau_hoi: text
- dap_an_a: text
- dap_an_b: text
- dap_an_c: text
- dap_an_d: text
- dap_an_dung: char
- mon_hoc: varchar
- chu_de: varchar
- muc_do_kho: enum
- nguoi_tao_id: bigint (FK)
- thoi_gian_tao: timestamp

**DE_THI**:

- id: bigint (PK)
- tieu_de: varchar
- mon_hoc: varchar
- lop: varchar
- thoi_gian_lam: int
- tong_diem: int
- giao_vien_id: bigint (FK)
- huong_dan_lam_bai: text
- trang_thai: enum
- thoi_gian_tao: timestamp

**CAU_HOI_TRONG_DE_THI**:

- id: bigint (PK)
- de_thi_id: bigint (FK)
- cau_hoi_id: bigint (FK)
- thu_tu: int
- diem: int

**KET_QUA**:

- id: bigint (PK)
- hoc_sinh_id: bigint (FK)
- de_thi_id: bigint (FK)
- diem: double
- so_cau_dung: int
- tong_so_cau: int
- chi_tiet_dap_an: text
- duong_dan_bai_lam: varchar
- trang_thai_cham: enum
- thoi_gian_nop: timestamp
- thoi_gian_cham: timestamp

**Foreign Key Relationships**:

- NGUOI_DUNG ← HOC_SINH (giao_vien_id)
- NGUOI_DUNG ← GIAO_AN (giao_vien_id)
- NGUOI_DUNG ← CAU_HOI (nguoi_tao_id)
- NGUOI_DUNG ← DE_THI (giao_vien_id)
- DE_THI ← CAU_HOI_TRONG_DE_THI (de_thi_id)
- CAU_HOI ← CAU_HOI_TRONG_DE_THI (cau_hoi_id)
- HOC_SINH ← KET_QUA (hoc_sinh_id)
- DE_THI ← KET_QUA (de_thi_id)

---

## 🎨 HƯỚNG DẪN VẼ

- **Màu sắc**: Actors (xanh lá), Use Cases (xanh dương), Classes (cam), Activities (tím)
- **Font**: Arial 10pt, nhãn bằng tiếng Việt

---

## 📚 TÀI LIỆU THAM KHẢO

- `Documents/SRS-ver2.docx` - Yêu cầu hệ thống
- `Documents/DDD.md` - Chi tiết kỹ thuật
- `Documents/DETAIL.md` - Chi tiết kỹ thuật

---

**Có thắc mắc? Liên hệ nhóm trưởng.**
