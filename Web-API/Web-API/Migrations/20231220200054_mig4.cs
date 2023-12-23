using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActiveNotesID",
                table: "Students",
                newName: "StudentNotesID");

            migrationBuilder.RenameColumn(
                name: "ActiveLessonsID",
                table: "Students",
                newName: "StudentCourseID");

            migrationBuilder.RenameColumn(
                name: "LessonID",
                table: "StudentActiveLessons",
                newName: "CourseID");

            migrationBuilder.AddColumn<int>(
                name: "StudentNotesID",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentCoursesID",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentNotesID",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "StudentCoursesID",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "StudentNotesID",
                table: "Students",
                newName: "ActiveNotesID");

            migrationBuilder.RenameColumn(
                name: "StudentCourseID",
                table: "Students",
                newName: "ActiveLessonsID");

            migrationBuilder.RenameColumn(
                name: "CourseID",
                table: "StudentActiveLessons",
                newName: "LessonID");
        }
    }
}
