using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using EmployeeDirectoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDirectoryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;
        private readonly IWebHostEnvironment _env;

        public UserController(IWebHostEnvironment env, IUserService userService, JwtService jwtService)
        {
            _env = env;
            _userService = userService;
            _jwtService = jwtService;
        }

        // 🧑‍💻 EMPLOYEE CREATION
        [HttpPost("AddEmployee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto dto)
        {
            var result = await _userService.AddEmployeeAsync(dto);
            if (!result) return BadRequest("Employee already exists");

            return Ok(new { message = "Employee created successfully" });
        }

        // 👑 ADMIN CREATION
        [HttpPost("AddAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] CreateAdminDto dto)
        {
            var success = await _userService.AddAdminAsync(dto);
            if (!success) return BadRequest("Admin already exists");

            return Ok(new { message = "Admin created successfully" });
        }

        // 🔐 LOGIN
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userService.ValidateUserAsync(dto.Email, dto.Password);
            if (user == null)
                return BadRequest("Invalid email or password");

            var accessToken = _jwtService.GenerateToken(user);
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var refreshToken = _jwtService.GenerateRefreshToken(ip ?? "unknown", user.UserId);

            await _userService.SaveRefreshTokenAsync(refreshToken);

            Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Login successful", token = accessToken, role = user.Role });
        }

        // 🔁 REFRESH TOKEN
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var token = request.RefreshToken ?? Request.Cookies["refreshToken"];
            if (token == null) return Unauthorized();

            var storedToken = await _userService.GetRefreshTokenAsync(token);
            if (storedToken == null || !storedToken.IsActive)
                return Unauthorized();

            storedToken.Revoked = DateTime.UtcNow;
            storedToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _userService.UpdateRefreshTokenAsync(storedToken);

            var newToken = _jwtService.GenerateRefreshToken(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", storedToken.UserId);
            await _userService.SaveRefreshTokenAsync(newToken);

            var user = storedToken.User!;
            var newAccessToken = _jwtService.GenerateToken(user);

            Response.Cookies.Append("refreshToken", newToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = newToken.Expires,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.None
            });

            return Ok(new { token = newAccessToken });
        }

        // 🚫 REVOKE TOKEN
        [HttpPost("Revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshRequest req)
        {
            var token = req.RefreshToken ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token)) return BadRequest("Token required");

            var stored = await _userService.GetRefreshTokenAsync(token);
            if (stored == null || stored.IsRevoked) return NotFound();

            stored.Revoked = DateTime.UtcNow;
            stored.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _userService.UpdateRefreshTokenAsync(stored);

            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Refresh token revoked" });
        }

        // 👀 GET ALL USERS
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }
    }
}
