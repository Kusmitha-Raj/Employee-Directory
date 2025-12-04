using EmployeeDirectoryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeeDirectoryApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }

}
}
