using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 



namespace TaskManagement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [MinLength(8, ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد.")]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; }=true;

        // **لیست تسک‌هایی که به این کاربر تخصیص داده شده‌اند**
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        // **لیست تسک‌هایی که این کاربر (مدیر) ایجاد کرده است**
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
    }
}
