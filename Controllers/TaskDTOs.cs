using TM = TaskManagement.Models;

namespace TaskManagement.DTOs
{
    public class TaskRequestDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TM.TaskStatus Status { get; set; } = TM.TaskStatus.Pending;
    }
}
