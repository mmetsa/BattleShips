using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    Player1 = table.Column<string>(nullable: false),
                    Player2 = table.Column<string>(nullable: false),
                    Player1Starts = table.Column<bool>(nullable: false),
                    Player1Ships = table.Column<string>(nullable: false),
                    Player2Ships = table.Column<string>(nullable: false),
                    ConsecutiveMovesOnHit = table.Column<bool>(nullable: false),
                    EBoatsCanTouch = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "GameStates",
                columns: table => new
                {
                    GameStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStates", x => x.GameStateId);
                    table.ForeignKey(
                        name: "FK_GameStates_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameStates_GameId",
                table: "GameStates",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStates");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
