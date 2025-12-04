using EmployeeDirectoryApp.Data;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeDirectoryApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentResponseDTO>> GetAllAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentResponseDTO
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name
                })
                .ToListAsync();
        }

        public async Task<DepartmentResponseDTO?> GetByIdAsync(int id)
        {
            var d = await _context.Departments.FindAsync(id);
            if (d == null) return null;

            return new DepartmentResponseDTO
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name
            };
        }

        public async Task<Department> AddAsync(DepartmentDto dto)
        {
            var existingDept = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.ToLower() == dto.Name.ToLower());

            if (existingDept != null)
                throw new InvalidOperationException("Department with this name already exists.");

            var department = new Department { Name = dto.Name };
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department?> UpdateAsync(int id, DepartmentUpdateDTO dto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return null;

            department.Name = dto.Name;
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return false;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
