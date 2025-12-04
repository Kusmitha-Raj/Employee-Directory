namespace EmployeeDirectoryApp.DTO
{
    public class EmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Gender { get; set; }
        public string JobRole { get; set; }
        public int DepartmentId { get; set; }
    }

    public class EmployeeUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string JobRole { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string PhoneNo { get; set; } = null!;
    }

    public class EmployeeResponseDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Gender { get; set; }
        public string JobRole { get; set; }
        public string DepartmentName { get; set; }
    }
}