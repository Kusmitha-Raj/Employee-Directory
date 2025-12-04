using System;
using System.ComponentModel.DataAnnotations;
namespace EmployeeDirectoryApp.Models.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public ICollection<Employee> Employees { get; set; }
        //public ICollection<EmployeeProject> EmployeeProjects { get; set; }
        //   = new List<EmployeeProject>();
    }
    
}
