using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // ذخیره مقدار متنی در JSON
    public enum UserRole
    {
        User,  // مقدار پیش‌فرض
        Admin  // مدیر سیستم
    }
}
