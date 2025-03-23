using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

using TM = TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _env;

        public TaskController(ITaskRepository taskRepository, IUserRepository userRepository, IWebHostEnvironment env)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _env = env;
        }

        //  دریافت لیست تسک‌های کاربر لاگین شده
        [HttpGet("user")]
        public async Task<IActionResult> GetTasksByUser()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }

        //  دریافت جزئیات یک تسک ،برای مالک یا مدیر قابل دسترسی است
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool isAdmin = User.IsInRole("Admin");

            var task = await _taskRepository.GetTaskByIdAsync(id, userId, isAdmin);
            if (task == null)
                return NotFound("دسترسی غیرمجاز یا تسک یافت نشد.");

            return Ok(task);
        }

        //  ایجاد تسک 
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] TaskRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Status = TM.TaskStatus.Pending,
                Priority = request.Priority,
                UserId = userId, 
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddTaskAsync(task);
            return Ok(new { message = "تسک با موفقیت اضافه شد." });
        }

        //  ویرایش تسک (فقط توسط مالک یا مدیر)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskRequestDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool isAdmin = User.IsInRole("Admin");

            var existingTask = await _taskRepository.GetTaskByIdAsync(id, userId, isAdmin);
            if (existingTask == null)
                return NotFound("شما اجازه ویرایش این تسک را ندارید یا تسک یافت نشده.");

            existingTask.Title = request.Title;
            existingTask.Description = request.Description;
            existingTask.DueDate = request.DueDate;
            existingTask.Status = request.Status;
            existingTask.Priority = request.Priority;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateTaskAsync(userId, existingTask, isAdmin);
            return Ok(new { message = "تسک با موفقیت به‌روزرسانی شد." });
        }

        //  حذف تسک ،فقط توسط مالک یا مدیر
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool isAdmin = User.IsInRole("Admin");

            var task = await _taskRepository.GetTaskByIdAsync(id, userId, isAdmin);
            if (task == null)
                return NotFound("شما اجازه حذف این تسک را ندارید یا تسک یافت نشد.");

            await _taskRepository.DeleteTaskAsync(userId, id, isAdmin);
            return Ok(new { message = "تسک با موفقیت حذف شد." });
        }

        //  مدیر وضایف را به کاربر نخصیص دهدس
        [HttpPut("{taskId}/assign/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTaskToUser(int taskId, int userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId, 0, true);
            if (task == null)
                return NotFound("تسک یافت نشد.");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("کاربر یافت نشد.");

            await _taskRepository.AssignTaskToUserAsync(taskId, userId);
            return Ok(new { message = "وظیفه با موفقیت به کاربر اختصاص داده شد." });
        }

        
    }
}
