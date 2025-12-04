using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;

namespace EmployeeDirectoryApp.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeResponseDTO>> GetAllAsync();
        Task<EmployeeResponseDTO?> GetByIdAsync(int id);
        Task<List<EmployeeResponseDTO>> GetByNameAsync(string name);
        Task<List<EmployeeResponseDTO>> GetByJobRoleAsync(string jobRole);
        Task<Employee?> UpdateAsync(int id, EmployeeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
