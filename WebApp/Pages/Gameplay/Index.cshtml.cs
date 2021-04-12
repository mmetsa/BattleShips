using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages.Gameplay
{
    public class Index : PageModel
    {
        
        private readonly ILogger<IndexModel> _logger;
        private readonly BattleShipsDb _context;

        public string? Message { get; set; }
        
        [Required(ErrorMessage = "You need to provide the Game Name!")]
        [BindProperty, MaxLength(128)] public string? GameName { get; set; }

        [BindProperty, Range(1,100), Required]
        public int Width { get; set; }
        
        [BindProperty, Range(1,100), Required]
        public int Height { get; set; }

        [BindProperty, Required(ErrorMessage = "You need to provide Player 1 Name!")]
        public string? Player1Name { get; set; }

        [BindProperty, Required(ErrorMessage = "You need to provide Player 2 Name!")]
        public string? Player2Name { get; set; }

        [BindProperty]
        public List<string>? Player1Ships { get; set; }

        [BindProperty]
        public List<string>? Player2Ships { get; set; }

        [BindProperty, Required] public string? StartingPlayer { get; set; }

        [BindProperty, Required] public string? BoatsCanTouch { get; set; }

        [BindProperty, Required] public string? ConsecutiveMovesOnHit { get; set; }
        
        [BindProperty] public bool RandomShips { get; set; }
        
        public Index(ILogger<IndexModel> logger, BattleShipsDb context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Player1Ships?.Count == 0 || Player2Ships?.Count == 0)
            {
                return Page();
            }
            var player1Ships = new List<Ship>();
            var player2Ships = new List<Ship>();
            
            foreach (var ship in Player1Ships!)
            {
                var name = ship.Substring(0,ship.Length - (ship.Length - ship.IndexOf('('))).Replace(" ", "");
                var size = ship.Substring(ship.IndexOf('(') + 1, 1);
                if (int.TryParse(size, out var shipSize))
                {
                    if (shipSize != 0)
                    {
                        var dbShip = new Ship()
                        {
                            Name = name,
                            Size = shipSize,
                            ID = 0
                        };
                        player1Ships.Add(dbShip);   
                    }
                    else
                    {
                        return RedirectToPage("../Gameplay/Index", new {message = "Can't have a ship with No Size!"});
                    }
                }
                else
                {
                    return RedirectToPage("../Gameplay/Index", new {message = "Can't have a ship with No Size!"});
                }
            }
            foreach (var ship in Player2Ships!)
            {
                var name = ship.Substring(0,ship.Length - (ship.Length - ship.IndexOf('('))).Replace(" ", "");
                var size = ship.Substring(ship.IndexOf('(') + 1, 1);
                if (!int.TryParse(size, out var shipSize)) continue;
                var dbShip = new Ship()
                {
                    Name = name,
                    Size = shipSize,
                    ID = 0
                };
                player2Ships.Add(dbShip);
            }

            var gameBoardArea = Width * Height;
            var p1ShipsArea = 0;
            var p2ShipsArea = 0;
            player1Ships.ForEach(x => p1ShipsArea += x.Size);
            player2Ships.ForEach(x => p2ShipsArea += x.Size);
            var shouldReturn = false;
            player1Ships.ForEach(x =>
            {
                if (x.Size > Width || x.Size > Height)
                {
                    shouldReturn = true;
                }
            });
            
            player2Ships.ForEach(x =>
            {
                if (x.Size > Width || x.Size > Height)
                {
                    shouldReturn = true;
                }
            });

            if (shouldReturn)
            {
                return RedirectToPage("../Gameplay/Index", new {message = "There was a too large ship!"});
            }
            
            if (BoatsCanTouch != null && BoatsCanTouch.Equals("Yes") && p1ShipsArea > gameBoardArea
            || BoatsCanTouch != null && BoatsCanTouch.Equals("No") && p1ShipsArea > gameBoardArea * 0.6
            || BoatsCanTouch != null && BoatsCanTouch.Equals("Yes") && p2ShipsArea > gameBoardArea
            || BoatsCanTouch != null && BoatsCanTouch.Equals("No") && p2ShipsArea > gameBoardArea * 0.6)
            {
                return RedirectToPage("../Gameplay/Index", new {message = "There were too many ships for this board size!"});

            }
            
            var game = new Game()
            {
                ConsecutiveMovesOnHit = ConsecutiveMovesOnHit?.Equals("Yes") ?? true,
                CreatedAt = DateTime.Now,
                EBoatsCanTouch = BoatsCanTouch ?? "No",
                GameCode = 0,
                GameId = 0,
                GameStates = null,
                Width = Width > 0 ? Width : 10,
                Height = Height > 0 ? Height : 10,
                Name = GameName ?? DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Player1 = Player1Name ?? "Player 1",
                Player2 = Player2Name ?? "Player 2",
                Player1Ships = JsonSerializer.Serialize(player1Ships),
                Player2Ships = JsonSerializer.Serialize(player2Ships),
                Player1Starts = StartingPlayer?.Equals("Player 1") ?? true,
            };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Gameplay/Play", new {id = game.GameId, random = RandomShips});
        }

        public void OnGet(string message)
        {
            Message = message;
        }
    }

    internal class Ship
    {
        public int ID { get; set; }
        public int Size { get; set; }
        public string Name { get; set; } = null!;

    }
}