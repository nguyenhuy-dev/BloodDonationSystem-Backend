using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init_FixEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CreatorId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreatorId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreateBy",
                table: "Events",
                column: "CreateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_CreateBy",
                table: "Events",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CreateBy",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreateBy",
                table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Events",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatorId",
                table: "Events",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_CreatorId",
                table: "Events",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
