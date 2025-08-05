# PlanbookAI - Test Health
Write-Host "Testing services health..." -ForegroundColor Blue

$services = @(
    @{ Name = "Gateway"; Port = 8080 },
    @{ Name = "Auth"; Port = 8081 },
    @{ Name = "User"; Port = 8082 },
    @{ Name = "Plan"; Port = 8083 },
    @{ Name = "Task"; Port = 8084 }
)

foreach ($service in $services) {
    $url = "http://localhost:$($service.Port)/actuator/health"
    
    try {
        $response = Invoke-WebRequest -Uri $url -TimeoutSec 5 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "$($service.Name): OK" -ForegroundColor Green
        } else {
            Write-Host "$($service.Name): HTTP $($response.StatusCode)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "$($service.Name): Not ready" -ForegroundColor Red
    }
}

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 