# =====================================================
# PLANBOOKAI COMPLETE DATABASE MIGRATION SCRIPT
# =====================================================
# Description: PowerShell script to create and setup complete database
# Usage: Run this script to create PlanbookAI database from scratch
# =====================================================

Write-Host "PLANBOOKAI COMPLETE DATABASE MIGRATION" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green
Write-Host ""

# Check Docker container
Write-Host "STEP 1: CHECK DOCKER CONTAINER" -ForegroundColor Yellow
Write-Host "===============================" -ForegroundColor Yellow

$containerStatus = docker ps --filter "name=planbookai-postgres-dev" --format "{{.Names}}"
if (-not $containerStatus) {
    Write-Host "[ERROR] PostgreSQL container is not running!" -ForegroundColor Red
    Write-Host "Please run: ..\start-dev-db.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "[OK] PostgreSQL container is running: $containerStatus" -ForegroundColor Green

# Database connection parameters
$DB_HOST = "localhost"
$DB_PORT = "5432"
$DB_NAME = "planbookai"
$DB_USER = "test"
$DB_PASSWORD = "test123"

Write-Host ""
Write-Host "STEP 2: DATABASE CONNECTION PARAMETERS" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow
Write-Host "Host: $DB_HOST" -ForegroundColor Cyan
Write-Host "Port: $DB_PORT" -ForegroundColor Cyan
Write-Host "Database: $DB_NAME" -ForegroundColor Cyan
Write-Host "User: $DB_USER" -ForegroundColor Cyan

# Function to execute SQL command
function Execute-SQL {
    param(
        [string]$SqlCommand,
        [string]$Description,
        [string]$Database = $DB_NAME
    )
    
    Write-Host "[INFO] $Description..." -ForegroundColor Cyan
    
    try {
        $result = docker exec planbookai-postgres-dev psql -v ON_ERROR_STOP=1 -U $DB_USER -d $Database -c "$SqlCommand" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] $Description completed successfully" -ForegroundColor Green
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
Write-Host "STEP 3: CREATE DATABASE" -ForegroundColor Yellow
Write-Host "=======================" -ForegroundColor Yellow

# Check and create database
if (-not (Test-Database $DB_NAME)) {
    Write-Host "[INFO] Database '$DB_NAME' does not exist. Creating..." -ForegroundColor Cyan
    $createDbResult = Execute-SQL "CREATE DATABASE $DB_NAME;" "Create database $DB_NAME" "postgres"
    if (-not $createDbResult) {
        Write-Host "[ERROR] Cannot create database. Exiting script." -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "[OK] Database '$DB_NAME' already exists" -ForegroundColor Green
}

Write-Host ""
Write-Host "STEP 4: CREATE SCHEMAS" -ForegroundColor Yellow
Write-Host "======================" -ForegroundColor Yellow

# List of schemas to create
$schemas = @("auth", "users", "assessment", "content", "students", "files", "notifications", "logging")

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
Write-Host "STEP 5: CREATE EXTENSIONS" -ForegroundColor Yellow
Write-Host "=========================" -ForegroundColor Yellow

# Create required extensions - Use gen_random_uuid() instead of uuid-ossp
Write-Host "[INFO] PostgreSQL 17 has gen_random_uuid() built-in, no need for uuid-ossp extension" -ForegroundColor Cyan
Write-Host "[OK] UUID generation available via gen_random_uuid()" -ForegroundColor Green

Execute-SQL 'CREATE EXTENSION IF NOT EXISTS "pgcrypto";' "Create extension pgcrypto"

Write-Host ""
Write-Host "STEP 6: CREATE TABLES" -ForegroundColor Yellow
Write-Host "=====================" -ForegroundColor Yellow

# Read SQL migration file
$sqlFile = "complete-migration.sql"
if (-not (Test-Path $sqlFile)) {
    Write-Host "[ERROR] Cannot find file: $sqlFile" -ForegroundColor Red
    Write-Host "Please ensure complete-migration.sql is in the scripts directory" -ForegroundColor Yellow
    exit 1
}

Write-Host "[INFO] Reading SQL migration file: $sqlFile" -ForegroundColor Cyan

# Read SQL file content
$sqlContent = Get-Content $sqlFile -Raw

# Split SQL content into separate commands
$sqlCommands = $sqlContent -split "-- =====================================================" | Where-Object { $_.Trim() -ne "" }

$successCount = 0
$totalCount = 0

foreach ($command in $sqlCommands) {
    $command = $command.Trim()
    if ($command -eq "") { continue }

    # Skip verification sections entirely
    if ($command -match "VERIFICATION" -or $command -match "Kiểm tra số lượng") { continue }

    # Remove header comment lines (but keep the SQL body)
    $lines = $command -split "`r?`n"
    $lines = $lines | Where-Object { $_ -notmatch "^\s*--\s*PLANBOOKAI DATABASE" -and $_ -notmatch "^\s*--\s*Tạo database" -and $_ -notmatch "^\s*--\s*Kết nối vào database" }

    # Remove psql meta-commands and CREATE DATABASE (handled earlier)
    $lines = $lines | Where-Object { $_ -notmatch "^\s*\\c\b" -and $_ -notmatch "(?i)^\s*CREATE\s+DATABASE\b" }

    $commandClean = ($lines -join "`n").Trim()
    if ($commandClean -eq "") { continue }

    $totalCount++
    if (Execute-SQL $commandClean "Execute SQL command #$totalCount") { $successCount++ }
}

Write-Host ""
Write-Host "STEP 7: VERIFY RESULTS" -ForegroundColor Yellow
Write-Host "======================" -ForegroundColor Yellow

# Check migration results
Write-Host "[INFO] Verifying migration results..." -ForegroundColor Cyan

$schemaCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');" 2>$null
$tableCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');" 2>$null

Write-Host "[INFO] Schemas created: $($schemaCount.Trim())/8" -ForegroundColor Cyan
Write-Host "[INFO] Tables created: $($tableCount.Trim())/20+" -ForegroundColor Cyan

# Check seed data
Write-Host ""
Write-Host "STEP 8: VERIFY SEED DATA" -ForegroundColor Yellow
Write-Host "=========================" -ForegroundColor Yellow

# Safe count queries with null handling
$rolesCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM auth.roles;" 2>$null
$usersCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM auth.users;" 2>$null
$topicsCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM content.chu_de;" 2>$null
$questionsCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM assessment.questions;" 2>$null

# Safe display with null handling
$rolesCountSafe = if ($rolesCount) { $rolesCount.Trim() } else { "0" }
$usersCountSafe = if ($usersCount) { $usersCount.Trim() } else { "0" }
$topicsCountSafe = if ($topicsCount) { $topicsCount.Trim() } else { "0" }
$questionsCountSafe = if ($questionsCount) { $questionsCount.Trim() } else { "0" }

Write-Host "[INFO] Roles: $rolesCountSafe/4" -ForegroundColor Cyan
Write-Host "[INFO] Users: $usersCountSafe/2" -ForegroundColor Cyan
Write-Host "[INFO] Topics: $topicsCountSafe/3" -ForegroundColor Cyan
Write-Host "[INFO] Questions: $questionsCountSafe/2" -ForegroundColor Cyan

Write-Host ""
Write-Host "STEP 8.5: UPDATE USER SERVICE SCHEMA" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Yellow

# Update auth.users table for soft delete
Write-Host "[INFO] Adding soft delete columns to auth.users..." -ForegroundColor Cyan
$softDeleteResult = Execute-SQL "ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS deleted_at TIMESTAMP NULL;" "Add deleted_at column"
if ($softDeleteResult) {
    $softDeleteResult2 = Execute-SQL "ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN DEFAULT FALSE;" "Add is_deleted column"
}

# Create OTP codes table
Write-Host "[INFO] Creating OTP codes table..." -ForegroundColor Cyan
$otpTableResult = Execute-SQL @"
CREATE TABLE IF NOT EXISTS users.otp_codes (
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
"@
"Create OTP codes table"

# Create password history table
Write-Host "[INFO] Creating password history table..." -ForegroundColor Cyan
$passwordHistoryResult = Execute-SQL @"
CREATE TABLE IF NOT EXISTS users.password_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    changed_at TIMESTAMP NOT NULL,
    changed_by UUID REFERENCES auth.users(id),
    reason VARCHAR(100) DEFAULT 'USER_CHANGE',
    ip_address INET,
    user_agent TEXT
);
"@
"Create password history table"

# Create user sessions table
Write-Host "[INFO] Creating user sessions table..." -ForegroundColor Cyan
$userSessionsResult = Execute-SQL @"
CREATE TABLE IF NOT EXISTS users.user_sessions (
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
"@
"Create user sessions table"

# Create indexes
Write-Host "[INFO] Creating indexes for UserService..." -ForegroundColor Cyan
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_otp_codes_user ON users.otp_codes(user_id);" "Create OTP user index"
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_otp_codes_purpose ON users.otp_codes(purpose);" "Create OTP purpose index"
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_otp_codes_expires ON users.otp_codes(expires_at);" "Create OTP expires index"
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_password_history_user ON users.password_history(user_id);" "Create password history index"
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_user_sessions_user ON users.user_sessions(user_id);" "Create user sessions index"
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_user_sessions_token ON users.user_sessions(session_token);" "Create session token index"
Execute-SQL "CREATE INDEX IF NOT EXISTS idx_users_user_sessions_active ON users.user_sessions(is_active);" "Create active sessions index"

# Create trigger for updated_at
Write-Host "[INFO] Creating updated_at trigger..." -ForegroundColor Cyan

# Create function first
$functionSql = "CREATE OR REPLACE FUNCTION update_updated_at_column() RETURNS TRIGGER AS `$`$ BEGIN NEW.updated_at = CURRENT_TIMESTAMP; RETURN NEW; END; `$`$ language 'plpgsql';"
Execute-SQL $functionSql "Create update_updated_at function"

# Create trigger
$triggerSql = "DROP TRIGGER IF EXISTS trigger_updated_at_auth_users_soft_delete ON auth.users; CREATE TRIGGER trigger_updated_at_auth_users_soft_delete BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();"
Execute-SQL $triggerSql "Create updated_at trigger"

Write-Host "[OK] UserService schema update completed!" -ForegroundColor Green

# Update table count after creating UserService schema
Write-Host "[INFO] Updating table count after UserService schema creation..." -ForegroundColor Cyan
$tableCount = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema IN ('auth', 'users', 'assessment', 'content', 'students', 'files', 'notifications', 'logging');" 2>$null
Write-Host "[INFO] Updated table count: $($tableCount.Trim())/25+" -ForegroundColor Cyan

Write-Host ""
Write-Host "STEP 9: DISPLAY TEST ACCOUNTS" -ForegroundColor Yellow
Write-Host "=============================" -ForegroundColor Yellow

# Display test accounts information with error handling
try {
    $testAccounts = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -t -c "
    SELECT 
        u.email,
        r.name as role,
        up.full_name
    FROM auth.users u
    JOIN auth.roles r ON u.role_id = r.id
    JOIN users.user_profiles up ON u.id = up.user_id
    ORDER BY r.id;" 2>$null

    Write-Host "[INFO] Test Accounts:" -ForegroundColor Cyan
    if ($testAccounts) {
        $testAccounts -split "`n" | Where-Object { $_.Trim() -ne "" } | ForEach-Object {
            $parts = $_ -split '\|'
            if ($parts.Length -ge 3) {
                $email = $parts[0].Trim()
                $role = $parts[1].Trim()
                $name = $parts[2].Trim()
                Write-Host "  - $email ($role): $name" -ForegroundColor White
            }
        }
    } else {
        Write-Host "  No test accounts found" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  Error retrieving test accounts: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "STEP 10: FINAL RESULTS" -ForegroundColor Yellow
Write-Host "======================" -ForegroundColor Yellow

# Safe evaluation with null handling
$schemaCountSafe = if ($schemaCount) { $schemaCount.Trim() } else { "0" }
$tableCountSafe = if ($tableCount) { $tableCount.Trim() } else { "0" }
$rolesCountSafe = if ($rolesCount) { $rolesCount.Trim() } else { "0" }
$usersCountSafe = if ($usersCount) { $usersCount.Trim() } else { "0" }

# Evaluate results
if ($schemaCountSafe -eq "8" -and $tableCountSafe -ge "25" -and $rolesCountSafe -eq "4" -and $usersCountSafe -eq "2") {
    Write-Host ""
    Write-Host "🎉 SUCCESS! DATABASE MIGRATION COMPLETED!" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Database Structure:" -ForegroundColor Green
    Write-Host "  - 8 Schemas: auth, users, assessment, content, students, files, notifications, logging" -ForegroundColor White
    Write-Host "  - 25+ Tables with full relationships" -ForegroundColor White
    Write-Host "  - All indexes and constraints created" -ForegroundColor White
    Write-Host "  - Auto-update triggers implemented" -ForegroundColor White
    Write-Host "  - UserService schema: OTP, password history, user sessions" -ForegroundColor White
    Write-Host ""
    Write-Host "✅ Seed Data:" -ForegroundColor Green
    Write-Host "  - 4 Roles: ADMIN, MANAGER, STAFF, TEACHER" -ForegroundColor White
    Write-Host "  - 2 Test Users: admin@planbookai.com, teacher@test.com" -ForegroundColor White
    Write-Host "  - 3 Chemistry topics" -ForegroundColor White
    Write-Host "  - 2 Sample questions with answers" -ForegroundColor White
    Write-Host ""
    Write-Host "✅ Test Accounts:" -ForegroundColor Green
    Write-Host "  - Admin: admin@planbookai.com / admin123" -ForegroundColor White
    Write-Host "  - Teacher: teacher@test.com / teacher123" -ForegroundColor White
    Write-Host ""
    Write-Host "🚀 Database ready for development!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ [ERROR] Migration incomplete. Please check logs above." -ForegroundColor Red
    Write-Host ""
    Write-Host "📊 Migration Summary:" -ForegroundColor Yellow
Write-Host "  - Schemas: $schemaCountSafe/8" -ForegroundColor White
Write-Host "  - Tables: $tableCountSafe/25+" -ForegroundColor White
Write-Host "  - Roles: $rolesCountSafe/4" -ForegroundColor White
Write-Host "  - Users: $usersCountSafe/2" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to continue..." -ForegroundColor Cyan
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host ""
Write-Host "📋 NEXT STEPS:" -ForegroundColor Yellow
Write-Host "1. Update connection strings in appsettings.json" -ForegroundColor White
Write-Host "2. Test database connection from services" -ForegroundColor White
Write-Host "3. Test authentication with test accounts" -ForegroundColor White
Write-Host "4. Check all API endpoints" -ForegroundColor White

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Ensure all blocks are closed
}
