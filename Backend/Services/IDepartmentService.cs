using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeDirectoryApp.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDTO>> GetAllAsync();
        Task<DepartmentResponseDTO?> GetByIdAsync(int id);
        Task<Department> AddAsync(DepartmentDto dto);
        Task<Department?> UpdateAsync(int id, DepartmentUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
