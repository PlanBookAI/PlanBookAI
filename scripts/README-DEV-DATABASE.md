# 🐳 PlanbookAI Development Database Setup

## 📋 Tổng quan

Setup này cung cấp PostgreSQL database local cho team development, không cần Supabase credentials.

## 🚀 Cách sử dụng

### 1. Khởi động Database (SMART SCRIPT)
```powershell
# Script tự động: tạo container nếu chưa có, start nếu đã có, migration tự động
.\scripts\start-dev-db.ps1
```

**Script sẽ tự động:**
- ✅ Kiểm tra Docker Desktop có chạy không
- ✅ Tạo PostgreSQL container nếu chưa có
- ✅ Khởi động container nếu đã tồn tại
- ✅ Chạy migration tạo tables theo DDD
- ✅ Báo trạng thái thành công/thất bại chi tiết
- ✅ Hiển thị thông tin kết nối đầy đủ

### 2. Dừng Database
```powershell
# Dừng container (giữ lại data)
docker stop planbookai-postgres-dev

# Dừng và xóa hoàn toàn (mất data)
docker-compose -f docker-compose.dev.yml down -v
```

## 🔗 Thông tin kết nối

### PostgreSQL Database
- **Host**: `localhost`
- **Port**: `5432`
- **Database**: `planbookai`
- **Username**: `test`
- **Password**: `test123`

### JDBC URL cho Spring Boot
```properties
spring.datasource.url=jdbc:postgresql://localhost:5432/planbookai
spring.datasource.username=test
spring.datasource.password=test123
```

## 📁 Cấu trúc Database (Theo DDD)

### Schemas theo Bounded Contexts:
- `users` - User Management Context (Authentication, Authorization)
- `content` - Educational Content Context (Lesson Plans, Templates)
- `assessment` - Assessment Context (Questions, Exams, Grading)
- `students` - Student Data Context (Student Info, Results)

### Tables chính đã tạo:
- **User Management**: `users`, `roles`, `sessions`
- **Educational Content**: `lesson_plans`, `lesson_templates`
- **Assessment**: `questions`, `exams`, `question_choices`, `exam_questions`
- **Student Data**: `students`, `classes`, `student_results`
- **Test**: `public.kiem_tra_ket_noi` - Để test connection

## 🛠️ Cấu hình Spring Boot

### application-dev.properties
```properties
# Database Development
spring.datasource.url=jdbc:postgresql://localhost:5432/planbookai
spring.datasource.username=test
spring.datasource.password=test123
spring.datasource.driver-class-name=org.postgresql.Driver

# JPA Configuration
spring.jpa.hibernate.ddl-auto=update
spring.jpa.show-sql=true
spring.jpa.properties.hibernate.dialect=org.hibernate.dialect.PostgreSQLDialect
```

### Chọn profile development
```properties
# application.properties
spring.profiles.active=dev
```

## 🔄 Reset Database

Để xóa tất cả data và reset database:
```powershell
docker-compose -f docker-compose.dev.yml down -v
.\scripts\start-dev-db.ps1
```

## 📝 Lưu ý cho Team

1. **Không commit credentials production** vào Git
2. **Chỉ sử dụng setup này cho development**
3. **Production sẽ sử dụng Supabase riêng**
4. **Mọi thành viên có thể sử dụng credentials development này**

## 🐛 Troubleshooting

### Container không start được:
```powershell
# Kiểm tra Docker
docker --version

# Kiểm tra port 5432 có bị chiếm không
netstat -an | findstr :5432

# Xem logs container
docker-compose -f docker-compose.dev.yml logs postgres-dev
```

### Không kết nối được database:
1. Đảm bảo container đang chạy: `docker ps`
2. Kiểm tra port forwarding: `netstat -an | findstr :5432`
3. Test connection: `telnet localhost 5432`