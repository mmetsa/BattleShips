using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GameBrain.Enums;

namespace GameBrain
{
    public class Battleships
    {
        private CellState[,] _board1;
        private CellState[,] _board2;
        public bool NextMoveByP1 { get; set; }

        public GameOptions GameOptions { get; set; }

        public List<string> GameStates { get; set; } = new List<string>();

        public Battleships(GameOptions gameOptions)
        {
            GameOptions = gameOptions;
            _board1 = new CellState[gameOptions.Width, gameOptions.Height];
            _board2 = new CellState[gameOptions.Width, gameOptions.Height];
            NextMoveByP1 = GameOptions.Player1Starts;
        }

        public Battleships() : this(new GameOptions()) {}

        public CellState[,] GetBoard(bool nextMoveByP1)
        {
            var res = new CellState[GameOptions.Width, GameOptions.Height];
            if (!nextMoveByP1)
            {
                Array.Copy(_board1, res, _board1.Length);
            }
            else
            {
                Array.Copy(_board2, res, _board2.Length);

            }

            return res;
        }

        public bool PlaceRandomShips(bool player1Board)
        {
            var ships = player1Board ? GameOptions.Player1Ships : GameOptions.Player2Ships;
            ships.Sort((x,y) => x.Size.CompareTo(y.Size));
            ships.Reverse();
            foreach (var ship in ships)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var vertical = new Random().Next(1, 3) == 1;
                    var x = vertical
                        ? new Random().Next(1, GameOptions.Width + 1)
                        : new Random().Next(1, GameOptions.Width - ship.Size + 2);
                    var y = vertical
                        ? new Random().Next(1, GameOptions.Height - ship.Size + 2)
                        : new Random().Next(1, GameOptions.Height + 1);
                    var placed = PlaceShip(x, y, ship, player1Board, vertical);
                    if (placed)
                    {
                        break;
                    }

                    if (placed || i != 999) continue;
                    var board = player1Board ? _board1 : _board2;
                    for (var j = 0; j < board.GetLength(0); j++)
                    {
                        for (var k = 0; k < board.GetLength(1); k++)
                        {
                            board[j, k] = new CellState();
                        }
                    }
                    return false;
                }
            }

