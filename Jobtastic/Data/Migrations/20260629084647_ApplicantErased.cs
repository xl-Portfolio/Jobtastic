using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobtastic.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicantErased : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_CompanyContacts_ContactID",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_CompanyContacts_UserID",
                table: "CompanyContacts");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_UserID",
                table: "CompanyContacts",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_CompanyContacts_ContactID",
                table: "JobPostings",
                column: "ContactID",
                principalTable: "CompanyContacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_CompanyContacts_ContactID",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_CompanyContacts_UserID",
                table: "CompanyContacts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfileInfo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Applicants_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    PostingID = table.Column<int>(type: "int", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UploadedDocumentsSource = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_JobPostings_PostingID",
                        column: x => x.PostingID,
                        principalTable: "JobPostings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_UserID",
                table: "CompanyContacts",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_UserID",
                table: "Applicants",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantID",
                table: "Applications",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_PostingID",
                table: "Applications",
                column: "PostingID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_CompanyContacts_ContactID",
                table: "JobPostings",
                column: "ContactID",
                principalTable: "CompanyContacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
