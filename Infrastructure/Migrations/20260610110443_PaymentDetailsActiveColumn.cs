using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentDetailApi.Migrations
{
    /// <inheritdoc />
    public partial class PaymentDetailsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "PaymentDetails",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "PaymentDetails");
        }
    }
}
