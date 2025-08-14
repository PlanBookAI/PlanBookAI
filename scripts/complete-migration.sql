-- =====================================================
-- PLANBOOKAI COMPLETE DATABASE MIGRATION
-- Generated from Entity Framework DbContexts
-- Date: 2025-08-14
-- =====================================================

-- STEP 1: CREATE DATABASE
-- =====================================================
-- Note: Database should be created by the script runner

-- STEP 2: CREATE SCHEMAS
-- =====================================================
CREATE SCHEMA IF NOT EXISTS users;
CREATE SCHEMA IF NOT EXISTS content;
CREATE SCHEMA IF NOT EXISTS assessment;
CREATE SCHEMA IF NOT EXISTS students;

-- Grant privileges
GRANT ALL PRIVILEGES ON SCHEMA users TO test;
GRANT ALL PRIVILEGES ON SCHEMA content TO test;
GRANT ALL PRIVILEGES ON SCHEMA assessment TO test;
GRANT ALL PRIVILEGES ON SCHEMA students TO test;

-- STEP 3: CREATE TABLES
-- =====================================================

-- USERS SCHEMA TABLES
-- =====================================================

-- Table: users.roles
CREATE TABLE IF NOT EXISTS users.roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Table: users.users
CREATE TABLE IF NOT EXISTS users.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    full_name VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role_id INTEGER NOT NULL,
    is_active BOOLEAN DEFAULT true,
    last_login TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_users_role_id FOREIGN KEY (role_id) REFERENCES users.roles(id) ON DELETE RESTRICT
);

-- Table: users.sessions
CREATE TABLE IF NOT EXISTS users.sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    token VARCHAR(500) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_sessions_user_id FOREIGN KEY (user_id) REFERENCES users.users(id) ON DELETE CASCADE
);

-- Table: users.user_profiles (UserService)
CREATE TABLE IF NOT EXISTS users.user_profiles (
    user_id UUID PRIMARY KEY,
    phone VARCHAR(20),
    address TEXT,
    avatar_url VARCHAR(500),
    bio TEXT,
    preferences JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_user_profiles_user_id FOREIGN KEY (user_id) REFERENCES users.users(id) ON DELETE CASCADE
);

-- Table: users.activity_history (UserService)
CREATE TABLE IF NOT EXISTS users.activity_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    activity_type VARCHAR(100) NOT NULL,
    description TEXT,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_activity_history_user_id FOREIGN KEY (user_id) REFERENCES users.users(id) ON DELETE CASCADE
);

-- CONTENT SCHEMA TABLES (PlanService)
-- =====================================================

-- Table: content.lesson_templates
CREATE TABLE IF NOT EXISTS content.lesson_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    template_name VARCHAR(255) NOT NULL,
    subject VARCHAR(100) NOT NULL,
    grade_level INTEGER NOT NULL,
    summary TEXT,
    template_content JSONB NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_by UUID NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_lesson_templates_created_by FOREIGN KEY (created_by) REFERENCES users.users(id) ON DELETE RESTRICT
);

-- Table: content.lesson_plans
CREATE TABLE IF NOT EXISTS content.lesson_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    subject VARCHAR(100) NOT NULL,
    grade_level INTEGER NOT NULL,
    lesson_date DATE NOT NULL,
    duration_minutes INTEGER NOT NULL,
    objectives TEXT,
    materials TEXT,
    activities TEXT,
    assessment TEXT,
    notes TEXT,
    status VARCHAR(50) DEFAULT 'draft',
    created_by UUID NOT NULL,
    template_id UUID,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_lesson_plans_created_by FOREIGN KEY (created_by) REFERENCES users.users(id) ON DELETE RESTRICT,
    CONSTRAINT fk_lesson_plans_template_id FOREIGN KEY (template_id) REFERENCES content.lesson_templates(id) ON DELETE SET NULL
);

-- Table: content.educational_content
CREATE TABLE IF NOT EXISTS content.educational_content (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    lesson_plan_id UUID NOT NULL,
    content_type VARCHAR(50) NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    content_data JSONB NOT NULL,
    order_index INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_educational_content_lesson_plan_id FOREIGN KEY (lesson_plan_id) REFERENCES content.lesson_plans(id) ON DELETE CASCADE
);

-- Table: content.learning_objectives
CREATE TABLE IF NOT EXISTS content.learning_objectives (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    lesson_plan_id UUID NOT NULL,
    objective_text TEXT NOT NULL,
    objective_type VARCHAR(50) DEFAULT 'knowledge',
    order_index INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_learning_objectives_lesson_plan_id FOREIGN KEY (lesson_plan_id) REFERENCES content.lesson_plans(id) ON DELETE CASCADE
);

-- ASSESSMENT SCHEMA TABLES (TaskService)
-- =====================================================

-- Table: assessment.questions
CREATE TABLE IF NOT EXISTS assessment.questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_text TEXT NOT NULL,
    question_type VARCHAR(50) NOT NULL DEFAULT 'multiple_choice',
    subject VARCHAR(100) NOT NULL,
    topic VARCHAR(100),
    difficulty_level VARCHAR(20) DEFAULT 'medium',
    points DECIMAL(5,2) DEFAULT 1.0,
    correct_answer TEXT,
    explanation TEXT,
    created_by UUID NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_questions_created_by FOREIGN KEY (created_by) REFERENCES users.users(id) ON DELETE RESTRICT
);

