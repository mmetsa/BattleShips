using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class AddGameCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameCode",
                table: "Games",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameCode",
                table: "Games");
        }
    }
}