            return true;
        }

        public bool PlaceShip(int x, int y, Ship ship, bool player1Board, bool vertical)
        {
            var board = player1Board ? _board1 : _board2;
            if (vertical)
            {
                for (var i = 0; i < ship.Size; i++)
                {
                    if (y - 1 + ship.Size > GameOptions.Height)
                    {
                        return false;
                    }
                    if (board[x - 1, y - 1 + i].ShipId != null)
                    {
                        return false;
                    }

                    if (GameOptions.BoatsCanTouch.Equals(EBoatsCanTouch.No) || GameOptions.BoatsCanTouch.Equals(EBoatsCanTouch.Corners))
                    {
                        if (x == 1 && board[x - 1, y - 1 + i].ShipId != null
                            || x == 1 && board[x, y - 1 + i].ShipId != null
                            || x == GameOptions.Width && board[x - 2, y - 1 + i].ShipId != null
                            || x != 1 && x < GameOptions.Width && (board[x - 2, y - 1 + i].ShipId != null
                                                                    || board[x, y - 1 + i].ShipId != null)
                            || y + i != GameOptions.Height && board[x - 1, y + i].ShipId != null
                            || y != 1 && board[x - 1, y - 1 + i - 1].ShipId != null)
                        {
                            return false;
                        }
                    }

                    if (!GameOptions.BoatsCanTouch.Equals(EBoatsCanTouch.No)) continue;
                    if (y - 2 >= 0 && x - 2 >= 0 && board[x - 2, y - 2].ShipId != null
                        || y - 2 >= 0 && x != GameOptions.Width && board[x, y - 2].ShipId != null
                        || y + i != GameOptions.Height && x - 2 >= 0 && board[x - 2, y + i].ShipId != null
                        || y + i != GameOptions.Height && x != GameOptions.Width && board[x, y + i].ShipId != null)
                    {
                        return false;
                    }
                }
                for (var i = 0; i < ship.Size; i++)
                {
                    board[x - 1, y - 1 + i].ShipId ??= ship.ID;
                }
            }
            else
            {
                for (var i = 0; i < ship.Size; i++)
                {
                    if (x - 1 + ship.Size > GameOptions.Width)
                    {
                        return false;
                    }
                    if (board[x - 1 + i, y - 1].ShipId != null)
                    {
                        return false;
                    }

                    if (GameOptions.BoatsCanTouch.Equals(EBoatsCanTouch.No) || GameOptions.BoatsCanTouch.Equals(EBoatsCanTouch.Corners))
                    {
                        if (y == 1 && board[x - 1 + i, y].ShipId != null
                        || y == GameOptions.Height && board[x - 1 + i, y - 2].ShipId != null
                        || y != 1 && y < GameOptions.Height && (board[x - 1 + i, y - 2].ShipId != null || board[x - 1 + i, y].ShipId != null)
                        || x + i != GameOptions.Width && board[x + i, y - 1].ShipId != null
                        || x != 1 && board[x + i - 2, y - 1].ShipId != null)
                        {
                            return false;
                        }
                    }

                    if (!GameOptions.BoatsCanTouch.Equals(EBoatsCanTouch.No)) continue;
                    if (y - 2 >= 0 && x + i - 2 >= 0 && board[x + i - 2, y - 2].ShipId != null
                        || y - 2 >= 0 && x + i != GameOptions.Width && board[x + i, y - 2].ShipId != null
                        || x + i - 2 >= 0 && y != GameOptions.Width && board[x + i - 2, y].ShipId != null
                        || x + i != GameOptions.Width && y != GameOptions.Width && board[x + i, y].ShipId != null)
                    {
                        return false;
                    }
                }
                for (var i = 0; i < ship.Size; i++)
                {
                    board[x - 1 + i, y - 1].ShipId ??= ship.ID;
                }
            }
            return true;
        }

        public bool GameOver()
        {
            var board = GameOptions.ConsecutiveMovesOnHit ? NextMoveByP1 ? _board2 :
                _board1 :
                NextMoveByP1 ? _board1 : _board2;
            for (var i = 0; i < board.GetLength(0); i++)
            {
                for (var j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i,j].ShipId != null && board[i,j].Bomb == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool PlaceBomb(int x, int y)
        {
            if (x - 1 < 0 || x - 1 >= GameOptions.Width || y - 1 < 0 || y - 1 >= GameOptions.Height)
            {
                return false;
            }
            if (NextMoveByP1)
            {
                if (_board2[x - 1, y - 1].Bomb) return false;
                _board2[x - 1, y - 1].Bomb = true;
                if (_board2[x - 1, y - 1].ShipId != null && GameOptions.ConsecutiveMovesOnHit)
                {
                    NextMoveByP1 = NextMoveByP1;
                }
                else
                {
                    NextMoveByP1 = !NextMoveByP1;
                }
                GameStates.Add(GetSerializedGameState());
                return true;
            }

            if (_board1[x - 1, y - 1].Bomb) return false;
            _board1[x - 1, y - 1].Bomb = true;
            if (_board1[x - 1, y - 1].ShipId != null && GameOptions.ConsecutiveMovesOnHit)
            {
                NextMoveByP1 = NextMoveByP1;
            }
            else
            {
                NextMoveByP1 = !NextMoveByP1;
            }
            GameStates.Add(GetSerializedGameState());
            return true;

        }

        public void CreateGameFromJsonString(string jsonString)
        {
            var gameOptions = JsonSerializer.Deserialize<DbGame>(jsonString);
            GameOptions.GameCode = gameOptions.GameCode;
            GameOptions.Width = gameOptions.Width;
            GameOptions.Height = gameOptions.Height;
            GameOptions.Player1 = gameOptions.Player1;
            GameOptions.Player2 = gameOptions.Player2;
            GameOptions.Player1Ships = JsonSerializer.Deserialize<List<Ship>>(gameOptions.Player1Ships);
            GameOptions.Player2Ships = JsonSerializer.Deserialize<List<Ship>>(gameOptions.Player2Ships);
            GameOptions.Player1Starts = gameOptions.Player1Starts;
            _board1 = new CellState[GameOptions.Width, GameOptions.Height];
            _board2 = new CellState[GameOptions.Width, GameOptions.Height];
            GameOptions.BoatsCanTouch = gameOptions.EBoatsCanTouch switch
            {
                "Yes" => EBoatsCanTouch.Yes,
                "No" => EBoatsCanTouch.No,
                _ => EBoatsCanTouch.Corners
            };
            GameOptions.ConsecutiveMovesOnHit = gameOptions.ConsecutiveMovesOnHit;
            
            var lastGameState = GameStates.Count != 0 ? JsonSerializer.Deserialize<GameState>(GameStates.Last()) : null;
            if (lastGameState == null)
            {
                _board1 = new CellState[GameOptions.Width, GameOptions.Height];
                _board2 = new CellState[GameOptions.Width, GameOptions.Height];
                NextMoveByP1 = gameOptions.Player1Starts;
            }
            else
            {
                for (var i = 0; i < lastGameState.Board1.Length; i++)
                {
                    for (var j = 0; j < lastGameState.Board1[i].Length; j++)
                    {
                        _board1[i, j] = lastGameState.Board1[i][j];
                        _board2[i, j] = lastGameState.Board2[i][j];
                    }
                }
                NextMoveByP1 = lastGameState.NextMoveByP1;
            }
        }

        public string GetSerializedGameState()
        {
            var gameState = new GameState();
            var board1 = new CellState[_board1.GetLength(0)][];
            var board2 = new CellState[_board2.GetLength(0)][];
            for (var i = 0; i < board1.Length; i++)
            {
                board1[i] = new CellState[_board1.GetLength(1)];
                board2[i] = new CellState[_board2.GetLength(1)];
            }

            for (int i = 0; i < _board1.GetLength(0); i++)
            {
                for (int j = 0; j < _board1.GetLength(1); j++)
                {
                    board1[i][j] = _board1[i, j];
                    board2[i][j] = _board2[i, j];
                }
            }

            gameState.Board1 = board1;
            gameState.Board2 = board2;
            gameState.NextMoveByP1 = NextMoveByP1;
            
            return JsonSerializer.Serialize(gameState, new JsonSerializerOptions{WriteIndented = true});
        }

        private class DbGame
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
        }
    }
}