-- Table: assessment.question_choices
CREATE TABLE IF NOT EXISTS assessment.question_choices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_id UUID NOT NULL,
    choice_text TEXT NOT NULL,
    is_correct BOOLEAN DEFAULT false,
    order_index INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_question_choices_question_id FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE CASCADE
);

-- Table: assessment.exams
CREATE TABLE IF NOT EXISTS assessment.exams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    subject VARCHAR(100) NOT NULL,
    grade_level INTEGER NOT NULL,
    duration_minutes INTEGER NOT NULL,
    total_points DECIMAL(5,2) DEFAULT 10.0,
    instructions TEXT,
    status VARCHAR(50) DEFAULT 'draft',
    created_by UUID NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_exams_created_by FOREIGN KEY (created_by) REFERENCES users.users(id) ON DELETE RESTRICT
);

-- Table: assessment.exam_questions
CREATE TABLE IF NOT EXISTS assessment.exam_questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id UUID NOT NULL,
    question_id UUID NOT NULL,
    points DECIMAL(5,2) DEFAULT 1.0,
    order_index INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_exam_questions_exam_id FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE,
    CONSTRAINT fk_exam_questions_question_id FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE RESTRICT
);

-- STUDENTS SCHEMA TABLES (TaskService)
-- =====================================================

-- Table: students.classes
CREATE TABLE IF NOT EXISTS students.classes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    class_name VARCHAR(100) NOT NULL,
    grade_level INTEGER NOT NULL,
    academic_year VARCHAR(20) NOT NULL,
    teacher_id UUID NOT NULL,
    max_students INTEGER DEFAULT 40,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_classes_teacher_id FOREIGN KEY (teacher_id) REFERENCES users.users(id) ON DELETE RESTRICT
);

-- Table: students.students
CREATE TABLE IF NOT EXISTS students.students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_code VARCHAR(50) NOT NULL UNIQUE,
    full_name VARCHAR(255) NOT NULL,
    date_of_birth DATE,
    gender VARCHAR(10),
    class_id UUID,
    teacher_id UUID NOT NULL,
    parent_contact VARCHAR(255),
    address TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_students_class_id FOREIGN KEY (class_id) REFERENCES students.classes(id) ON DELETE SET NULL,
    CONSTRAINT fk_students_teacher_id FOREIGN KEY (teacher_id) REFERENCES users.users(id) ON DELETE RESTRICT
);

-- Table: students.student_results
CREATE TABLE IF NOT EXISTS students.student_results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID NOT NULL,
    exam_id UUID NOT NULL,
    score DECIMAL(5,2),
    max_score DECIMAL(5,2),
    percentage DECIMAL(5,2),
    time_taken_minutes INTEGER,
    submitted_at TIMESTAMP,
    graded_at TIMESTAMP,
    grader_id UUID,
    feedback TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_student_results_student_id FOREIGN KEY (student_id) REFERENCES students.students(id) ON DELETE CASCADE,
    CONSTRAINT fk_student_results_exam_id FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE,
    CONSTRAINT fk_student_results_grader_id FOREIGN KEY (grader_id) REFERENCES users.users(id) ON DELETE SET NULL
);

-- Table: students.answer_sheets (TaskService)
CREATE TABLE IF NOT EXISTS students.answer_sheets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID NOT NULL,
    exam_id UUID NOT NULL,
    answers JSONB NOT NULL,
    submitted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_answer_sheets_student_id FOREIGN KEY (student_id) REFERENCES students.students(id) ON DELETE CASCADE,
    CONSTRAINT fk_answer_sheets_exam_id FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE
);

-- STEP 4: CREATE INDEXES
-- =====================================================

-- Users schema indexes
CREATE INDEX IF NOT EXISTS idx_users_email ON users.users(email);
CREATE INDEX IF NOT EXISTS idx_users_role ON users.users(role_id);
CREATE INDEX IF NOT EXISTS idx_roles_name ON users.roles(name);
CREATE INDEX IF NOT EXISTS idx_sessions_user ON users.sessions(user_id);
CREATE INDEX IF NOT EXISTS idx_sessions_token ON users.sessions(token);
CREATE INDEX IF NOT EXISTS idx_user_profiles_user_id ON users.user_profiles(user_id);
CREATE INDEX IF NOT EXISTS idx_activity_history_user_id ON users.activity_history(user_id);
CREATE INDEX IF NOT EXISTS idx_activity_history_created_at ON users.activity_history(created_at);

