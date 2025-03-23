using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public UserController(IUserRepository userRepository, PasswordService passwordService, TokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        //  ثبت‌نام کاربر جدید
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("این ایمیل قبلاً ثبت شده است.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = UserRole.User 
            };

            await _userRepository.AddUserAsync(user);
            return Ok(new { message = "کاربر با موفقیت ثبت شد." });
        }

        //  ورود و دریافت توکن
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser == null || !_passwordService.VerifyPassword(request.Password, existingUser.PasswordHash))
                return Unauthorized("ایمیل یا رمز عبور اشتباه است.");

            if (!existingUser.IsActive)
                return Unauthorized("حساب کاربری شما غیرفعال شده است.");

            var token = _tokenService.GenerateJwtToken(existingUser);
            return Ok(new { token });
        }

        //  دریافت اطلاعات پروفایل کاربر ل
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("کاربر یافت نشد.");

            return Ok(new { user.FullName, user.Email, user.Role });
        }

        // ویرایش اطلاعات کاربر ،فقط خود کاربر می تواند
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                return NotFound("کاربر یافت نشد.");

            // بررسی تکراری نبودن ایمیل
            if (existingUser.Email != request.Email)
            {
                var emailExists = await _userRepository.GetUserByEmailAsync(request.Email);
                if (emailExists != null)
                    return BadRequest("این ایمیل قبلاً ثبت شده است.");
            }

            existingUser.FullName = request.FullName;
            existingUser.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                existingUser.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            }

            await _userRepository.UpdateUserAsync(existingUser);
            return Ok(new { message = "اطلاعات کاربر با موفقیت به‌روزرسانی شد." });
        }

        //  غیرفعال کردن کاربر ،فقط مدیر می تواند
        [HttpPut("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
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
