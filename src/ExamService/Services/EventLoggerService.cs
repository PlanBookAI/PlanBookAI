using ExamService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExamService.Services
{
    public class EventLoggerService : IEventLogger
    {
        private readonly ILogger<EventLoggerService> _logger;

        public EventLoggerService(ILogger<EventLoggerService> logger)
        {
            _logger = logger;
        }

        public void LogEvent<T>(string eventType, T eventPayload)
        {
            var payloadJson = JsonSerializer.Serialize(eventPayload);

            // Ghi log với LogLevel.Information và một định dạng có cấu trúc
            // Một hệ thống log tập trung (như ELK, Seq, Datadog) có thể dễ dàng lọc các log này
            _logger.LogInformation(
                "EVENT_TRIGGERED: Type={EventType}, Payload={EventPayload}",
                eventType,
                payloadJson);
        }
    }
}