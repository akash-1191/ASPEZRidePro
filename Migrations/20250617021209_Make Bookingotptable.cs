using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EZRide_Project.Migrations
{
    /// <inheritdoc />
    public partial class MakeBookingotptable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailSentTo",
                table: "BookingOTPs",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailSentTo",
                table: "BookingOTPs");
        }
    }
}
