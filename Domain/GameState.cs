using System.Text.Json.Serialization;

namespace Domain
{
    public class GameState
    {
        // This Database Table holds the info for all the States of all the Games.
        public int GameStateId { get; set; }
        public int GameId { get; set; }
        [JsonIgnore]
        public Game Game { get; set; } = null!;
        
        // This is the Serialized game state.
        public string State { get; set; } = null!;

    }
}