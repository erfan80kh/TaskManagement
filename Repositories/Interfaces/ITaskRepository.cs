using TaskManagement.Models;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
    Task<TaskItem?> GetTaskByIdAsync(int id, int userId, bool isAdmin);
    Task AddTaskAsync(TaskItem task);
    Task UpdateTaskAsync(int userId, TaskItem task, bool isAdmin);
    Task DeleteTaskAsync(int userId, int id, bool isAdmin);
    Task AssignTaskToUserAsync(int taskId, int userId);
}
