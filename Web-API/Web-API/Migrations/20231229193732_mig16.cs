using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class mig16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeID",
                table: "EmployeeCourses");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeNumber",
                table: "EmployeeCourses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeNumber",
                table: "EmployeeCourses");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeID",
                table: "EmployeeCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
