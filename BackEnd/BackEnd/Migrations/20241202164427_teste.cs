using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Reservations",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "checkInDate",
                table: "Reservations",
                newName: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Reservations",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Reservations",
                newName: "checkInDate");
        }
    }
}
