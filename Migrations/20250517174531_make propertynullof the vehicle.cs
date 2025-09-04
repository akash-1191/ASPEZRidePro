using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EZRide_Project.Migrations
{
    /// <inheritdoc />
    public partial class makepropertynullofthevehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CarName",
                table: "Vehicles",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "BikeName",
                table: "Vehicles",
                type: "varchar(80)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(80)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CarName",
                table: "Vehicles",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BikeName",
                table: "Vehicles",
                type: "varchar(80)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldNullable: true);
        }
    }
}
