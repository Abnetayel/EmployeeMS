using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class ml2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Employee",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Employee");
        }
    }
}
