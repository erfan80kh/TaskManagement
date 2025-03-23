using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // دسترسی همه کاربران برای مدیر
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        //  id دریافت کاربر بر اساس 
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        // دریافت کاربر بر اساس ایمیل
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        // اضافه کردن کاربر جدید
        public async Task AddUserAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // به‌روزرسانی اطلاعات کاربر
        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                throw new KeyNotFoundException("کاربر یافت نشد.");

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.Role = user.Role;
            existingUser.UpdatedAt = DateTime.UtcNow; //  ثبت تاریخ آخرین ویرایش

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }

        // حذف کاربر
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("کاربر یافت نشد.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        //  غیرفعال کردن کاربر 
        public async Task DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("کاربر یافت نشد.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow; //  ثبت تاریخ غیرفعال شدن

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
