using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; } // تاریخ سررسید

        public TaskStatus Status { get; set; } = TaskStatus.In_progress;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [MaxLength(500)]
        public string? FilePath { get; set; } // مسیر فایل پیوست شده

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // ارتباط با کاربر
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // ارتباط با کاربری که این تسک را ایجاد کرده است (مدیر یا خود کاربر
        public int? CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual User? CreatedByUser { get; set; } 
    }
}
