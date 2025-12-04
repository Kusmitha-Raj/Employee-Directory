using EmployeeDirectoryApp.Models.Entities;

namespace EmployeeDirectoryApp.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        RefreshToken GenerateRefreshToken(string ipAddress, int userId);
    }
}
