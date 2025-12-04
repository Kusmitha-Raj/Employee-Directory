using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDirectoryApp.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool MustChangePassword { get; set; } = true;
        public Employee? Employee { get; set; }
    }
}
