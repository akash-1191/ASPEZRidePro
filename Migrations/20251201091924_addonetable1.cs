using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EZRide_Project.Migrations
{
    /// <inheritdoc />
    public partial class addonetable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerDocument_Users_OwnerId",
                table: "OwnerDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnerDocument",
                table: "OwnerDocument");

            migrationBuilder.RenameTable(
                name: "OwnerDocument",
                newName: "OwnerDocuments");

            migrationBuilder.RenameIndex(
                name: "IX_OwnerDocument_OwnerId",
                table: "OwnerDocuments",
                newName: "IX_OwnerDocuments_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnerDocuments",
                table: "OwnerDocuments",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerDocuments_Users_OwnerId",
                table: "OwnerDocuments",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerDocuments_Users_OwnerId",
                table: "OwnerDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnerDocuments",
                table: "OwnerDocuments");

            migrationBuilder.RenameTable(
                name: "OwnerDocuments",
                newName: "OwnerDocument");

            migrationBuilder.RenameIndex(
                name: "IX_OwnerDocuments_OwnerId",
                table: "OwnerDocument",
                newName: "IX_OwnerDocument_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnerDocument",
                table: "OwnerDocument",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerDocument_Users_OwnerId",
                table: "OwnerDocument",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
