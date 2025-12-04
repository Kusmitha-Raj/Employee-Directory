using EmployeeDirectoryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EmployeeDirectoryApp.Data.Configurations
{
    public class EmployeeConfig: IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.EmployeeId);
            builder.Property(e => e.EmployeeId).ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.PhoneNo).HasMaxLength(20);
            builder.Property(e => e.Gender).HasMaxLength(10);
            builder.Property(e => e.JobRole).HasMaxLength(50);

            builder.HasOne(e => e.Department)
                   .WithMany(d => d.Employees)
                   .HasForeignKey(e => e.DepartmentId);
            builder.
     HasOne(e => e.User)
    .WithOne(u => u.Employee)
    .HasForeignKey<Employee>(e => e.UserId)
    .OnDelete(DeleteBehavior.Cascade);
        }

    }
}