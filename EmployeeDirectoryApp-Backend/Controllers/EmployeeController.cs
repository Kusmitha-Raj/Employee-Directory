using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDirectoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;  
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _employeeService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound("Employee not found");
            return Ok(employee);
        }

        [HttpGet("byName")]
        public async Task<IActionResult> GetByName(string name)
        {
            var employees = await _employeeService.GetByNameAsync(name);
            return Ok(employees);
        }

        [HttpGet("byJobRole")]
        public async Task<IActionResult> GetByJobRole(string jobRole)
        {
            var employees = await _employeeService.GetByJobRoleAsync(jobRole);
            return Ok(employees);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto dto)
        {
            var updated = await _employeeService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Employee not found");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _employeeService.DeleteAsync(id);
            if (!success) return NotFound("Employee not found");
            return Ok("Employee deleted");
        }
    }
}
