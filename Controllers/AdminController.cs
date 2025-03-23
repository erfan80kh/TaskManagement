using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Repositories.Interfaces;

using TM = TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public AdminController(IUserRepository userRepository, ITaskRepository taskRepository)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        //  دریافت لیست 
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        //  دریافت لیست همه تسک‌ها
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return Ok(tasks);
        }

        //  دریافت آمار کلی وظایف
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var tasks = await _taskRepository.GetAllTasksAsync();

            var stats = new
            {
                TotalUsers = users.Count(),
                TotalTasks = tasks.Count(),
                CompletedTasks = tasks.Count(t => t.Status == TM.TaskStatus.Completed),
                InProgressTasks = tasks.Count(t => t.Status == TM.TaskStatus.In_progress),
                CanceledTasks = tasks.Count(t => t.Status == TM.TaskStatus.Canceled)
            };

            return Ok(stats);
        }

        //  غیرفعال کردن کاربر
        [HttpPut("deactivate-user/{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("کاربر یافت نشد.");

            await _userRepository.DeactivateUserAsync(id);
            return Ok(new { message = "کاربر با موفقیت غیرفعال شد." });
        }
    }
}
