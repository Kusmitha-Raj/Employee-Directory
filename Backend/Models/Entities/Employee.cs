using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDirectoryApp.Models.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Gender { get; set; }
        public string JobRole { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        //public ICollection<EmployeeProject> EmployeeProjects { get; set; } =
        //  new List<EmployeeProject>();
    }
}
