using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GameState = Domain.GameState;

namespace WebApp.Pages.Gameplay
{
    public class Play : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BattleShipsDb _context;

        public Play(ILogger<IndexModel> logger, BattleShipsDb context)
        {
            _logger = logger;
            _context = context;
            GameBrain = new Battleships();
        }

        public Game? Game { get; set; }
        public Battleships? GameBrain { get; set; }

        public bool Rotation { get; set; }

        public bool HitScreen { get; set; }

        public bool GameOver { get; set; }

        public async Task OnGetAsync(int id, int? x, int? y, int? board, bool? rotation, bool random)
        {
            Rotation = rotation ?? false;
            Game = await _context.Games
                .Where(b => b.GameId == id)
                .Include(b => b.GameStates).FirstOrDefaultAsync();

            if (Game?.GameStates != null && Game.GameStates.Count > 0)
            {
                GameBrain!.GameStates.Add(Game.GameStates.Last().State);
            }
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            GameBrain!.CreateGameFromJsonString(JsonSerializer.Serialize(Game, jsonOptions));

            if (random)
            {
                Game!.Player1Ships = "[]";
                Game!.Player2Ships = "[]";
                GameBrain.GameOptions.Player1Ships.Sort((xx,yy) => xx.Size.CompareTo(yy.Size));
                GameBrain.GameOptions.Player1Ships.Reverse();
                foreach (var ship in GameBrain.GameOptions.Player1Ships.ToList())
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        var vertical = new Random().Next(1, 3) == 1;
                        var x1 = vertical
                            ? new Random().Next(1, GameBrain.GameOptions.Width + 1)
                            : new Random().Next(1, GameBrain.GameOptions.Width - ship.Size + 2);
                        var y1 = vertical
                            ? new Random().Next(1, GameBrain.GameOptions.Height - ship.Size + 2)
                            : new Random().Next(1, GameBrain.GameOptions.Height + 1);
                        var placed = GameBrain.PlaceShip(x1, y1, ship, true, vertical);
                        if (placed)
                        {
                            GameBrain.GameOptions.Player1Ships.RemoveAt(0);
                            break;
                        }

                        if (placed || i != 999) continue;
                        for (var j = 0; j < GameBrain.GetBoard(false).GetLength(0); j++)
                        {
                            for (var k = 0; k < GameBrain.GetBoard(false).GetLength(1); k++)
                            {
                                GameBrain.GetBoard(false)[j, k] = new CellState();
                            }
                        }
                    }
                }
                
                GameBrain.GameOptions.Player2Ships.Sort((xx,yy) => xx.Size.CompareTo(yy.Size));
                GameBrain.GameOptions.Player2Ships.Reverse();
                foreach (var ship in GameBrain.GameOptions.Player2Ships.ToList())
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        var vertical = new Random().Next(1, 3) == 1;
                        var x1 = vertical
                            ? new Random().Next(1, GameBrain.GameOptions.Width + 1)
                            : new Random().Next(1, GameBrain.GameOptions.Width - ship.Size + 2);
                        var y1 = vertical
                            ? new Random().Next(1, GameBrain.GameOptions.Height - ship.Size + 2)
                            : new Random().Next(1, GameBrain.GameOptions.Height + 1);
                        var placed = GameBrain.PlaceShip(x1, y1, ship, false, vertical);
                        if (placed)
                        {
                            GameBrain.GameOptions.Player2Ships.RemoveAt(0);
                            break;
                        }

                        if (placed || i != 999) continue;
                        for (var j = 0; j < GameBrain.GetBoard(true).GetLength(0); j++)
                        {
                            for (var k = 0; k < GameBrain.GetBoard(true).GetLength(1); k++)
                            {
                                GameBrain.GetBoard(true)[j, k] = new CellState();
                            }
                        }
                    }
                }
            }

            if (x != null && y != null)
            {
                if (GameBrain.NextMoveByP1 && GameBrain.GameOptions.Player1Ships.Count != 0 && board == 2
                || !GameBrain.NextMoveByP1 && GameBrain.GameOptions.Player2Ships.Count != 0 && board == 2
                || GameBrain.NextMoveByP1 && GameBrain.GameOptions.Player1Ships.Count == 0 && board == 1
                || !GameBrain.NextMoveByP1 && GameBrain.GameOptions.Player2Ships.Count == 0 && board == 1)
                {
                    return;
                }
                if (GameBrain.NextMoveByP1 && GameBrain.GameOptions.Player1Ships.Count != 0)
                {
                    if (GameBrain.PlaceShip(x.Value, y.Value, GameBrain.GameOptions.Player1Ships.First(), true, rotation!.Value))
                    {
                        GameBrain.GameOptions.Player1Ships.RemoveAt(0);
                        Game!.Player1Ships = JsonSerializer.Serialize(GameBrain.GameOptions.Player1Ships);
                        if (GameBrain.GameOptions.Player1Ships.Count == 0)
                        {
                            GameBrain.NextMoveByP1 = !GameBrain.NextMoveByP1;
                        }
                    }
                } else if (!GameBrain.NextMoveByP1 && GameBrain.GameOptions.Player2Ships.Count != 0)
                {
                    if (GameBrain.GameOptions.Player2Ships.Count != 0)
                    {
                        if (GameBrain.PlaceShip(x.Value, y.Value, GameBrain.GameOptions.Player2Ships.First(), false, rotation!.Value))
                        {
                            GameBrain.GameOptions.Player2Ships.RemoveAt(0);
                            Game!.Player2Ships = JsonSerializer.Serialize(GameBrain.GameOptions.Player2Ships);
                            if (GameBrain.GameOptions.Player2Ships.Count == 0)
                            {
                                GameBrain.NextMoveByP1 = !GameBrain.NextMoveByP1;
                            }
                        }
                    }
                }
                else if (GameBrain.GameOptions.Player1Ships.Count == 0 && GameBrain.GameOptions.Player2Ships.Count == 0)
                {
                    var player = GameBrain.NextMoveByP1;
                    if (GameBrain.PlaceBomb(x.Value, y.Value))
                    {
                        if (player.Equals(GameBrain.NextMoveByP1)
                            || GameBrain.GetBoard(GameBrain.NextMoveByP1)[x.Value - 1, y.Value - 1].ShipId != null)
                        {
                            HitScreen = true;
                        }
                        if (GameBrain.GameOver())
                        {
                            GameOver = true;
                        }
                    }
                }
            }
            var gameState = new GameState()
            {
                Game = Game!,
                GameId = Game!.GameId,
                GameStateId = 0,
                State = GameBrain.GetSerializedGameState()
            };
            await _context.GameStates.AddAsync(gameState);
            await _context.SaveChangesAsync();



        }
    }
}