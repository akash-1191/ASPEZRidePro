using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EZRide_Project.Migrations
{
    /// <inheritdoc />
    public partial class rebuildvehicletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Vehicles",
                newName: "Vehicletype");

            migrationBuilder.RenameColumn(
                name: "CarType",
                table: "Vehicles",
                newName: "CarName");

            migrationBuilder.RenameColumn(
                name: "BikeType",
                table: "Vehicles",
                newName: "BikeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Vehicletype",
                table: "Vehicles",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "CarName",
                table: "Vehicles",
                newName: "CarType");

            migrationBuilder.RenameColumn(
                name: "BikeName",
                table: "Vehicles",
                newName: "BikeType");
        }
    }
}
