using System;
using System.Collections.Generic;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        public int GameCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Name { get; set; } = null!;
        
        public int Width { get; set; }
        public int Height { get; set; }
        public string Player1 { get; set; } = null!;
        public string Player2 { get; set; } = null!;
        public bool Player1Starts { get; set; }
        
        // Json Serialized list of Ships
        public string Player1Ships { get; set; } = null!;
        public string Player2Ships { get; set; } = null!;
        
        public bool ConsecutiveMovesOnHit { get; set; }
        public string EBoatsCanTouch { get; set; } = null!;
        public ICollection<GameState>? GameStates { get; set; }

        public override string ToString()
        {
            return "Game Id: " + GameId + " -- Created at: " + CreatedAt.ToLongDateString() + " -- Gamestates: " +
                   GameStates;
        }
    }
}