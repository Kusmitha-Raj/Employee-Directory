namespace EmployeeDirectoryApp.DTO
{
    public class DepartmentDto
    {
        public string Name { get; set; }
    }

    public class DepartmentUpdateDTO : DepartmentDto { }

    public class DepartmentResponseDTO
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
    }
}
