using System.Text.Json;

namespace GameBrain
{
    public class GameState
    {
        public CellState[][] Board1 { get; set; } = null!;
        public CellState[][] Board2 { get; set; } = null!;

        public bool NextMoveByP1 { get; set; }

        public GameState() : this(new CellState[0][], new CellState[0][], true) {}

        public GameState(CellState[][] board1, CellState[][] board2, bool nextMoveByP1)
        {
            Board1 = board1;
            Board2 = board2;
            NextMoveByP1 = nextMoveByP1;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}