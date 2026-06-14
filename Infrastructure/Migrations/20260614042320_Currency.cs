using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentDetailApi.Migrations
{
    /// <inheritdoc />
    public partial class Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                    table.UniqueConstraint("AK_Currency_CurrencyCode", x => x.CurrencyCode);
                });

            //Insert data from source database
            migrationBuilder.Sql(@"
                INSERT INTO PaymentDetailDb.dbo.Currency (CurrencyCode, Name, ModifiedDate)
                SELECT CurrencyCode, Name, ModifiedDate
                FROM AdventureWorks2022.Sales.Currency
            ");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Currency");
        }
    }
}
