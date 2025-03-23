using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;



namespace TaskManagement.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id, int userId, bool isAdmin)
        {
            var task = await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.CreatedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return null;
            if (!isAdmin && task.UserId != userId) return null;

            return task;
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(int userId, TaskItem task, bool isAdmin)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);
            if (existingTask == null) throw new KeyNotFoundException("تسک یافت نشد.");

            if (!isAdmin && existingTask.UserId != userId)
                throw new UnauthorizedAccessException("شما اجازه ویرایش این تسک را ندارید.");

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.UpdatedAt = DateTime.UtcNow;

            _context.Tasks.Update(existingTask);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int userId, int id, bool isAdmin)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) throw new KeyNotFoundException("تسک یافت نشد.");

            if (!isAdmin && task.UserId != userId)
                throw new UnauthorizedAccessException("شما اجازه حذف این تسک را ندارید.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task AssignTaskToUserAsync(int taskId, int userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) throw new KeyNotFoundException("تسک یافت نشد.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException("کاربر یافت نشد.");

            task.UserId = userId;
            await _context.SaveChangesAsync();
        }
    }
}

