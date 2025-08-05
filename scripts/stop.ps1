# PlanbookAI - Stop All Services
Write-Host "Stopping all services..." -ForegroundColor Red

# Navigate to src directory
Set-Location "../src"

# Stop all services
docker-compose down

Write-Host "Services stopped!" -ForegroundColor Green

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 