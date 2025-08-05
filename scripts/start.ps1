# PlanbookAI - Start All Services
Write-Host "Starting all services..." -ForegroundColor Green

# Navigate to src directory
Set-Location "../src"

# Start all services
docker-compose up --build -d

Write-Host "Services started!" -ForegroundColor Green
Write-Host "Gateway: http://localhost:8080" -ForegroundColor Yellow
Write-Host "Auth: http://localhost:8081" -ForegroundColor Yellow
Write-Host "User: http://localhost:8082" -ForegroundColor Yellow
Write-Host "Plan: http://localhost:8083" -ForegroundColor Yellow
Write-Host "Task: http://localhost:8084" -ForegroundColor Yellow

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 