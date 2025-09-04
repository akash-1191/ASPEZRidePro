using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EZRide_Project.Migrations
{
    /// <inheritdoc />
    public partial class addcolumninthebookig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerKelomeater",
                table: "Bookings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerKelomeater",
                table: "Bookings");
        }
    }
}
