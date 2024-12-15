using Microsoft.EntityFrameworkCore.Migrations;

namespace Delivery.DAL.Migrations
{
    public partial class NewCorrectMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FistName",
                table: "Couriers");

            migrationBuilder.DropColumn(
                name: "FistName",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Couriers",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Clients",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Couriers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "FistName",
                table: "Couriers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FistName",
                table: "Clients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
