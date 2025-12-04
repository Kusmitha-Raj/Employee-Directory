using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDirectoryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // secure if you want; can remove for now
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var depts = await _service.GetAllAsync();
            return Ok(depts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var dept = await _service.GetByIdAsync(id);
            if (dept == null) return NotFound("Department not found");
            return Ok(dept);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentDto dto)
        {
            try
            {
                var created = await _service.AddAsync(dto);
                return Ok(created); // or map to DepartmentResponseDTO if you want
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdateDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Department not found");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound("Department not found");

            return Ok("Department deleted");
        }
    }
}
