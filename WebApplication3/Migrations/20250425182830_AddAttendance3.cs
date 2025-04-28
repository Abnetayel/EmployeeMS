using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendance3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AttendanceRate",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FeedbackScore",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OvertimeHours",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PerformanceScore",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectCount",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SkillLevel",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TaskCompletionRate",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TrainingHours",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WorkHours",
                table: "Employee",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceRate",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "FeedbackScore",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "PerformanceScore",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ProjectCount",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "TaskCompletionRate",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "TrainingHours",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "WorkHours",
                table: "Employee");
        }
    }
}
