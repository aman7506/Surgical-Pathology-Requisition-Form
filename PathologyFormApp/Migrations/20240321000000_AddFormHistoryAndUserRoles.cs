using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PathologyFormApp.Migrations
{
    public partial class AddFormHistoryAndUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Status column to PathologyForm
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PathologyForm",
                type: "nvarchar(max)",
                nullable: true);

            // Add CreatedById column
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "PathologyForm",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Add ReviewedById column
            migrationBuilder.AddColumn<string>(
                name: "ReviewedById",
                table: "PathologyForm",
                type: "nvarchar(450)",
                nullable: true);

            // Add ReviewedAt column
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "PathologyForm",
                type: "datetime2",
                nullable: true);

            // Create FormHistory table
            migrationBuilder.CreateTable(
                name: "FormHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormUHID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormHistory_PathologyForm_FormUHID",
                        column: x => x.FormUHID,
                        principalTable: "PathologyForm",
                        principalColumn: "UHID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_FormHistory_FormUHID",
                table: "FormHistory",
                column: "FormUHID");

            migrationBuilder.CreateIndex(
                name: "IX_FormHistory_UserId",
                table: "FormHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PathologyForm_CreatedById",
                table: "PathologyForm",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PathologyForm_ReviewedById",
                table: "PathologyForm",
                column: "ReviewedById");

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_PathologyForm_AspNetUsers_CreatedById",
                table: "PathologyForm",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PathologyForm_AspNetUsers_ReviewedById",
                table: "PathologyForm",
                column: "ReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PathologyForm_AspNetUsers_CreatedById",
                table: "PathologyForm");

            migrationBuilder.DropForeignKey(
                name: "FK_PathologyForm_AspNetUsers_ReviewedById",
                table: "PathologyForm");

            migrationBuilder.DropTable(
                name: "FormHistory");

            migrationBuilder.DropIndex(
                name: "IX_PathologyForm_CreatedById",
                table: "PathologyForm");

            migrationBuilder.DropIndex(
                name: "IX_PathologyForm_ReviewedById",
                table: "PathologyForm");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PathologyForm");

            migrationBuilder.DropColumn(
                name: "ReviewedById",
                table: "PathologyForm");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "PathologyForm");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PathologyForm");
        }
    }
} 