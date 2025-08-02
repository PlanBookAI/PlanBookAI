-- PlanbookAI Development Database Migration Script
-- Tạo database schemas và tables theo Domain Driven Design

-- ==============================================
-- 1. TẠO SCHEMAS CHO CÁC BOUNDED CONTEXTS
-- ==============================================

CREATE SCHEMA IF NOT EXISTS users;           -- User Management Context
CREATE SCHEMA IF NOT EXISTS content;         -- Educational Content Context  
CREATE SCHEMA IF NOT EXISTS assessment;      -- Assessment Context
CREATE SCHEMA IF NOT EXISTS students;        -- Student Data Context

-- Grant permissions cho user test
GRANT ALL PRIVILEGES ON SCHEMA users TO test;
GRANT ALL PRIVILEGES ON SCHEMA content TO test;
GRANT ALL PRIVILEGES ON SCHEMA assessment TO test;
GRANT ALL PRIVILEGES ON SCHEMA students TO test;

-- ==============================================
-- 2. USER MANAGEMENT DOMAIN TABLES
-- ==============================================

-- Bảng vai trò người dùng
CREATE TABLE IF NOT EXISTS users.roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng người dùng chính
CREATE TABLE IF NOT EXISTS users.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    role_id INTEGER NOT NULL REFERENCES users.roles(id),
    is_active BOOLEAN DEFAULT true,
    last_login TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng phiên đăng nhập
CREATE TABLE IF NOT EXISTS users.sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users.users(id),
    token VARCHAR(500) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    ip_address VARCHAR(45),
    user_agent TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==============================================
-- 3. EDUCATIONAL CONTENT DOMAIN TABLES
-- ==============================================

-- Bảng mẫu giáo án
CREATE TABLE IF NOT EXISTS content.lesson_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    template_content JSONB NOT NULL,
    subject VARCHAR(50) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    created_by UUID NOT NULL REFERENCES users.users(id),
    status VARCHAR(20) DEFAULT 'ACTIVE',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng giáo án
CREATE TABLE IF NOT EXISTS content.lesson_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    objectives TEXT,
    content JSONB NOT NULL,
    subject VARCHAR(50) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    teacher_id UUID NOT NULL REFERENCES users.users(id),
    template_id UUID REFERENCES content.lesson_templates(id),
    status VARCHAR(20) DEFAULT 'DRAFT',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==============================================
-- 4. ASSESSMENT DOMAIN TABLES
-- ==============================================

-- Bảng câu hỏi
CREATE TABLE IF NOT EXISTS assessment.questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    content TEXT NOT NULL,
    type VARCHAR(20) NOT NULL CHECK (type IN ('MULTIPLE_CHOICE', 'ESSAY')),
    difficulty VARCHAR(20) NOT NULL CHECK (difficulty IN ('EASY', 'MEDIUM', 'HARD', 'VERY_HARD')),
    subject VARCHAR(50) NOT NULL,
    topic VARCHAR(255),
    correct_answer VARCHAR(10), -- Cho câu trắc nghiệm: A, B, C, D
    explanation TEXT,
    created_by UUID NOT NULL REFERENCES users.users(id),
    status VARCHAR(20) DEFAULT 'ACTIVE',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng lựa chọn cho câu hỏi trắc nghiệm
CREATE TABLE IF NOT EXISTS assessment.question_choices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_id UUID NOT NULL REFERENCES assessment.questions(id) ON DELETE CASCADE,
    choice_order CHAR(1) NOT NULL CHECK (choice_order IN ('A', 'B', 'C', 'D')),
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng đề thi
CREATE TABLE IF NOT EXISTS assessment.exams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    subject VARCHAR(50) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    duration_minutes INTEGER NOT NULL CHECK (duration_minutes BETWEEN 15 AND 180),
    total_score DECIMAL(4,2) DEFAULT 10.00,
    teacher_id UUID NOT NULL REFERENCES users.users(id),
    status VARCHAR(20) DEFAULT 'DRAFT',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng câu hỏi trong đề thi
CREATE TABLE IF NOT EXISTS assessment.exam_questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id UUID NOT NULL REFERENCES assessment.exams(id) ON DELETE CASCADE,
    question_id UUID NOT NULL REFERENCES assessment.questions(id),
    question_order INTEGER NOT NULL,
    points DECIMAL(4,2) NOT NULL CHECK (points > 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(exam_id, question_order)
);

-- ==============================================
-- 5. STUDENT DATA DOMAIN TABLES
-- ==============================================

-- Bảng lớp học
CREATE TABLE IF NOT EXISTS students.classes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    student_count INTEGER DEFAULT 0,
    homeroom_teacher_id UUID NOT NULL REFERENCES users.users(id),
    academic_year VARCHAR(10) NOT NULL, -- VD: "2024-2025"
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng học sinh
CREATE TABLE IF NOT EXISTS students.students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(255) NOT NULL,
    student_code VARCHAR(50) NOT NULL,
    birth_date DATE,
    gender VARCHAR(10) CHECK (gender IN ('MALE', 'FEMALE')),
    class_id UUID NOT NULL REFERENCES students.classes(id),
    owner_teacher_id UUID NOT NULL REFERENCES users.users(id),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(student_code, owner_teacher_id)
);

