# 🧠 PlanbookAI - AI Tools Portal for High School Teachers

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.0-blue.svg)](https://reactjs.org/)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://docker.com/)

> **PlanbookAI** là một nền tảng web tích hợp AI nhằm hỗ trợ giáo viên trung học phổ thông tự động hóa các công việc giảng dạy và hành chính, đặc biệt tập trung vào môn Hóa học trong giai đoạn phát triển ban đầu.

## 📋 Mục lục

- [✨ Tính năng chính](#-tính-năng-chính)
- [🏗️ Kiến trúc hệ thống](#️-kiến-trúc-hệ-thống)
- [📚 API Documentation](#-api-documentation)
- [🔒 Security](#-security)
- [🚦 Monitoring & Logging](#-monitoring--logging)
- [📈 Performance](#-performance)

## ✨ Tính năng chính

### 🎯 Dành cho Giáo viên
- **📝 Quản lý Giáo án**: Tạo và tùy chỉnh giáo án với hỗ trợ AI
- **❓ Ngân hàng Câu hỏi**: Quản lý tập trung câu hỏi theo chủ đề và mức độ khó
- **📋 Tạo Đề thi**: Sinh đề trắc nghiệm tự động với thuật toán cân bằng
- **🔍 Chấm bài OCR**: Nhận dạng và chấm điểm tự động từ ảnh bài làm
- **📊 Phân tích Kết quả**: Báo cáo chi tiết về hiệu suất học sinh

### 🛠️ Dành cho Quản trị
- **👤 Quản lý Người dùng**: Phân quyền theo vai trò (RBAC)
- **📈 Giám sát Hệ thống**: Dashboard theo dõi hiệu năng và sử dụng
- **⚙️ Cấu hình**: Tùy chỉnh template và quy tắc nghiệp vụ
- **💰 Quản lý Gói dịch vụ**: Subscription và billing management

## 🏗️ Kiến trúc hệ thống

### 📐 Tổng quan
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Next.js       │    │  API Gateway    │    │  Microservices  │
│   Frontend      │◄──►│  (YARP)         │◄──►│  (.NET Core)    │
│   (Vercel)      │    │  Load Balancer  │    │  Auth/User/Plan │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                ▲                        ▲
                                │                        │
                                ▼                        ▼
                       ┌─────────────────┐    ┌─────────────────┐
                       │  External APIs  │    │   PostgreSQL    │
                       │  Gemini AI      │    │   (Supabase)    │
                       │  Google Vision  │    │   Database      │
                       └─────────────────┘    └─────────────────┘
```

### 🔧 Tech Stack

| Lớp | Công nghệ | Mô tả |
|-----|-----------|--------|
| **Frontend** | Next.js 14, React 18, TypeScript | Server-side rendering, responsive UI |
| **API Gateway** | .NET 9, YARP | Reverse proxy, authentication, rate limiting |
| **Backend Services** | .NET 9, C# 13 | Microservices architecture |
| **Database** | PostgreSQL 15 (Supabase) | Managed database with real-time features |
| **Authentication** | JWT, BCrypt | Token-based auth with role-based access |
| **AI Services** | Gemini AI, Google Vision API | Content generation and OCR processing |
| **Containerization** | Docker, Docker Compose | Development and deployment |
| **Cloud** | AWS (Self-hosted), Vercel | Infrastructure and frontend hosting |

### 🎯 Microservices

| Service | Port | Mô tả | Endpoint |
|---------|------|--------|----------|
| **Gateway** | 8080 | API Gateway, routing, auth | `/api/v1/*` |
| **Auth** | 8081 | Authentication, authorization | `/api/v1/xac-thuc` |
| **User** | 8082 | User management, profiles | `/api/v1/nguoi-dung` |
| **Plan** | 8083 | Lesson plans, templates | `/api/v1/giao-an` |
| **Task** | 8084 | Assignments, grading | `/api/v1/nhiem-vu` |

## 📚 API Documentation

<details>
<summary><strong>🔐 Authentication APIs</strong></summary>

### Đăng nhập
```bash
POST /api/v1/xac-thuc/dang-nhap
Content-Type: application/json

{
  "email": "teacher@example.com",
  "password": "password123"
}

# Response
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "...",
  "user": { ... }
}
```

### Đăng xuất
```bash
POST /api/v1/xac-thuc/dang-xuat
Authorization: Bearer {token}
```

### Làm mới Token
```bash
POST /api/v1/xac-thuc/lam-moi-token
Content-Type: application/json

{
  "refreshToken": "..."
}
```

</details>

<details>
<summary><strong>👤 User Management APIs</strong></summary>

### Lấy thông tin người dùng
```bash
GET /api/v1/nguoi-dung/thong-tin
Authorization: Bearer {token}
```

### Cập nhật profile
```bash
PUT /api/v1/nguoi-dung/cap-nhat
Authorization: Bearer {token}
Content-Type: application/json

{
  "hoTen": "Nguyễn Văn A",
  "soDienThoai": "0123456789"
}
```

</details>

<details>
<summary><strong>📝 Lesson Plan APIs</strong></summary>

### Tạo giáo án
```bash
POST /api/v1/giao-an/tao
Authorization: Bearer {token}
Content-Type: application/json

{
  "tieuDe": "Bài 1: Cấu trúc nguyên tử",
  "monHoc": "HoaHoc",
  "lop": 10
}
```

### Lấy danh sách giáo án
```bash
GET /api/v1/giao-an/danh-sach
Authorization: Bearer {token}
```

</details>

<details>
<summary><strong>❓ Question Bank APIs</strong></summary>

### Tạo câu hỏi
```bash
POST /api/v1/cau-hoi/tao
Authorization: Bearer {token}
Content-Type: application/json

{
  "noiDung": "Nguyên tử có cấu trúc như thế nào?",
  "monHoc": "HoaHoc",
  "chuDe": "CauTrucNguyenTu",
  "mucDoKho": "TrungBinh"
}
```

### Tìm kiếm câu hỏi
```bash
GET /api/v1/cau-hoi/tim-kiem?monHoc=HoaHoc&chuDe=CauTrucNguyenTu
Authorization: Bearer {token}
```

</details>

<details>
<summary><strong>📋 Exam APIs</strong></summary>

### Tạo đề thi
```bash
POST /api/v1/de-thi/tao
Authorization: Bearer {token}
Content-Type: application/json

{
  "tieuDe": "Đề kiểm tra 15 phút",
  "soCauHoi": 20,
  "thoiGianLam": 15,
  "monHoc": "HoaHoc"
}
```

### Chấm bài OCR
```bash
POST /api/v1/de-thi/cham-bai
Authorization: Bearer {token}
Content-Type: multipart/form-data

# Upload file ảnh bài làm
```

</details>

<details>
<summary><strong>📊 Monitoring APIs</strong></summary>

### Thông tin hệ thống
```bash
GET /api/v1/giam-sat/thong-tin-he-thong
Authorization: Bearer {token}
```

### Thống kê hiệu năng
```bash
GET /api/v1/giam-sat/thong-ke
Authorization: Bearer {token}
```

### Kiểm tra sức khỏe services
```bash
GET /api/v1/giam-sat/kiem-tra-dich-vu
Authorization: Bearer {token}
```

</details>

## 🔒 Security

- **JWT Authentication** với refresh tokens
- **Role-based Access Control** (RBAC)
- **Rate Limiting**: 100 requests/phút
- **CORS** configuration cho frontend
- **Data Encryption** cho thông tin nhạy cảm
- **Input Validation** với FluentValidation

## 🚦 Monitoring & Logging

- **Health Checks**: Tự động kiểm tra tình trạng services
- **Structured Logging**: Serilog với JSON format
- **Performance Metrics**: Response time, throughput
- **Error Tracking**: Centralized error handling

## 📈 Performance

- **Response Time**: < 2s cho hầu hết operations
- **OCR Processing**: < 30s per document
- **Concurrent Users**: 100+ teachers simultaneously
- **Uptime**: 99.5% availability target

## 👥 Development Team

### 🎯 Team Members

- **Nguyễn Vương Minh Khôi** (22H1120108) - Project/Team Lead & Technical Architect & DevOps & Developer
- **Phạm Văn Phi Long** (068205003905) - Developer
- **Lê Tùng Lâm** (2251120302) - Developer
- **Trần Khắc Quân** (2251120439) - Developer
- **Ngô Đình Quốc Thịnh** (2251120443) - Developer
- **Nguyễn Trọng Kim** (2251120299) - Developer

### 📧 Contact Information

#### [Trường Đại Học Giao Thông Vận Tải TPHCM Khoa CNTT](https://it3e.ut.edu.vn/)

- **Khôi**: 22H1120108@ut.edu.vn
- **Long**: longpvp3905@ut.edu.vn
- **Lâm**: 2251120302@ut.edu.vn
- **Quân**: 2251120439@ut.edu.vn
- **Thịnh**: 2251120443@ut.edu.vn
- **Kim**: 2251120299@ut.edu.vn

## 🎓 Academic Supervisor

**MSc. Nguyễn Văn Chiến** - Faculty Advisor

## 📄 License

Dự án này được phát hành dưới [MIT License](LICENSE).
