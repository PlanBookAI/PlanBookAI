# Simple migration script
param(
    [string]$SqlFile = "planbookai2.sql"
)

$ErrorActionPreference = "Stop"

# Copy SQL file to container
Write-Host "Copying SQL file to container..."
docker cp "scripts/$SqlFile" planbookai-postgres-dev:/tmp/migration.sql

# Execute migration
Write-Host "Executing migration..."
docker exec planbookai-postgres-dev psql -U test -d planbookai -f /tmp/migration.sql

Write-Host "Migration completed!"
