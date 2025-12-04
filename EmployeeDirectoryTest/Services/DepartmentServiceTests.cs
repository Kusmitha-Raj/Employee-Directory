using System;
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
    public class DepartmentServiceTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Adds_New_Department()
        {
            var context = CreateContext(nameof(AddAsync_Adds_New_Department));
            var service = new DepartmentService(context);

            var dto = new DepartmentDto { Name = "HR" };

            var created = await service.AddAsync(dto);

            created.DepartmentId.Should().BeGreaterThan(0);
            created.Name.Should().Be("HR");
        }

        [Fact]
        public async Task AddAsync_Throws_When_Department_Name_Exists()
        {
            var context = CreateContext(nameof(AddAsync_Throws_When_Department_Name_Exists));
            context.Departments.Add(new Department { Name = "IT" });
            await context.SaveChangesAsync();

            var service = new DepartmentService(context);
            var dto = new DepartmentDto { Name = "IT" };

            Func<Task> act = async () => await service.AddAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateAsync_Updates_Existing_Department()
        {
            var context = CreateContext(nameof(UpdateAsync_Updates_Existing_Department));
            var dept = new Department { Name = "OldName" };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();

            var service = new DepartmentService(context);
            var dto = new DepartmentUpdateDTO { Name = "NewName" };

            var updated = await service.UpdateAsync(dept.DepartmentId, dto);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("NewName");
        }

        [Fact]
        public async Task UpdateAsync_Returns_Null_When_Department_NotFound()
        {
            var context = CreateContext(nameof(UpdateAsync_Returns_Null_When_Department_NotFound));
            var service = new DepartmentService(context);

            var dto = new DepartmentUpdateDTO { Name = "NewName" };

            var updated = await service.UpdateAsync(999, dto);

            updated.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Removes_Department_And_Returns_True()
        {
            var context = CreateContext(nameof(DeleteAsync_Removes_Department_And_Returns_True));
            var dept = new Department { Name = "ToDelete" };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();

            var service = new DepartmentService(context);

            var result = await service.DeleteAsync(dept.DepartmentId);

            result.Should().BeTrue();
            (await context.Departments.FindAsync(dept.DepartmentId)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Returns_False_When_Department_NotFound()
        {
            var context = CreateContext(nameof(DeleteAsync_Returns_False_When_Department_NotFound));
            var service = new DepartmentService(context);

            var result = await service.DeleteAsync(999);

            result.Should().BeFalse();
        }
    }
}
