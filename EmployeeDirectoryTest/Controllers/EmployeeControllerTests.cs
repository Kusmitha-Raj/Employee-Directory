using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeDirectoryApp.Controllers;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using EmployeeDirectoryApp.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EmployeeDirectoryApp.Tests.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _mockService;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _mockService = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_Returns_Ok_With_Employees()
        {
            // Arrange
            var employees = new List<EmployeeResponseDTO>
            {
                new EmployeeResponseDTO { EmployeeId = 1, FullName = "Test User" }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedEmployees = okResult.Value.Should().BeAssignableTo<List<EmployeeResponseDTO>>().Subject;

            returnedEmployees.Should().HaveCount(1);
            returnedEmployees[0].FullName.Should().Be("Test User");

            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeById_Returns_Ok_When_Found()
        {
            var employee = new EmployeeResponseDTO { EmployeeId = 1, FullName = "Tester" };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _controller.GetEmployeeById(1);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returned = okResult.Value.Should().BeAssignableTo<EmployeeResponseDTO>().Subject;
            returned.EmployeeId.Should().Be(1);
            returned.FullName.Should().Be("Tester");

            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeById_Returns_NotFound_When_Null()
        {
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((EmployeeResponseDTO?)null);

            var result = await _controller.GetEmployeeById(1);

            result.Should().BeOfType<NotFoundObjectResult>();
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_Returns_Ok_When_Success()
        {
            var dto = new EmployeeUpdateDto();
            var updatedEmployee = new Employee { EmployeeId = 1 };

            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(updatedEmployee);

            var result = await _controller.UpdateEmployee(1, dto);

            result.Should().BeOfType<OkObjectResult>();
            _mockService.Verify(s => s.UpdateAsync(1, dto), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_Returns_NotFound_When_EmployeeNotExists()
        {
            _mockService.Setup(s => s.UpdateAsync(10, It.IsAny<EmployeeUpdateDto>())).ReturnsAsync((Employee?)null);

            var result = await _controller.UpdateEmployee(10, new EmployeeUpdateDto());

            result.Should().BeOfType<NotFoundObjectResult>();
            _mockService.Verify(s => s.UpdateAsync(10, It.IsAny<EmployeeUpdateDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_Returns_Ok_When_Success()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteEmployee(1);

            result.Should().BeOfType<OkObjectResult>();
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_Returns_NotFound_When_NotExists()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeleteEmployee(1);

            result.Should().BeOfType<NotFoundObjectResult>();
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }
    }
}
