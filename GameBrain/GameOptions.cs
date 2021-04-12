using System.Collections.Generic;
using System.Text.Json.Serialization;
using GameBrain.Enums;

namespace GameBrain
{
    public class GameOptions
    {
        // Each variable has a default value so when the Player can go straight to playing the game.
        
        [JsonIgnore] public int GameCode { get; set; }
        public int Width { get; set; } = 10;
        public int Height { get; set; } = 10;
        public string Player1 { get; set; } = "Player 1"!;
        public string Player2 { get; set; } = "Player 2"!;
        public bool Player1Starts { get; set; } = true;

        public List<Ship> Player1Ships { get; set; } = new List<Ship>
        {
            // These are the default Ships in the Game.
            new Ship(0, "Small Ship", 1),
            new Ship(1, "Small Ship", 1),
            new Ship(2, "Small Ship", 1),
            new Ship(3, "Small Ship", 1),
            new Ship(4, "Small Ship", 1),
            new Ship(5, "Medium Ship", 2),
            new Ship(6, "Medium Ship", 2),
            new Ship(7, "Medium Ship", 2),
            new Ship(8, "Medium Ship", 2),
            new Ship(9, "Big Ship", 3),
            new Ship(10, "Big Ship", 3),
            new Ship(11, "Big Ship", 3),
            new Ship(12, "Large Ship", 4),
            new Ship(13, "Large Ship", 4),
            new Ship(14, "XLarge Ship", 5)
        };

        public List<Ship> Player2Ships { get; set; } = new List<Ship>
        {
            new Ship(0, "Small Ship", 1),
            new Ship(1, "Small Ship", 1),
            new Ship(2, "Small Ship", 1),
            new Ship(3, "Small Ship", 1),
            new Ship(4, "Small Ship", 1),
            new Ship(5, "Medium Ship", 2),
            new Ship(6, "Medium Ship", 2),
            new Ship(7, "Medium Ship", 2),
            new Ship(8, "Medium Ship", 2),
            new Ship(9, "Big Ship", 3),
            new Ship(10, "Big Ship", 3),
            new Ship(11, "Big Ship", 3),
            new Ship(12, "Large Ship", 4),
            new Ship(13, "Large Ship", 4),
            new Ship(14, "XLarge Ship", 5)
        };
        public EBoatsCanTouch BoatsCanTouch { get; set; } = EBoatsCanTouch.No;
        public bool ConsecutiveMovesOnHit { get; set; } = true;
    }
}