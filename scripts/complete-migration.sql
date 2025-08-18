-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 1: DATABASE & SCHEMAS
-- =====================================================

-- Tạo database
CREATE DATABASE planbookai;

-- Kết nối vào database planbookai
\c planbookai;

-- Tạo các schemas chính
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS users;
CREATE SCHEMA IF NOT EXISTS assessment;
CREATE SCHEMA IF NOT EXISTS content;
CREATE SCHEMA IF NOT EXISTS students;
CREATE SCHEMA IF NOT EXISTS files;
CREATE SCHEMA IF NOT EXISTS notifications;
CREATE SCHEMA IF NOT EXISTS logging;

-- Cấp quyền cho schemas
GRANT ALL PRIVILEGES ON SCHEMA auth TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA users TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA assessment TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA content TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA students TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA files TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA notifications TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA logging TO postgres;

-- Tạo extension cần thiết (PostgreSQL 17 có sẵn gen_random_uuid())
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 2: BẢNG AUTH
-- =====================================================

-- Bảng vai trò người dùng
CREATE TABLE auth.roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng người dùng chính
CREATE TABLE auth.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role_id INTEGER NOT NULL REFERENCES auth.roles(id),
    is_active BOOLEAN DEFAULT true,
    last_login TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng phiên đăng nhập
CREATE TABLE auth.sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    token VARCHAR(500) NOT NULL UNIQUE,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho auth
CREATE INDEX idx_auth_users_email ON auth.users(email);
CREATE INDEX idx_auth_users_role ON auth.users(role_id);
CREATE INDEX idx_auth_sessions_user ON auth.sessions(user_id);
CREATE INDEX idx_auth_sessions_token ON auth.sessions(token);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 3: BẢNG USERS
-- =====================================================

-- Bảng hồ sơ người dùng
CREATE TABLE users.user_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    full_name VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    address TEXT,
    bio TEXT,
    avatar_url VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng lịch sử hoạt động
CREATE TABLE users.activity_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    action VARCHAR(100) NOT NULL,
    description TEXT,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho users
CREATE INDEX idx_users_profiles_user ON users.user_profiles(user_id);
CREATE INDEX idx_users_activity_logs_user ON users.activity_logs(user_id);
CREATE INDEX idx_users_activity_logs_created ON users.activity_logs(created_at);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 4: BẢNG ASSESSMENT
-- =====================================================

-- Bảng câu hỏi
CREATE TABLE assessment.questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    content TEXT NOT NULL,
    type VARCHAR(50) NOT NULL CHECK (type IN ('MULTIPLE_CHOICE', 'ESSAY', 'SHORT_ANSWER', 'TRUE_FALSE')),
    difficulty VARCHAR(50) CHECK (difficulty IN ('EASY', 'MEDIUM', 'HARD', 'VERY_HARD')),
    subject VARCHAR(100) NOT NULL DEFAULT 'HOA_HOC',
    topic VARCHAR(200),
    correct_answer VARCHAR(10),
    explanation TEXT,
    created_by UUID NOT NULL REFERENCES auth.users(id),
    status VARCHAR(50) DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE', 'INACTIVE', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng lựa chọn câu hỏi
CREATE TABLE assessment.question_choices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_id UUID NOT NULL REFERENCES assessment.questions(id) ON DELETE CASCADE,
    choice_order CHAR(1) NOT NULL CHECK (choice_order IN ('A', 'B', 'C', 'D')),
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(question_id, choice_order)
);

