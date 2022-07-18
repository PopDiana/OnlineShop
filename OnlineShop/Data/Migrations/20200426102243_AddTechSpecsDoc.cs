using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityCore.Data.Migrations
{
    public partial class AddTechSpecsDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnicalSpecsDoc",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechnicalSpecsDoc",
                table: "Products");
        }
    }
}