-- Content schema indexes
CREATE INDEX IF NOT EXISTS idx_lesson_templates_subject ON content.lesson_templates(subject);
CREATE INDEX IF NOT EXISTS idx_lesson_templates_grade ON content.lesson_templates(grade_level);
CREATE INDEX IF NOT EXISTS idx_lesson_templates_created_by ON content.lesson_templates(created_by);
CREATE INDEX IF NOT EXISTS idx_lesson_plans_title ON content.lesson_plans(title);
CREATE INDEX IF NOT EXISTS idx_lesson_plans_subject ON content.lesson_plans(subject);
CREATE INDEX IF NOT EXISTS idx_lesson_plans_created_by ON content.lesson_plans(created_by);
CREATE INDEX IF NOT EXISTS idx_lesson_plans_status ON content.lesson_plans(status);
CREATE INDEX IF NOT EXISTS idx_educational_content_lesson_plan_id ON content.educational_content(lesson_plan_id);
CREATE INDEX IF NOT EXISTS idx_learning_objectives_lesson_plan_id ON content.learning_objectives(lesson_plan_id);

-- Assessment schema indexes
CREATE INDEX IF NOT EXISTS idx_questions_subject ON assessment.questions(subject);
CREATE INDEX IF NOT EXISTS idx_questions_topic ON assessment.questions(topic);
CREATE INDEX IF NOT EXISTS idx_questions_difficulty ON assessment.questions(difficulty_level);
CREATE INDEX IF NOT EXISTS idx_questions_created_by ON assessment.questions(created_by);
CREATE INDEX IF NOT EXISTS idx_question_choices_question_id ON assessment.question_choices(question_id);
CREATE INDEX IF NOT EXISTS idx_exams_title ON assessment.exams(title);
CREATE INDEX IF NOT EXISTS idx_exams_subject ON assessment.exams(subject);
CREATE INDEX IF NOT EXISTS idx_exams_created_by ON assessment.exams(created_by);
CREATE INDEX IF NOT EXISTS idx_exam_questions_exam_id ON assessment.exam_questions(exam_id);
CREATE INDEX IF NOT EXISTS idx_exam_questions_question_id ON assessment.exam_questions(question_id);

-- Students schema indexes
CREATE INDEX IF NOT EXISTS idx_classes_grade_level ON students.classes(grade_level);
CREATE INDEX IF NOT EXISTS idx_classes_teacher_id ON students.classes(teacher_id);
CREATE INDEX IF NOT EXISTS idx_students_student_code ON students.students(student_code);
CREATE INDEX IF NOT EXISTS idx_students_class_id ON students.students(class_id);
CREATE INDEX IF NOT EXISTS idx_students_teacher_id ON students.students(teacher_id);
CREATE INDEX IF NOT EXISTS idx_student_results_student_id ON students.student_results(student_id);
CREATE INDEX IF NOT EXISTS idx_student_results_exam_id ON students.student_results(exam_id);
CREATE INDEX IF NOT EXISTS idx_answer_sheets_student_id ON students.answer_sheets(student_id);
CREATE INDEX IF NOT EXISTS idx_answer_sheets_exam_id ON students.answer_sheets(exam_id);

-- STEP 5: CREATE TRIGGERS FOR UPDATED_AT
-- =====================================================

-- Function to update updated_at column
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers for all tables with updated_at
CREATE TRIGGER trigger_updated_at_users_roles
    BEFORE UPDATE ON users.roles
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_users_users
    BEFORE UPDATE ON users.users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_users_user_profiles
    BEFORE UPDATE ON users.user_profiles
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_lesson_templates
    BEFORE UPDATE ON content.lesson_templates
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_lesson_plans
    BEFORE UPDATE ON content.lesson_plans
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_educational_content
    BEFORE UPDATE ON content.educational_content
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_content_learning_objectives
    BEFORE UPDATE ON content.learning_objectives
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_assessment_questions
    BEFORE UPDATE ON assessment.questions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_assessment_exams
    BEFORE UPDATE ON assessment.exams
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_students_classes
    BEFORE UPDATE ON students.classes
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_students_students
    BEFORE UPDATE ON students.students
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_updated_at_students_student_results
    BEFORE UPDATE ON students.student_results
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- STEP 6: INSERT SEED DATA
-- =====================================================

-- Insert default roles
INSERT INTO users.roles (id, name, description, is_active, created_at, updated_at) VALUES
(1, 'ADMIN', 'Quản trị viên hệ thống', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'MANAGER', 'Quản lý nội dung và người dùng', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'STAFF', 'Nhân viên tạo nội dung mẫu', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'TEACHER', 'Giáo viên sử dụng hệ thống', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

-- STEP 7: GRANT PERMISSIONS
-- =====================================================

-- Grant all privileges on all tables to test user
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA users TO test;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA content TO test;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA assessment TO test;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA students TO test;

-- Grant usage on sequences
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA users TO test;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA content TO test;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA assessment TO test;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA students TO test;

-- STEP 8: VERIFICATION
-- =====================================================

-- Count tables in each schema
SELECT 'users' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'users'
UNION ALL
SELECT 'content' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'content'
UNION ALL
SELECT 'assessment' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'assessment'
UNION ALL
SELECT 'students' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'students';

-- Count roles
SELECT COUNT(*) as role_count FROM users.roles;

-- =====================================================
-- MIGRATION COMPLETED SUCCESSFULLY!
-- =====================================================
