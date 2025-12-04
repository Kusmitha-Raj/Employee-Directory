namespace EmployeeDirectoryApp.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class ChangePasswordDto
    {
        public required string Email { get; set; }=null!;
        public string OldPassword { get; set; }= null!;
        public required string NewPassword { get; set; }= null!;
    }
    public class EmployeeResponseDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string JobRole { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }

    public class CreateEmployeeDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string JobRole { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string PhoneNo { get; set; } = null!;
    }
    public class CreateAdminDto
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!; 
    }
    public class UserDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }

}


