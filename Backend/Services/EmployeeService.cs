using EmployeeDirectoryApp.Data;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace EmployeeDirectoryApp.Services

{
    public class EmployeeService:IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeResponseDTO>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.EmployeeId,
                    FullName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                    Email = e.Email,
                    PhoneNo = e.PhoneNo,
                    Gender = e.Gender,
                    JobRole = e.JobRole,
                    DepartmentName = e.Department != null ? e.Department.Name : null
                })
                .ToListAsync();
        }

        public async Task<EmployeeResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.EmployeeId == id)
                .Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.EmployeeId,
                    FullName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                    Email = e.Email,
                    PhoneNo = e.PhoneNo,
                    Gender = e.Gender,
                    JobRole = e.JobRole,
                    DepartmentName = e.Department != null ? e.Department.Name : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmployeeResponseDTO>> GetByNameAsync(string name)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name))
                .Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.EmployeeId,
                    FullName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                    Email = e.Email,
                    PhoneNo = e.PhoneNo,
                    Gender = e.Gender,
                    JobRole = e.JobRole,
                    DepartmentName = e.Department != null ? e.Department.Name : null
                })
                .ToListAsync();
        }

        public async Task<List<EmployeeResponseDTO>> GetByJobRoleAsync(string jobRole)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.JobRole == jobRole)
                .Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.EmployeeId,
                    FullName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                    Email = e.Email,
                    PhoneNo = e.PhoneNo,
                    Gender = e.Gender,
                    JobRole = e.JobRole,
                    DepartmentName = e.Department != null ? e.Department.Name : null
                })
                .ToListAsync();
        }

        public async Task<Employee?> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return null;

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.PhoneNo = dto.PhoneNo;
            employee.Gender = dto.Gender;
            employee.JobRole = dto.JobRole;
            employee.DepartmentId = dto.DepartmentId;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