-- Bảng đề thi
CREATE TABLE assessment.exams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    subject VARCHAR(100) NOT NULL DEFAULT 'HOA_HOC',
    grade INTEGER CHECK (grade IN (10, 11, 12)),
    duration_minutes INTEGER CHECK (duration_minutes > 0),
    total_score DECIMAL(5,2) CHECK (total_score > 0),
    teacher_id UUID NOT NULL REFERENCES auth.users(id),
    status VARCHAR(50) DEFAULT 'DRAFT' CHECK (status IN ('DRAFT', 'PUBLISHED', 'COMPLETED', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng câu hỏi trong đề thi
CREATE TABLE assessment.exam_questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id UUID NOT NULL REFERENCES assessment.exams(id) ON DELETE CASCADE,
    question_id UUID NOT NULL REFERENCES assessment.questions(id),
    question_order INTEGER NOT NULL,
    points DECIMAL(4,2) NOT NULL CHECK (points > 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(exam_id, question_order),
    UNIQUE(exam_id, question_id)
);

-- Tạo indexes cho assessment
CREATE INDEX idx_assessment_questions_subject ON assessment.questions(subject);
CREATE INDEX idx_assessment_questions_topic ON assessment.questions(topic);
CREATE INDEX idx_assessment_questions_difficulty ON assessment.questions(difficulty);
CREATE INDEX idx_assessment_questions_created_by ON assessment.questions(created_by);
CREATE INDEX idx_assessment_exams_teacher ON assessment.exams(teacher_id);
CREATE INDEX idx_assessment_exams_subject ON assessment.exams(subject);
CREATE INDEX idx_assessment_question_choices_question ON assessment.question_choices(question_id);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 5: BẢNG CONTENT
-- =====================================================

-- Bảng mẫu giáo án
CREATE TABLE content.lesson_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    template_content JSONB,
    subject VARCHAR(100) NOT NULL DEFAULT 'HOA_HOC',
    grade INTEGER CHECK (grade IN (10, 11, 12)),
    created_by UUID REFERENCES auth.users(id),
    status VARCHAR(50) DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE', 'INACTIVE', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng giáo án
CREATE TABLE content.lesson_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    objectives TEXT,
    content JSONB,
    subject VARCHAR(100) NOT NULL DEFAULT 'HOA_HOC',
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    teacher_id UUID NOT NULL REFERENCES auth.users(id),
    template_id UUID REFERENCES content.lesson_templates(id),
    status VARCHAR(50) DEFAULT 'DRAFT' CHECK (status IN ('DRAFT', 'COMPLETED', 'PUBLISHED', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng chủ đề
CREATE TABLE content.chu_de (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    subject VARCHAR(100) NOT NULL DEFAULT 'HOA_HOC',
    grade INTEGER CHECK (grade IN (10, 11, 12)),
    parent_id UUID REFERENCES content.chu_de(id),
    created_by UUID REFERENCES auth.users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho content
CREATE INDEX idx_content_lesson_templates_subject ON content.lesson_templates(subject);
CREATE INDEX idx_content_lesson_plans_teacher ON content.lesson_plans(teacher_id);
CREATE INDEX idx_content_lesson_plans_subject ON content.lesson_plans(subject);
CREATE INDEX idx_content_chu_de_subject ON content.chu_de(subject);
CREATE INDEX idx_content_chu_de_parent ON content.chu_de(parent_id);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 6: BẢNG STUDENTS
-- =====================================================

-- Bảng lớp học
CREATE TABLE students.classes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    student_count INTEGER DEFAULT 0 CHECK (student_count >= 0),
    homeroom_teacher_id UUID REFERENCES auth.users(id),
    academic_year VARCHAR(20),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng học sinh
CREATE TABLE students.students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(255) NOT NULL,
    student_code VARCHAR(50),
    birth_date DATE,
    gender VARCHAR(10) CHECK (gender IN ('MALE', 'FEMALE', 'OTHER')),
    class_id UUID REFERENCES students.classes(id),
    owner_teacher_id UUID NOT NULL REFERENCES auth.users(id),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(owner_teacher_id, student_code)
);

-- Bảng kết quả học sinh
CREATE TABLE students.student_results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID NOT NULL REFERENCES students.students(id),
    exam_id UUID NOT NULL REFERENCES assessment.exams(id),
    score DECIMAL(5,2) CHECK (score >= 0),
    actual_duration INTEGER CHECK (actual_duration > 0),
    answer_details JSONB,
    grading_method VARCHAR(50) CHECK (grading_method IN ('OCR', 'MANUAL', 'AUTO')),
    notes TEXT,
    exam_date TIMESTAMP,
    graded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(student_id, exam_id)
);

-- Bảng bài làm (answer sheets)
CREATE TABLE students.answer_sheets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID NOT NULL REFERENCES students.students(id),
    exam_id UUID NOT NULL REFERENCES assessment.exams(id),
    image_url VARCHAR(500),
    ocr_result JSONB,
    ocr_status VARCHAR(50) DEFAULT 'PENDING' CHECK (ocr_status IN ('PENDING', 'PROCESSING', 'COMPLETED', 'FAILED')),
    ocr_accuracy DECIMAL(5,2),
    processed_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho students
CREATE INDEX idx_students_classes_teacher ON students.classes(homeroom_teacher_id);
CREATE INDEX idx_students_students_class ON students.students(class_id);
CREATE INDEX idx_students_students_teacher ON students.students(owner_teacher_id);
CREATE INDEX idx_students_students_code ON students.students(student_code);
CREATE INDEX idx_students_student_results_student ON students.student_results(student_id);
CREATE INDEX idx_students_student_results_exam ON students.student_results(exam_id);
CREATE INDEX idx_students_answer_sheets_student ON students.answer_sheets(student_id);
CREATE INDEX idx_students_answer_sheets_exam ON students.answer_sheets(exam_id);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 7: BẢNG FILES
-- =====================================================

-- Bảng file storage
CREATE TABLE files.file_storage (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    file_name VARCHAR(255) NOT NULL,
    original_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_size BIGINT NOT NULL,
    mime_type VARCHAR(100),
    file_hash VARCHAR(64),
    uploaded_by UUID NOT NULL REFERENCES auth.users(id),
    file_type VARCHAR(50) CHECK (file_type IN ('IMAGE', 'DOCUMENT', 'PDF', 'EXCEL', 'OTHER')),
    status VARCHAR(50) DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE', 'ARCHIVED', 'DELETED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng file metadata
CREATE TABLE files.file_metadata (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    file_id UUID NOT NULL REFERENCES files.file_storage(id) ON DELETE CASCADE,
    key VARCHAR(100) NOT NULL,
    value TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(file_id, key)
);

-- Tạo indexes cho files
CREATE INDEX idx_files_storage_uploaded_by ON files.file_storage(uploaded_by);
CREATE INDEX idx_files_storage_file_type ON files.file_storage(file_type);
CREATE INDEX idx_files_storage_status ON files.file_storage(status);
CREATE INDEX idx_files_metadata_file ON files.file_metadata(file_id);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 8: BẢNG NOTIFICATIONS
-- =====================================================

-- Bảng thông báo
CREATE TABLE notifications.notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id),
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    type VARCHAR(50) DEFAULT 'INFO' CHECK (type IN ('INFO', 'SUCCESS', 'WARNING', 'ERROR')),
    is_read BOOLEAN DEFAULT false,
    read_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng email queue
CREATE TABLE notifications.email_queue (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    to_email VARCHAR(255) NOT NULL,
    subject VARCHAR(255) NOT NULL,
    body TEXT NOT NULL,
    status VARCHAR(50) DEFAULT 'PENDING' CHECK (status IN ('PENDING', 'SENT', 'FAILED')),
    retry_count INTEGER DEFAULT 0,
    sent_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho notifications
CREATE INDEX idx_notifications_user ON notifications.notifications(user_id);
CREATE INDEX idx_notifications_is_read ON notifications.notifications(is_read);
CREATE INDEX idx_notifications_created_at ON notifications.notifications(created_at);
CREATE INDEX idx_email_queue_status ON notifications.email_queue(status);

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 9: BẢNG LOGGING
-- =====================================================

-- Bảng system logs
CREATE TABLE logging.system_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    level VARCHAR(20) NOT NULL CHECK (level IN ('DEBUG', 'INFO', 'WARNING', 'ERROR', 'CRITICAL')),
    message TEXT NOT NULL,
    source VARCHAR(100),
    user_id UUID REFERENCES auth.users(id),
    ip_address INET,
    user_agent TEXT,
    stack_trace TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng performance metrics
CREATE TABLE logging.performance_metrics (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    service_name VARCHAR(100) NOT NULL,
    endpoint VARCHAR(255),
    response_time_ms INTEGER,
    status_code INTEGER,
    user_id UUID REFERENCES auth.users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho logging
CREATE INDEX idx_logging_system_logs_level ON logging.system_logs(level);
CREATE INDEX idx_logging_system_logs_created_at ON logging.system_logs(created_at);
CREATE INDEX idx_logging_performance_service ON logging.performance_metrics(service_name);
CREATE INDEX idx_logging_performance_created_at ON logging.performance_metrics(created_at);


-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 10: TRIGGERS & FUNCTIONS
-- =====================================================

-- Function để tự động cập nhật updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Tạo triggers cho tất cả bảng có updated_at
CREATE TRIGGER trigger_updated_at_auth_roles 
    BEFORE UPDATE ON auth.roles FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_auth_users 
    BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_users_user_profiles 
    BEFORE UPDATE ON users.user_profiles FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_assessment_questions 
    BEFORE UPDATE ON assessment.questions FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_assessment_exams 
    BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_lesson_templates 
    BEFORE UPDATE ON content.lesson_templates FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_lesson_plans 
    BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_chu_de 
    BEFORE UPDATE ON content.chu_de FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_students_classes 
    BEFORE UPDATE ON students.classes FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_students_students 
    BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_files_file_storage 
    BEFORE UPDATE ON files.file_storage FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 11: SEED DATA
-- =====================================================

-- Seed roles (vai trò)
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES
(1, 'ADMIN', 'Quản trị viên hệ thống', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'MANAGER', 'Quản lý nội dung và người dùng', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'STAFF', 'Nhân viên tạo nội dung mẫu', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'TEACHER', 'Giáo viên sử dụng hệ thống', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

-- Seed admin user (mật khẩu: admin123)
INSERT INTO auth.users (id, email, password_hash, role_id, is_active, created_at, updated_at) VALUES
('550e8400-e29b-41d4-a716-446655440001', 'admin@planbookai.com', 
 crypt('admin123', gen_salt('bf')), 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (email) DO NOTHING;

-- Seed test teacher (mật khẩu: teacher123)
INSERT INTO auth.users (id, email, password_hash, role_id, is_active, created_at, updated_at) VALUES
('550e8400-e29b-41d4-a716-446655440002', 'teacher@test.com', 
 crypt('teacher123', gen_salt('bf')), 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (email) DO NOTHING;

-- Seed user profiles
INSERT INTO users.user_profiles (user_id, full_name, phone, address, bio, created_at, updated_at) VALUES
('550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'Hà Nội', 'Quản trị viên hệ thống', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('550e8400-e29b-41d4-a716-446655440002', 'Giáo viên Test', '0987654321', 'TP.HCM', 'Giáo viên Hóa học THPT', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Seed chủ đề Hóa học
INSERT INTO content.chu_de (name, description, subject, grade, created_by, created_at, updated_at) VALUES
('Cấu trúc nguyên tử', 'Nghiên cứu về cấu trúc và tính chất của nguyên tử', 'HOA_HOC', 10, 
 '550e8400-e29b-41d4-a716-446655440002', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('Bảng tuần hoàn', 'Nghiên cứu về bảng tuần hoàn các nguyên tố hóa học', 'HOA_HOC', 10, 
 '550e8400-e29b-41d4-a716-446655440002', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('Liên kết hóa học', 'Nghiên cứu về các loại liên kết hóa học', 'HOA_HOC', 10, 
 '550e8400-e29b-41d4-a716-446655440002', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Seed câu hỏi mẫu với choices
DO $$
DECLARE
    question1_id UUID;
    question2_id UUID;
BEGIN
    -- Insert câu hỏi 1 và lấy ID
    INSERT INTO assessment.questions (content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at) 
    VALUES ('Nguyên tử có cấu trúc như thế nào?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 
            'Cấu trúc nguyên tử', 'A', 'Nguyên tử gồm hạt nhân và electron', 
            '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
    RETURNING id INTO question1_id;
    
    -- Insert choices cho câu hỏi 1
    INSERT INTO assessment.question_choices (question_id, choice_order, content, created_at) VALUES
    (question1_id, 'A', 'Hạt nhân và electron', CURRENT_TIMESTAMP),
    (question1_id, 'B', 'Chỉ có hạt nhân', CURRENT_TIMESTAMP),
    (question1_id, 'C', 'Chỉ có electron', CURRENT_TIMESTAMP),
    (question1_id, 'D', 'Proton và neutron', CURRENT_TIMESTAMP);
    
    -- Insert câu hỏi 2 và lấy ID
    INSERT INTO assessment.questions (content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at) 
    VALUES ('Trong bảng tuần hoàn, các nguyên tố được sắp xếp theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 
            'Bảng tuần hoàn', 'B', 'Theo số hiệu nguyên tử tăng dần', 
            '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
    RETURNING id INTO question2_id;
    
    -- Insert choices cho câu hỏi 2
    INSERT INTO assessment.question_choices (question_id, choice_order, content, created_at) VALUES
    (question2_id, 'A', 'Theo khối lượng nguyên tử', CURRENT_TIMESTAMP),
    (question2_id, 'B', 'Theo số hiệu nguyên tử tăng dần', CURRENT_TIMESTAMP),
    (question2_id, 'C', 'Theo tên nguyên tố', CURRENT_TIMESTAMP),
    (question2_id, 'D', 'Theo màu sắc', CURRENT_TIMESTAMP);
END $$;

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 12: VERIFICATION
-- =====================================================

-- Kiểm tra số lượng schemas
SELECT 'Schemas created:' as info, COUNT(*) as count 
FROM information_schema.schemata 
WHERE schema_name IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');

-- Kiểm tra số lượng tables
SELECT 'Tables created:' as info, COUNT(*) as count 
FROM information_schema.tables 
WHERE table_schema IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');

-- Kiểm tra seed data
SELECT 'Roles seeded:' as info, COUNT(*) as count FROM auth.roles;
SELECT 'Users seeded:' as info, COUNT(*) as count FROM auth.users;
SELECT 'User profiles seeded:' as info, COUNT(*) as count FROM users.user_profiles;
SELECT 'Topics seeded:' as info, COUNT(*) as count FROM content.chu_de;
SELECT 'Questions seeded:' as info, COUNT(*) as count FROM assessment.questions;

-- Hiển thị thông tin đăng nhập test
SELECT 
    'TEST ACCOUNTS:' as info,
    u.email,
    r.name as role,
    up.full_name
FROM auth.users u
JOIN auth.roles r ON u.role_id = r.id
JOIN users.user_profiles up ON u.id = up.user_id
ORDER BY r.id;

-- Thông báo hoàn thành
DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'PLANBOOKAI DATABASE CREATED SUCCESSFULLY!';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Total schemas: 8';
    RAISE NOTICE 'Total tables: 20+';
    RAISE NOTICE 'Seed data: Complete';
    RAISE NOTICE 'Test accounts created';
    RAISE NOTICE 'Database ready for development!';
    RAISE NOTICE '========================================';
END $$;

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 13: USER SERVICE SCHEMA
-- =====================================================

-- Cập nhật bảng users để hỗ trợ soft delete
ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS deleted_at TIMESTAMP NULL;
ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN DEFAULT FALSE;

-- Bảng OTP cho các chức năng xác thực (sẽ dùng chung với notification service)
CREATE TABLE users.otp_codes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    otp_code VARCHAR(6) NOT NULL,
    purpose VARCHAR(50) NOT NULL CHECK (purpose IN ('PASSWORD_RESET', 'EMAIL_VERIFICATION', 'PHONE_VERIFICATION')),
    expires_at TIMESTAMP NOT NULL,
    used BOOLEAN DEFAULT FALSE,
    attempts INTEGER DEFAULT 0,
    max_attempts INTEGER DEFAULT 3,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng lịch sử thay đổi mật khẩu
CREATE TABLE users.password_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    changed_at TIMESTAMP NOT NULL,
    changed_by UUID REFERENCES auth.users(id),
    reason VARCHAR(100) DEFAULT 'USER_CHANGE',
    ip_address INET,
    user_agent TEXT
);

-- Bảng quản lý phiên đăng nhập (session management)
CREATE TABLE users.user_sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    session_token VARCHAR(500) NOT NULL UNIQUE,
    device_info JSONB,
    ip_address INET,
    user_agent TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    expires_at TIMESTAMP NOT NULL,
    last_activity TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tạo indexes cho UserService
CREATE INDEX idx_users_otp_codes_user ON users.otp_codes(user_id);
CREATE INDEX idx_users_otp_codes_purpose ON users.otp_codes(purpose);
CREATE INDEX idx_users_otp_codes_expires ON users.otp_codes(expires_at);
CREATE INDEX idx_users_password_history_user ON users.password_history(user_id);
CREATE INDEX idx_users_user_sessions_user ON users.user_sessions(user_id);
CREATE INDEX idx_users_user_sessions_token ON users.user_sessions(session_token);
CREATE INDEX idx_users_user_sessions_active ON users.user_sessions(is_active);

-- Tạo trigger để tự động cập nhật updated_at cho bảng users
CREATE TRIGGER trigger_updated_at_auth_users_soft_delete 
    BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- PLANBOOKAI DATABASE - PHẦN 14: FINAL VERIFICATION
-- =====================================================

-- Kiểm tra số lượng schemas
SELECT 'Schemas created:' as info, COUNT(*) as count 
FROM information_schema.schemata 
WHERE schema_name IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');

-- Kiểm tra số lượng tables
SELECT 'Tables created:' as info, COUNT(*) as count 
FROM information_schema.tables 
WHERE table_schema IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');

-- Kiểm tra seed data
SELECT 'Roles seeded:' as info, COUNT(*) as count FROM auth.roles;
SELECT 'Users seeded:' as info, COUNT(*) as count FROM auth.users;
SELECT 'User profiles seeded:' as info, COUNT(*) as count FROM users.user_profiles;
SELECT 'Topics seeded:' as info, COUNT(*) as count FROM content.chu_de;
SELECT 'Questions seeded:' as info, COUNT(*) as count FROM assessment.questions;

-- Kiểm tra UserService schema
SELECT 'OTP codes table:' as info, COUNT(*) as count FROM users.otp_codes;
SELECT 'Password history table:' as info, COUNT(*) as count FROM users.password_history;
SELECT 'User sessions table:' as info, COUNT(*) as count FROM users.user_sessions;

-- Hiển thị thông tin đăng nhập test
SELECT 
    'TEST ACCOUNTS:' as info,
    u.email,
    r.name as role,
    up.full_name,
    u.is_deleted,
    u.deleted_at
FROM auth.users u
JOIN auth.roles r ON u.role_id = r.id
JOIN users.user_profiles up ON u.id = up.user_id
ORDER BY r.id;

-- Thông báo hoàn thành
DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'PLANBOOKAI DATABASE CREATED SUCCESSFULLY!';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Total schemas: 8';
    RAISE NOTICE 'Total tables: 23+';
    RAISE NOTICE 'Seed data: Complete';
    RAISE NOTICE 'UserService schema: Added';
    RAISE NOTICE 'Test accounts created';
    RAISE NOTICE 'Database ready for development!';
    RAISE NOTICE '========================================';
END $$;