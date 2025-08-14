# PlanbookAI Complete Database Migration Script
# Chạy migration hoàn chỉnh từ file SQL

Write-Host "PLANBOOKAI COMPLETE DATABASE MIGRATION" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

# Check if PostgreSQL container is running
$containerStatus = docker ps --filter "name=planbookai-postgres-dev" --format "{{.Names}}"
if (-not $containerStatus) {
    Write-Host "[ERROR] PostgreSQL container is not running!" -ForegroundColor Red
    Write-Host "Please run: .\start-dev-db.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "[INFO] PostgreSQL container is running" -ForegroundColor Green

# Database connection parameters
$DB_HOST = "localhost"
$DB_PORT = "5432"
$DB_NAME = "planbookai"
$DB_USER = "test"
$DB_PASSWORD = "test123"

# Function to execute SQL file
function Execute-SQLFile {
    param(
        [string]$SqlFile,
        [string]$Description
    )
    
    Write-Host "[INFO] $Description..." -ForegroundColor Cyan
    
    try {
        # Read SQL file content
        $sqlContent = Get-Content $SqlFile -Raw
        
        # Execute SQL content
        $result = docker exec planbookai-postgres-dev psql -U $DB_USER -d $DB_NAME -c "$sqlContent" 2>&1
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
Write-Host "STEP 2: RUNNING COMPLETE MIGRATION" -ForegroundColor Yellow
Write-Host "==================================" -ForegroundColor Yellow

# Check if migration file exists
$migrationFile = "complete-migration.sql"
if (-not (Test-Path $migrationFile)) {
    Write-Host "[ERROR] Migration file '$migrationFile' not found!" -ForegroundColor Red
    exit 1
}

Write-Host "[INFO] Found migration file: $migrationFile" -ForegroundColor Green

# Execute complete migration
$success = Execute-SQLFile $migrationFile "Complete database migration"

if ($success) {
    Write-Host ""
    Write-Host "STEP 3: VERIFICATION" -ForegroundColor Yellow
    Write-Host "====================" -ForegroundColor Yellow
    
    # Verify schemas
    Write-Host "[INFO] Verifying schemas..." -ForegroundColor Cyan
    Execute-SQL "SELECT schema_name FROM information_schema.schemata WHERE schema_name IN ('users', 'content', 'assessment', 'students') ORDER BY schema_name;" "List schemas"
    
    # Verify tables count
    Write-Host "[INFO] Verifying tables..." -ForegroundColor Cyan
    Execute-SQL "SELECT 'users' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'users' UNION ALL SELECT 'content' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'content' UNION ALL SELECT 'assessment' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'assessment' UNION ALL SELECT 'students' as schema_name, COUNT(*) as table_count FROM information_schema.tables WHERE table_schema = 'students';" "Count tables"
    
    # Verify roles
    Write-Host "[INFO] Verifying roles..." -ForegroundColor Cyan
    Execute-SQL "SELECT * FROM users.roles;" "List roles"
    
    Write-Host ""
    Write-Host "SUCCESS! COMPLETE DATABASE MIGRATION COMPLETED!" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Database Structure:" -ForegroundColor Yellow
    Write-Host "  - 4 Schemas: users, content, assessment, students" -ForegroundColor White
    Write-Host "  - All tables with full relationships" -ForegroundColor White
    Write-Host "  - All indexes and constraints created" -ForegroundColor White
    Write-Host "  - Update triggers implemented" -ForegroundColor White
    Write-Host "  - Seed data inserted" -ForegroundColor White
    Write-Host ""
    Write-Host "Database is ready for development!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "ERROR! MIGRATION FAILED!" -ForegroundColor Red
    Write-Host "=======================" -ForegroundColor Red
    Write-Host "Please check the error messages above and fix any issues." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
