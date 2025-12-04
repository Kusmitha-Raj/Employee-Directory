using EmployeeDirectoryApp.Controllers;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using EmployeeDirectoryApp.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeDirectoryApp.Tests.Controllers
{
    public class DepartmentControllerTests
    {
        [Fact]
        public async Task GetAllDepartments_Returns_Ok_With_List()
        {
            var mockService = new Mock<IDepartmentService>();
            mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<DepartmentResponseDTO>
                {
                    new DepartmentResponseDTO { DepartmentId = 1, Name = "IT" }
                });

            var controller = new DepartmentController(mockService.Object);

            var result = await controller.GetAllDepartments();

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();

            var value = ok!.Value as List<DepartmentResponseDTO>;
            value.Should().NotBeNull();
            value!.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddDepartment_Returns_BadRequest_On_Duplicate()
        {
            var mockService = new Mock<IDepartmentService>();
            mockService
                .Setup(s => s.AddAsync(It.IsAny<DepartmentDto>()))
                .ThrowsAsync(new InvalidOperationException("Department with this name already exists."));

            var controller = new DepartmentController(mockService.Object);

            var result = await controller.AddDepartment(new DepartmentDto { Name = "IT" });

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateDepartment_Returns_NotFound_When_Service_Returns_Null()
        {
            var mockService = new Mock<IDepartmentService>();
            mockService
                .Setup(s => s.UpdateAsync(1, It.IsAny<DepartmentUpdateDTO>()))
                .ReturnsAsync((Department?)null);

            var controller = new DepartmentController(mockService.Object);

            var result = await controller.UpdateDepartment(1, new DepartmentUpdateDTO { Name = "IT" });

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteDepartment_Returns_Ok_When_Success()
        {
            var mockService = new Mock<IDepartmentService>();
            mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var controller = new DepartmentController(mockService.Object);

            var result = await controller.DeleteDepartment(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteDepartment_Returns_NotFound_When_Fails()
        {
            var mockService = new Mock<IDepartmentService>();
            mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var controller = new DepartmentController(mockService.Object);

            var result = await controller.DeleteDepartment(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
