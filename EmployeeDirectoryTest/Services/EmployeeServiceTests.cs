using System.Threading.Tasks;
using EmployeeDirectoryApp.Data;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using EmployeeDirectoryApp.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeDirectoryApp.Tests.Services
{
    public class EmployeeServiceTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Employee_When_Found()
        {
            var context = CreateContext(nameof(GetByIdAsync_Returns_Employee_When_Found));

            var dept = new Department { Name = "IT" };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();

            var emp = new Employee
            {
                FirstName = "Kusmitha",
                LastName = "Raj",
                Email = "k@example.com",
                PhoneNo = "11111",
                Gender = "Female",
                JobRole = "SDE Intern",
                DepartmentId = dept.DepartmentId
            };
            context.Employees.Add(emp);
            await context.SaveChangesAsync();

            var service = new EmployeeService(context);

            var result = await service.GetByIdAsync(emp.EmployeeId);

            result.Should().NotBeNull();
            result!.EmployeeId.Should().Be(emp.EmployeeId);
            result.FullName.Should().Be("Kusmitha Raj");
            result.Email.Should().Be("k@example.com");
            result.DepartmentName.Should().Be("IT");
        }

    }
}
