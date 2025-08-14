# PlanbookAI Database Migration

## Tổng quan

Thư mục này chứa các script để thiết lập và migrate database cho PlanbookAI project.

## Files

### 1. `complete-migration.sql`
- **Mô tả**: File SQL migration hoàn chỉnh được generate từ tất cả các Entity Framework DbContext
- **Nội dung**: 
  - Tạo 4 schemas: `users`, `content`, `assessment`, `students`
  - Tạo tất cả tables với relationships đầy đủ
  - Tạo indexes cho performance
  - Tạo triggers cho `updated_at` columns
  - Insert seed data (roles cơ bản)
  - Grant permissions cho user `test`

### 2. `run-complete-migration.ps1`
- **Mô tả**: PowerShell script để chạy migration hoàn chỉnh
- **Chức năng**:
  - Kiểm tra database container
  - Chạy file `complete-migration.sql`
  - Verify kết quả migration
  - Hiển thị báo cáo chi tiết

### 3. `db-migration.ps1` (Legacy)
- **Mô tả**: Script migration cũ (không còn sử dụng)
- **Lý do**: Chỉ tạo structure, không có seed data

## Cách sử dụng

### Bước 1: Start database
```powershell
.\start-dev-db.ps1
```

### Bước 2: Chạy migration hoàn chỉnh
```powershell
.\run-complete-migration.ps1
```

## Database Structure

### Schema: `users`
- **roles**: Vai trò người dùng (ADMIN, MANAGER, STAFF, TEACHER)
- **users**: Thông tin người dùng cơ bản
- **sessions**: Phiên đăng nhập
- **user_profiles**: Hồ sơ chi tiết người dùng
- **activity_history**: Lịch sử hoạt động

### Schema: `content`
- **lesson_templates**: Mẫu giáo án
- **lesson_plans**: Kế hoạch bài giảng
- **educational_content**: Nội dung giáo dục
- **learning_objectives**: Mục tiêu học tập

### Schema: `assessment`
- **questions**: Câu hỏi
- **question_choices**: Lựa chọn câu trả lời
- **exams**: Đề thi
- **exam_questions**: Câu hỏi trong đề thi

### Schema: `students`
- **classes**: Lớp học
- **students**: Học sinh
- **student_results**: Kết quả học tập
- **answer_sheets**: Bài làm của học sinh

## Relationships

### Users
- `users.role_id` → `users.roles.id`
- `users.sessions.user_id` → `users.users.id`
- `users.user_profiles.user_id` → `users.users.id`
- `users.activity_history.user_id` → `users.users.id`

### Content
- `content.lesson_plans.created_by` → `users.users.id`
- `content.lesson_plans.template_id` → `content.lesson_templates.id`
- `content.educational_content.lesson_plan_id` → `content.lesson_plans.id`
- `content.learning_objectives.lesson_plan_id` → `content.lesson_plans.id`

### Assessment
- `assessment.questions.created_by` → `users.users.id`
- `assessment.question_choices.question_id` → `assessment.questions.id`
- `assessment.exams.created_by` → `users.users.id`
- `assessment.exam_questions.exam_id` → `assessment.exams.id`
- `assessment.exam_questions.question_id` → `assessment.questions.id`

### Students
- `students.classes.teacher_id` → `users.users.id`
- `students.students.class_id` → `students.classes.id`
- `students.students.teacher_id` → `users.users.id`
- `students.student_results.student_id` → `students.students.id`
- `students.student_results.exam_id` → `assessment.exams.id`
- `students.student_results.grader_id` → `users.users.id`
- `students.answer_sheets.student_id` → `students.students.id`
- `students.answer_sheets.exam_id` → `assessment.exams.id`

## Indexes

Mỗi schema có các indexes tối ưu cho:
- **Primary keys**: Tự động tạo
- **Foreign keys**: Để tăng tốc JOIN operations
- **Search fields**: Email, tên, subject, topic
- **Date fields**: created_at, updated_at

## Triggers

Tất cả tables có `updated_at` column đều có trigger tự động cập nhật timestamp khi UPDATE.

## Seed Data

### Roles
1. **ADMIN** - Quản trị viên hệ thống
2. **MANAGER** - Quản lý nội dung và người dùng  
3. **STAFF** - Nhân viên tạo nội dung mẫu
4. **TEACHER** - Giáo viên sử dụng hệ thống

## Permissions

User `test` có tất cả quyền trên:
- Tất cả tables trong 4 schemas
- Tất cả sequences
- Tất cả functions và triggers

## Troubleshooting

### Lỗi thường gặp

1. **Container không chạy**
   ```powershell
   docker ps | findstr postgres
   ```

2. **Permission denied**
   ```powershell
   docker exec planbookai-postgres-dev psql -U test -d planbookai -c "SELECT current_user;"
   ```

3. **Schema không tồn tại**
   ```powershell
   docker exec planbookai-postgres-dev psql -U test -d planbookai -c "\dn"
   ```

### Reset database

Nếu cần reset hoàn toàn:
```powershell
docker exec planbookai-postgres-dev psql -U test -d postgres -c "DROP DATABASE IF EXISTS planbookai;"
.\run-complete-migration.ps1
```

## Notes

- Migration script sử dụng `IF NOT EXISTS` để tránh lỗi duplicate
- Seed data sử dụng `ON CONFLICT DO NOTHING` để tránh lỗi duplicate
- Tất cả timestamps sử dụng `CURRENT_TIMESTAMP`
- UUID sử dụng `gen_random_uuid()` function của PostgreSQL
