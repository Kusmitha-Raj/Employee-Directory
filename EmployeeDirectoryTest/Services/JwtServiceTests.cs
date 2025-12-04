using EmployeeDirectoryApp.Models.Entities;
using EmployeeDirectoryApp.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace EmployeeDirectoryApp.Tests.Auth
{
    public class JwtServiceTests
    {
        [Fact]
        public void GenerateToken_Returns_Valid_Jwt_With_Correct_Claims()
        {
            var configValues = new Dictionary<string, string>
            {
                { "Jwt:Key", "this_is_the_secret_key_for_testing_123456" },
                { "Jwt:Issuer", "test-issuer" },
                { "Jwt:Audience", "test-audience" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var jwtService = new JwtService(configuration);
            var user = new User
            {
                UserId = 10,
                Email = "user@test.com",
                PasswordHash = "dummyHash",
                Role = "Employee",
                MustChangePassword = false
            };

            var token = jwtService.GenerateToken(user);

            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
                .Should().Be("10");
            jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value
                .Should().Be("user@test.com");
            jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value
                .Should().Be("Employee");
        }
    }
}
