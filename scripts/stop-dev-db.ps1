# PlanbookAI Development Database Stop Script
# Dung PostgreSQL Docker containers

Write-Host "Stopping PlanbookAI Development Database" -ForegroundColor Red
Write-Host "=========================================" -ForegroundColor Red

# Stop containers
Write-Host "Stopping containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.dev.yml down

if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] Containers stopped successfully" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Failed to stop containers" -ForegroundColor Red
    exit 1
}

# Check status
Write-Host "Checking container status..." -ForegroundColor Yellow
$containerRunning = docker ps --filter "name=planbookai-postgres-dev" --format "{{.Names}}"
if (-not $containerRunning) {
    Write-Host "[OK] No containers running" -ForegroundColor Green
} else {
    Write-Host "[WARNING] Some containers may still be running" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "SUCCESS! Development Database stopped!" -ForegroundColor Green
Write-Host ""
Write-Host "RESTART COMMANDS:" -ForegroundColor Yellow
Write-Host "  Start again: .\scripts\start-dev-db.ps1" -ForegroundColor White
Write-Host ""
Write-Host "CLEANUP COMMANDS:" -ForegroundColor Red
Write-Host "  Remove all data: docker-compose -f docker-compose.dev.yml down -v" -ForegroundColor White
Write-Host "  Remove images: docker rmi postgres:17" -ForegroundColor White