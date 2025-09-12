## Kế hoạch tái thiết ExamService (Plan A)

### Mục tiêu
- Ổn định build .NET 9, chạy qua Gateway 8080, service 8084.
- Tuân chuẩn tiếng Việt không dấu cho route, DTO, service, repository.
- Ánh xạ DB theo scripts/bpa.sql (schema assessment, auth).

### Phạm vi giai đoạn 1
- Giữ API cốt lõi đã có: cau-hoi, de-thi, de-thi-cau-hoi, tao-de-thi, mau-de-thi, thong-ke (stub).
- Middleware: logging, auth header, xử lý lỗi chuẩn.
- Cấu hình: connection string qua env, RabbitMQ để stub (không bắt buộc khi dev).

### Công việc
1) Giữ nguyên cấu hình cổng 8084, test qua Gateway 8080.
2) Chuyển RabbitMQ sang biến môi trường, tắt khi thiếu cấu hình.
3) Rà soát và map Entity/EF theo bpa.sql: assessment.exams, questions, question_choices, exam_questions.
4) Chuẩn hoá DTO/Validators tiếng Việt.
5) Hoàn thiện ThongKeController (placeholder → logic), theo ExamService.txt.
6) Làm sạch files thử nghiệm, kiểm tra leak credentials.

### Ghi chú
- Không build/run tự động; Dev sẽ tự build theo quy trình dự án.
- Route giữ chuẩn /api/v1/… tiếng Việt không dấu.

