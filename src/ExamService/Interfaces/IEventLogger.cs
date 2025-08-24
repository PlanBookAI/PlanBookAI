using System.Text.Json;

namespace ExamService.Interfaces
{
    public interface IEventLogger
    {
        void LogEvent<T>(string eventType, T eventPayload);
    }
}