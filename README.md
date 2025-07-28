# Tổng quan Dự án: PlanbookAI

## 1. Giới thiệu chung

- **Tên dự án**: PlanbookAI
- **Tên tiếng Việt**: Cổng công cụ AI dành cho giáo viên trung học phổ thông
- **Tên tiếng Anh**: AI Tools Portal for High School Teachers
- **Loại dự án**: Đồ án capstone, triển khai một lần, không yêu cầu high availability
- **Thời gian thực hiện**: 8 tuần (tối thiểu 400 giờ), nhóm 4-5 người
- **Vai trò chính**: Project Manager

## 2. Mục tiêu

- Hỗ trợ giáo viên trung học phổ thông, đặc biệt là môn Hóa học, giảm tải công việc thủ công lặp lại.
- Cung cấp một nền tảng AI giúp soạn bài, tạo đề kiểm tra, chấm bài bằng OCR và phân tích kết quả học tập.
- Tạo kho công cụ và nội dung giảng dạy có thể mở rộng cho nhiều môn học trong tương lai.

## 3. Tính năng chính

- **Quản lý ngân hàng câu hỏi**
- **Tạo bài tập tự động theo chủ đề, cấp độ**
- **Sinh đề trắc nghiệm tùy chỉnh**
- **Chấm điểm bằng OCR**
- **Phân tích kết quả, thống kê, biểu đồ**

## 4. Đối tượng sử dụng

- **Admin**: Quản lý hệ thống
- **Manager**: Quản lý nội dung & phân công
- **Staff**: Hỗ trợ kỹ thuật, nội dung
- **Teacher**: Người sử dụng công cụ giảng dạy

## 5. Yêu cầu phi chức năng

- **Hiệu năng**: Phản hồi nhanh, chịu tải tốt cho lớp học trung bình (~50 người)
- **Bảo mật**: Xác thực JWT, phân quyền rõ ràng
- **Khả năng mở rộng**: Có thể triển khai thêm môn học, người dùng
- **Tương thích**: Đa trình duyệt, responsive cho laptop và máy tính bảng
- **Bảo trì**: Dễ mở rộng mã nguồn, cấu trúc rõ ràng

## 6. Kiến trúc và công nghệ

- **Kiến trúc**: N-tier, RESTful API
- **Xác thực**: JWT
- **Triển khai**: Docker + AWS
- **Cơ sở dữ liệu**: PostgreSQL (qua Supabase)
- **Frontend**: ReactJS (NextJS)
- **Backend**: Spring Boot
- **AI Service**: Google Gemini (OCR & sinh nội dung)

## 7. Tài liệu cần có (Documentation)

- URD – User Requirements Document
- SRS – Software Requirements Specification
- SAD – Software Architecture Document
- DDD – Detailed Design Document
- Implementation Docs
- Test Documentation
- Installation Guide
- Source Code Docs
- Deployment Package Docs

## 8. Thành phần hệ thống

- **Ứng dụng**:
  - Web-based Admin Portal
  - Manager Dashboard
  - RESTful API Services

- **Dữ liệu**:
  - PostgreSQL (qua Supabase)

- **Hạ tầng**:
  - Docker
  - AWS Cloud
