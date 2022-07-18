using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityCore.Data.Migrations
{
    public partial class AuctionProduct1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Auctioned",
                table: "Products",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Auctioned",
                table: "Products",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
