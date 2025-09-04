using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EZRide_Project.Migrations
{
    /// <inheritdoc />
    public partial class setnullpropertyinthedocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DLImagePath",
                table: "CustomerDocuments",
                type: "varchar(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)");

            migrationBuilder.AlterColumn<string>(
                name: "AgeProofPath",
                table: "CustomerDocuments",
                type: "Varchar(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "Varchar(150)");

            migrationBuilder.AlterColumn<string>(
                name: "AddressProofPath",
                table: "CustomerDocuments",
                type: "Varchar(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "Varchar(150)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DLImagePath",
                table: "CustomerDocuments",
                type: "varchar(150)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AgeProofPath",
                table: "CustomerDocuments",
                type: "Varchar(150)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "Varchar(150)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AddressProofPath",
                table: "CustomerDocuments",
                type: "Varchar(150)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "Varchar(150)",
                oldNullable: true);
        }
    }
}
