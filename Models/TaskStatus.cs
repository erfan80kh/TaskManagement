using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public enum TaskStatus
    {
        Pending = 0,    // در انتظار تأیید
        In_progress = 1,
        Completed = 2,
        Canceled = 3,
        OnHold = 4      // متوقف شده
    }
}