-- Bảng kết quả học tập
CREATE TABLE IF NOT EXISTS students.student_results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID NOT NULL REFERENCES students.students(id) ON DELETE CASCADE,
    exam_id UUID NOT NULL REFERENCES assessment.exams(id),
    score DECIMAL(4,2) NOT NULL CHECK (score >= 0 AND score <= 10),
    actual_duration INTEGER, -- Thời gian thực tế (phút)
    answer_details JSONB, -- Lưu chi tiết câu trả lời
    grading_method VARCHAR(20) DEFAULT 'OCR' CHECK (grading_method IN ('OCR', 'MANUAL')),
    notes TEXT,
    exam_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    graded_at TIMESTAMP
);

-- ==============================================
-- 6. INDEXES ĐỂ TỐI ƯU HIỆU SUẤT
-- ==============================================

-- User Management indexes
CREATE INDEX IF NOT EXISTS idx_users_email ON users.users(email);
CREATE INDEX IF NOT EXISTS idx_users_role ON users.users(role_id);
CREATE INDEX IF NOT EXISTS idx_sessions_user ON users.sessions(user_id);
CREATE INDEX IF NOT EXISTS idx_sessions_token ON users.sessions(token);

-- Educational Content indexes
CREATE INDEX IF NOT EXISTS idx_lesson_plans_teacher ON content.lesson_plans(teacher_id);
CREATE INDEX IF NOT EXISTS idx_lesson_plans_subject ON content.lesson_plans(subject);
CREATE INDEX IF NOT EXISTS idx_lesson_templates_subject ON content.lesson_templates(subject);

-- Assessment indexes
CREATE INDEX IF NOT EXISTS idx_questions_subject ON assessment.questions(subject);
CREATE INDEX IF NOT EXISTS idx_questions_difficulty ON assessment.questions(difficulty);
CREATE INDEX IF NOT EXISTS idx_questions_created_by ON assessment.questions(created_by);
CREATE INDEX IF NOT EXISTS idx_exams_teacher ON assessment.exams(teacher_id);
CREATE INDEX IF NOT EXISTS idx_question_choices_question ON assessment.question_choices(question_id);

-- Student Data indexes
CREATE INDEX IF NOT EXISTS idx_students_class ON students.students(class_id);
CREATE INDEX IF NOT EXISTS idx_students_teacher ON students.students(owner_teacher_id);
CREATE INDEX IF NOT EXISTS idx_students_code ON students.students(student_code);
CREATE INDEX IF NOT EXISTS idx_student_results_student ON students.student_results(student_id);
CREATE INDEX IF NOT EXISTS idx_student_results_exam ON students.student_results(exam_id);

-- ==============================================
-- 7. DỮ LIỆU MẪU CHO DEVELOPMENT
-- ==============================================

-- Thêm các vai trò cơ bản
INSERT INTO users.roles (name, description) VALUES
('ADMIN', 'Quản trị viên hệ thống'),
('MANAGER', 'Quản lý nội dung và người dùng'),
('STAFF', 'Nhân viên tạo nội dung mẫu'),
('TEACHER', 'Giáo viên sử dụng hệ thống')
ON CONFLICT (name) DO NOTHING;

-- Thêm tài khoản admin mặc định (password: admin123)
INSERT INTO users.users (email, password_hash, full_name, role_id) VALUES
('admin@planbookai.dev', '$2a$10$rZ8R1LZ5Z5Z5Z5Z5Z5Z5ZOeKq9Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z', 'Admin Development', 1)
ON CONFLICT (email) DO NOTHING;

-- Thêm một giáo viên test
INSERT INTO users.users (email, password_hash, full_name, role_id) VALUES
('teacher@planbookai.dev', '$2a$10$rZ8R1LZ5Z5Z5Z5Z5Z5Z5ZOeKq9Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z', 'Giáo Viên Test', 4)
ON CONFLICT (email) DO NOTHING;

-- Bảng test để verify connection
CREATE TABLE IF NOT EXISTS public.kiem_tra_ket_noi (
    id SERIAL PRIMARY KEY,
    service_name VARCHAR(100) NOT NULL,
    test_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20) DEFAULT 'SUCCESS'
);

-- Insert test record
INSERT INTO public.kiem_tra_ket_noi (service_name, status) VALUES
('PlanbookAI Development Setup', 'SUCCESS');

-- ==============================================
-- 8. FUNCTIONS VÀ TRIGGERS
-- ==============================================

-- Function để tự động update timestamp
CREATE OR REPLACE FUNCTION update_modified_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.ngay_cap_nhat = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Triggers cho auto-update timestamps
CREATE TRIGGER update_users_modtime BEFORE UPDATE ON users.users FOR EACH ROW EXECUTE FUNCTION update_modified_column();
CREATE TRIGGER update_lesson_plans_modtime BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION update_modified_column();
CREATE TRIGGER update_exams_modtime BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION update_modified_column();
CREATE TRIGGER update_students_modtime BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION update_modified_column();

-- ==============================================
-- HOÀN THÀNH MIGRATION
-- ==============================================

-- Log migration completion
INSERT INTO public.kiem_tra_ket_noi (service_name, status) VALUES
('Database Migration Completed', 'SUCCESS');