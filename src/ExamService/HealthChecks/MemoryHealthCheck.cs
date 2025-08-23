using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Kiểm tra dung lượng bộ nhớ mà service đang sử dụng
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var memoryThreshold = 1024L * 1024L * 500L; // 500 MB

        if (allocated < memoryThreshold)
        {
            return Task.FromResult(HealthCheckResult.Healthy(
                $"Sử dụng bộ nhớ: {allocated / 1024 / 1024} MB"
            ));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy(
            $"Sử dụng bộ nhớ vượt ngưỡng: {allocated / 1024 / 1024} MB"
        ));
    }
}