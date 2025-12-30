using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using PathologyFormApp.Models;

namespace PathologyFormApp.Migrations
{
    public partial class SeedRolesAndUsers : Migration
    {
        private static readonly string[] RoleColumns = ["Id", "Name", "NormalizedName", "ConcurrencyStamp"];
        private static readonly string[] UserColumns = [
            "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", 
            "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", 
            "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", 
            "LockoutEnabled", "AccessFailedCount", "FullName", "Role", "IsActive", "CreatedAt"
        ];
        private static readonly string[] UserRoleColumns = ["UserId", "RoleId"];

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create roles
            var doctorRoleId = Guid.NewGuid().ToString();
            var nurseRoleId = Guid.NewGuid().ToString();

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: RoleColumns,
                values: new object[,]
                {
                    {
                        doctorRoleId,
                        "Doctor",
                        "DOCTOR",
                        Guid.NewGuid().ToString()
                    },
                    {
                        nurseRoleId,
                        "Nurse",
                        "NURSE",
                        Guid.NewGuid().ToString()
                    }
                });

            // Create default admin user (password: Admin@123)
            var adminId = Guid.NewGuid().ToString();
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: UserColumns,
                values: new object[]
                {
                    adminId,
                    "admin",
                    "ADMIN",
                    "admin@hospital.com",
                    "ADMIN@HOSPITAL.COM",
                    true,
                    "AQAAAAEAACcQAAAAELbXp1QrHhX5YQrHhX5YQrHhX5YQrHhX5YQrHhX5YQrHhX5YQrHhX5YQrHhX5YQ==",
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    "",
                    false,
                    false,
                    DateTimeOffset.UtcNow.AddYears(100),
                    true,
                    0,
                    "System Administrator",
                    "Doctor",
                    true,
                    DateTime.UtcNow
                });

            // Assign admin user to Doctor role
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: UserRoleColumns,
                values: new object[]
                {
                    adminId,
                    doctorRoleId
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove users by NormalizedUserName
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "NormalizedUserName",
                keyValue: "ADMIN");

            // Remove roles by NormalizedName
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "NormalizedName",
                keyValue: "NURSE");
            
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "NormalizedName",
                keyValue: "DOCTOR");
        }
    }
} 