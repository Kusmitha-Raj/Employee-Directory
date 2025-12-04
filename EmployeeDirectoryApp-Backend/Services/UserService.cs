using EmployeeDirectoryApp.Data;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectoryApp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> AddEmployeeAsync(CreateEmployeeDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == dto.Email);

            if (existingUser != null && existingEmployee != null)
                return false;

            User user;
            if (existingUser == null)
            {
                var defaultPassword = dto.FirstName + "@123";
                user = new User
                {
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                    Role = "Employee",
                    MustChangePassword = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                user = existingUser;
            }

            if (existingEmployee == null)
            {
                var employee = new Employee
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Gender = dto.Gender,
                    JobRole = dto.JobRole,
                    DepartmentId = dto.DepartmentId,
                    PhoneNo = dto.PhoneNo,
                    UserId = user.UserId
                };
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> AddAdminAsync(CreateAdminDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return false;

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                MustChangePassword = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<RefreshTokens?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<bool> SaveRefreshTokenAsync(RefreshTokens token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRefreshTokenAsync(RefreshTokens token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
