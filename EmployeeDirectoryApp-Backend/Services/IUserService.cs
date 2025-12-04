using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;

public interface IUserService
{
    Task<User?> ValidateUserAsync(string email, string password);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> AddEmployeeAsync(CreateEmployeeDto dto);
    Task<bool> AddAdminAsync(CreateAdminDto dto);
    Task<List<UserDto>> GetAllAsync();

    Task<RefreshTokens?> GetRefreshTokenAsync(string token);
    Task<bool> SaveRefreshTokenAsync(RefreshTokens token);
    Task<bool> UpdateRefreshTokenAsync(RefreshTokens token);
}
