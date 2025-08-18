# PlanbookAI Database Migration Script
# Kiểm tra và tạo database structure (không thêm data)

Write-Host "PLANBOOKAI DATABASE MIGRATION" -ForegroundColor Green
Write-Host "=============================" -ForegroundColor Green
Write-Host ""

# Check if PostgreSQL container is running
$containerStatus = docker ps --filter "name=planbookai-postgres-dev" --format "{{.Names}}"
if (-not $containerStatus) {
    Write-Host "[ERROR] PostgreSQL container is not running!" -ForegroundColor Red
    Write-Host "Please run: .\scripts\start-dev-db.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "[INFO] PostgreSQL container is running" -ForegroundColor Green

# Database connection parameters
$DB_HOST = "localhost"
$DB_PORT = "5432"
$DB_NAME = "planbookai"
$DB_USER = "test"
$DB_PASSWORD = "test123"

# Function to execute SQL command
function Execute-SQL {
    param(
        [string]$SqlCommand,
        [string]$Description,
        [string]$Database = $DB_NAME
    )
    
    Write-Host "[INFO] $Description..." -ForegroundColor Cyan
    
    try {
        $result = docker exec planbookai-postgres-dev psql -U $DB_USER -d $Database -c "$SqlCommand" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] $Description completed" -ForegroundColor Green
            return $true
        } else {
            Write-Host "[ERROR] $Description failed: $result" -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "[ERROR] $Description failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to check if database exists
function Test-Database {
    param([string]$DatabaseName)
    
    $checkDb = docker exec planbookai-postgres-dev psql -U $DB_USER -d postgres -t -c "SELECT 1 FROM pg_database WHERE datname='$DatabaseName';" 2>$null
    return ($checkDb -match "1")
}

# Function to check if schema exists
function Test-Schema {
    param([string]$SchemaName)
    
    $checkSchema = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT 1 FROM information_schema.schemata WHERE schema_name='$SchemaName';" 2>$null
    return ($checkSchema -match "1")
}

# Function to check if table exists
function Test-Table {
    param([string]$SchemaName, [string]$TableName)
    
    $checkTable = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT 1 FROM information_schema.tables WHERE table_schema='$SchemaName' AND table_name='$TableName';" 2>$null
    return ($checkTable -match "1")
}

Write-Host ""
Write-Host "STEP 1: DATABASE VERIFICATION" -ForegroundColor Yellow
Write-Host "==============================" -ForegroundColor Yellow

# Check and create database
if (-not (Test-Database $DB_NAME)) {
    Write-Host "[INFO] Database '$DB_NAME' does not exist. Creating..." -ForegroundColor Cyan
    Execute-SQL "CREATE DATABASE $DB_NAME;" "Create database $DB_NAME" "postgres"
} else {
    Write-Host "[OK] Database '$DB_NAME' already exists" -ForegroundColor Green
}

Write-Host ""
Write-Host "STEP 2: SCHEMA CREATION" -ForegroundColor Yellow
Write-Host "======================" -ForegroundColor Yellow

# Define schemas
$schemas = @("users", "content", "assessment", "students")

foreach ($schema in $schemas) {
    if (-not (Test-Schema $schema)) {
        Write-Host "[INFO] Schema '$schema' does not exist. Creating..." -ForegroundColor Cyan
        Execute-SQL "CREATE SCHEMA IF NOT EXISTS $schema;" "Create schema $schema"
        Execute-SQL "GRANT ALL PRIVILEGES ON SCHEMA $schema TO $DB_USER;" "Grant privileges on schema $schema"
    } else {
        Write-Host "[OK] Schema '$schema' already exists" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "STEP 3: TABLE CREATION" -ForegroundColor Yellow
Write-Host "=====================" -ForegroundColor Yellow

# Create tables with full structure (ordered by dependencies)
$tablesOrdered = @(
    @{
        name = "users.roles"
        sql = @"
CREATE TABLE IF NOT EXISTS users.roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "users.users"
        sql = @"
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
"@
    },
    @{
        name = "users.sessions"
        sql = @"
CREATE TABLE IF NOT EXISTS users.sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users.users(id) ON DELETE CASCADE,
    token VARCHAR(500) NOT NULL UNIQUE,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "content.lesson_templates"
        sql = @"
CREATE TABLE IF NOT EXISTS content.lesson_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    template_content JSONB,
    subject VARCHAR(100),
    grade INTEGER CHECK (grade IN (10, 11, 12)),
    created_by UUID REFERENCES users.users(id),
    status VARCHAR(50) DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE', 'INACTIVE', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "content.lesson_plans"
        sql = @"
CREATE TABLE IF NOT EXISTS content.lesson_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    objectives TEXT,
    content JSONB,
    subject VARCHAR(100) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    teacher_id UUID NOT NULL REFERENCES users.users(id),
    template_id UUID REFERENCES content.lesson_templates(id),
    status VARCHAR(50) DEFAULT 'DRAFT' CHECK (status IN ('DRAFT', 'COMPLETED', 'PUBLISHED', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "assessment.questions"
        sql = @"
CREATE TABLE IF NOT EXISTS assessment.questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    content TEXT NOT NULL,
    type VARCHAR(50) NOT NULL CHECK (type IN ('MULTIPLE_CHOICE', 'ESSAY', 'SHORT_ANSWER', 'TRUE_FALSE')),
    difficulty VARCHAR(50) CHECK (difficulty IN ('EASY', 'MEDIUM', 'HARD', 'VERY_HARD')),
    subject VARCHAR(100) NOT NULL,
    topic VARCHAR(200),
    correct_answer VARCHAR(10),
    explanation TEXT,
    created_by UUID REFERENCES users.users(id),
    status VARCHAR(50) DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE', 'INACTIVE', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "assessment.question_choices"
        sql = @"
CREATE TABLE IF NOT EXISTS assessment.question_choices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_id UUID NOT NULL REFERENCES assessment.questions(id) ON DELETE CASCADE,
    choice_order CHAR(1) NOT NULL CHECK (choice_order IN ('A', 'B', 'C', 'D')),
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(question_id, choice_order)
);
"@
    },
    @{
        name = "assessment.exams"
        sql = @"
CREATE TABLE IF NOT EXISTS assessment.exams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    subject VARCHAR(100) NOT NULL,
    grade INTEGER CHECK (grade IN (10, 11, 12)),
    duration_minutes INTEGER CHECK (duration_minutes > 0),
    total_score DECIMAL(5,2) CHECK (total_score > 0),
    teacher_id UUID NOT NULL REFERENCES users.users(id),
    status VARCHAR(50) DEFAULT 'DRAFT' CHECK (status IN ('DRAFT', 'PUBLISHED', 'COMPLETED', 'ARCHIVED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "assessment.exam_questions"
        sql = @"
CREATE TABLE IF NOT EXISTS assessment.exam_questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id UUID NOT NULL REFERENCES assessment.exams(id) ON DELETE CASCADE,
    question_id UUID NOT NULL REFERENCES assessment.questions(id),
    question_order INTEGER NOT NULL,
    points DECIMAL(4,2) NOT NULL CHECK (points > 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(exam_id, question_order),
    UNIQUE(exam_id, question_id)
);
"@
    },
    @{
        name = "students.classes"
        sql = @"
CREATE TABLE IF NOT EXISTS students.classes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    grade INTEGER NOT NULL CHECK (grade IN (10, 11, 12)),
    student_count INTEGER DEFAULT 0 CHECK (student_count >= 0),
    homeroom_teacher_id UUID REFERENCES users.users(id),
    academic_year VARCHAR(20),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
    },
    @{
        name = "students.students"
        sql = @"
CREATE TABLE IF NOT EXISTS students.students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(255) NOT NULL,
    student_code VARCHAR(50),
    birth_date DATE,
    gender VARCHAR(10) CHECK (gender IN ('MALE', 'FEMALE', 'OTHER')),
    class_id UUID REFERENCES students.classes(id),
    owner_teacher_id UUID NOT NULL REFERENCES users.users(id),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(owner_teacher_id, student_code)
);
"@
    },
    @{
        name = "students.student_results"
        sql = @"
CREATE TABLE IF NOT EXISTS students.student_results (
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
"@
    }
)

# Create tables in dependency order
foreach ($table in $tablesOrdered) {
    $tableName = $table.name
    $schemaName = $tableName.Split('.')[0]
    $tableNameOnly = $tableName.Split('.')[1]
    
    if (-not (Test-Table $schemaName $tableNameOnly)) {
        Write-Host "[INFO] Table '$tableName' does not exist. Creating..." -ForegroundColor Cyan
        Execute-SQL $table.sql "Create table $tableName"
    } else {
        Write-Host "[OK] Table '$tableName' already exists" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "STEP 4: INDEX CREATION" -ForegroundColor Yellow
Write-Host "=====================" -ForegroundColor Yellow

# Create indexes for performance
$indexes = @(
    "CREATE INDEX IF NOT EXISTS idx_users_email ON users.users(email);",
    "CREATE INDEX IF NOT EXISTS idx_users_role ON users.users(role_id);",
    "CREATE INDEX IF NOT EXISTS idx_sessions_user ON users.sessions(user_id);",
    "CREATE INDEX IF NOT EXISTS idx_sessions_token ON users.sessions(token);",
    "CREATE INDEX IF NOT EXISTS idx_lesson_plans_teacher ON content.lesson_plans(teacher_id);",
    "CREATE INDEX IF NOT EXISTS idx_lesson_plans_subject ON content.lesson_plans(subject);",
    "CREATE INDEX IF NOT EXISTS idx_lesson_plans_status ON content.lesson_plans(status);",
    "CREATE INDEX IF NOT EXISTS idx_lesson_templates_subject ON content.lesson_templates(subject);",
    "CREATE INDEX IF NOT EXISTS idx_questions_subject ON assessment.questions(subject);",
    "CREATE INDEX IF NOT EXISTS idx_questions_topic ON assessment.questions(topic);",
    "CREATE INDEX IF NOT EXISTS idx_questions_difficulty ON assessment.questions(difficulty);",
    "CREATE INDEX IF NOT EXISTS idx_questions_created_by ON assessment.questions(created_by);",
    "CREATE INDEX IF NOT EXISTS idx_exams_teacher ON assessment.exams(teacher_id);",
    "CREATE INDEX IF NOT EXISTS idx_exams_subject ON assessment.exams(subject);",
    "CREATE INDEX IF NOT EXISTS idx_question_choices_question ON assessment.question_choices(question_id);",
    "CREATE INDEX IF NOT EXISTS idx_students_class ON students.students(class_id);",
    "CREATE INDEX IF NOT EXISTS idx_students_teacher ON students.students(owner_teacher_id);",
    "CREATE INDEX IF NOT EXISTS idx_students_code ON students.students(student_code);",
    "CREATE INDEX IF NOT EXISTS idx_student_results_student ON students.student_results(student_id);",
    "CREATE INDEX IF NOT EXISTS idx_student_results_exam ON students.student_results(exam_id);",
    "CREATE INDEX IF NOT EXISTS idx_student_results_exam_date ON students.student_results(exam_date);"
)

foreach ($index in $indexes) {
    $indexName = ($index -split " ")[4]  # Extract index name
    Execute-SQL $index "Create index $indexName"
}

Write-Host ""
Write-Host "STEP 5: TRIGGERS CREATION" -ForegroundColor Yellow
Write-Host "=========================" -ForegroundColor Yellow

# Create update triggers for updated_at columns
$triggerFunction = @"
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS `$`$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
`$`$ language 'plpgsql';
"@

Execute-SQL $triggerFunction "Create update_updated_at_column function"

# Create triggers for tables with updated_at column
$tablesWithUpdatedAt = @(
    "users.roles",
    "users.users", 
    "content.lesson_templates",
    "content.lesson_plans",
    "assessment.questions",
    "assessment.exams",
    "students.classes",
    "students.students"
)

foreach ($table in $tablesWithUpdatedAt) {
    $triggerName = "trigger_updated_at_" + ($table -replace '\.', '_')
    $triggerSQL = "CREATE TRIGGER $triggerName BEFORE UPDATE ON $table FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();"
    Execute-SQL $triggerSQL "Create trigger $triggerName"
}

# Seed default roles
Write-Host ""
Write-Host "STEP 6: SEED DATA" -ForegroundColor Yellow
Write-Host "=================" -ForegroundColor Yellow

$seedRolesSql = @"
INSERT INTO users.roles (id, name, description, is_active, created_at, updated_at) VALUES
(1, 'ADMIN', 'Quản trị viên hệ thống', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'MANAGER', 'Quản lý nội dung và người dùng', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'STAFF', 'Nhân viên tạo nội dung mẫu', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'TEACHER', 'Giáo viên sử dụng hệ thống', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;
"@
Execute-SQL $seedRolesSql "Seed default roles"

Write-Host ""
Write-Host "STEP 7: VERIFICATION" -ForegroundColor Yellow
Write-Host "===================" -ForegroundColor Yellow

# Verify migration
Write-Host "[INFO] Verifying migration..." -ForegroundColor Cyan

$schemaCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name IN ('users', 'content', 'assessment', 'students');" 2>$null
$tableCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema IN ('users', 'content', 'assessment', 'students');" 2>$null

Write-Host "[INFO] Schemas created: $($schemaCount.Trim())/4" -ForegroundColor Cyan
Write-Host "[INFO] Tables created: $($tableCount.Trim())/12" -ForegroundColor Cyan

if ($schemaCount.Trim() -eq "4" -and $tableCount.Trim() -eq "12") {
    Write-Host ""
    Write-Host "SUCCESS! DATABASE MIGRATION COMPLETED!" -ForegroundColor Green
    Write-Host "=======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Database Structure:" -ForegroundColor Cyan
    Write-Host "  - 4 Schemas: users, content, assessment, students" -ForegroundColor White
    Write-Host "  - 12 Tables with full relationships" -ForegroundColor White
    Write-Host "  - All indexes and constraints created" -ForegroundColor White
    Write-Host "  - Update triggers implemented" -ForegroundColor White
    Write-Host ""
    Write-Host "Database is ready for development!" -ForegroundColor Green
Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
} else {
    Write-Host ""
    Write-Host "[ERROR] Migration incomplete. Please check the logs above." -ForegroundColor Red
    Write-Host ""
    Write-Host "Press any key to continue..." -ForegroundColor Cyan
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}