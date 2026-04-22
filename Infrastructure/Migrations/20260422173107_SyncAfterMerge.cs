using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LEAVE THIS COMPLETELY EMPTY!
            // Your database already has these columns from the merge. 
            // Running this empty method forces EF Core to mark the migration as applied 
            // and syncs your snapshot without crashing.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Students_StudentId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_LandLords_Users_UserId",
                table: "LandLords");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Students_StudentId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_City",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_LandLords_NationalId",
                table: "LandLords");

            migrationBuilder.DropIndex(
                name: "IX_LandLords_UserId",
                table: "LandLords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "LandLords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LandLords");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "LandLords");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "LandLords");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LandLords");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LandLords_UserId",
                table: "LandLords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Students_StudentId",
                table: "Complaints",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandLords_Users_UserId",
                table: "LandLords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Students_StudentId",
                table: "Reviews",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}