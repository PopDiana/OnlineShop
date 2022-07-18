using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityCore.Data.Migrations
{
    public partial class auctionsettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuctionAvailableUntil",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuctionLeadingUserId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HighestBid",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SecondHighestBid",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionAvailableUntil",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AuctionLeadingUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HighestBid",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SecondHighestBid",
                table: "Products");
        }
    }
}
