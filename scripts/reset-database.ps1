# Reset database script
$ErrorActionPreference = "Stop"

Write-Host "Stopping all connections to database..."
docker exec planbookai-postgres-dev psql -U test -d postgres -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'planbookai' AND pid <> pg_backend_pid();"

Write-Host "Dropping database..."
docker exec planbookai-postgres-dev psql -U test -d postgres -c "DROP DATABASE IF EXISTS planbookai;"

Write-Host "Creating new database..."
docker exec planbookai-postgres-dev psql -U test -d postgres -c "CREATE DATABASE planbookai;"

Write-Host "Database reset completed!"
