using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeDirectoryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewTableS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjects_Projects_ProjectId1",
                table: "EmployeeProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProjects_ProjectId1",
                table: "EmployeeProjects");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "EmployeeProjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId1",
                table: "EmployeeProjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjects_ProjectId1",
                table: "EmployeeProjects",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjects_Projects_ProjectId1",
                table: "EmployeeProjects",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "ProjectId");
        }
    }
}
