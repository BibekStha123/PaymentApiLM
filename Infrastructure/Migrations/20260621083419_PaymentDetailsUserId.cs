using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentDetailApi.Migrations
{
    /// <inheritdoc />
    public partial class PaymentDetailsUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardOwnerName",
                table: "PaymentDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PaymentDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_UserId",
                table: "PaymentDetails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDetails_Users_UserId",
                table: "PaymentDetails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDetails_Users_UserId",
                table: "PaymentDetails");

            migrationBuilder.DropIndex(
                name: "IX_PaymentDetails_UserId",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PaymentDetails");

            migrationBuilder.AddColumn<string>(
                name: "CardOwnerName",
                table: "PaymentDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
