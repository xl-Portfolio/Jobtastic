using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobtastic.Migrations
{
    /// <inheritdoc />
    public partial class Anpassung1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactID",
                table: "Postings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Postings_ContactID",
                table: "Postings",
                column: "ContactID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyID",
                table: "AspNetUsers",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyID",
                table: "AspNetUsers",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Postings_Contacts_ContactID",
                table: "Postings",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Postings_Contacts_ContactID",
                table: "Postings");

            migrationBuilder.DropIndex(
                name: "IX_Postings_ContactID",
                table: "Postings");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ContactID",
                table: "Postings");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "AspNetUsers");
        }
    }
}
