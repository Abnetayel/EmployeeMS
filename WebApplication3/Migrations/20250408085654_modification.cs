using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class modification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiddleName",
                table: "Employee",
                newName: "Skills");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Employee",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Employee",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Employee",
                newName: "EmployementType");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfRegistration",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Experience",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DateOfRegistration",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "Employee",
                newName: "MiddleName");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Employee",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Employee",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "EmployementType",
                table: "Employee",
                newName: "Email");
        }
    }
}
