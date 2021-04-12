using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain;
using GameBrain;
using GameConsoleUI;
using MenuSystem;
using GameBrain.Enums;
using GameState = Domain.GameState;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CSharp
{
    internal static class Program
    {
        private static BattleShipsDb db = new BattleShipsDb();
        private static readonly GameOptions GameOptions = new GameOptions();
        private static Battleships _game = new Battleships();
        private static void Main(string[] args)
        {
            var boardSize = new Menu("=====> Board Size <=====", MenuLevel.Level2Plus);
            boardSize.Add(new MenuItem("Normal [10 x 10]", Dimensions10By10));
            boardSize.Add(new MenuItem("Large [15 x 15]", Dimensions15By15));
            boardSize.Add(new MenuItem("Extra Large [20 x 20]", Dimensions20By20));
            boardSize.Add(new MenuItem("Custom dimensions", CustomDimensions));

            var playerNames = new Menu("=====> Player Names <=====", MenuLevel.Level2Plus);
            playerNames.Add(new MenuItem("Change Player 1 Name", () => {ChangePlayerName(1);}));
            playerNames.Add(new MenuItem("Change Player 2 Name", () => {ChangePlayerName(2);}));
            
            var whoStarts = new Menu("=====> Select Starting Player <=====", MenuLevel.Level2Plus);
            whoStarts.Add(new MenuItem("Player 1 Starts", () =>
            {
                GameOptions.Player1Starts = true;
                Console.WriteLine("Player 1 will start!");
            }));
            whoStarts.Add(new MenuItem("Player 2 Starts", () =>
            {
                GameOptions.Player1Starts = false;
                Console.WriteLine("Player 2 will start!");
            }));
            
            var battleShips = new Menu("=====> Play Game <=====", MenuLevel.Level1);
            battleShips.Add(new MenuItem("Person vs Person", () => {Battleships(_game); }));
            battleShips.Add(new MenuItem("Person vs AI", NoAction));
            battleShips.Add(new MenuItem("AI vs AI", NoAction));
            
            var options = new Menu("=====> Game Options <=====", MenuLevel.Level1);
            options.Add(new MenuItem("Select Board Size ", boardSize.RunMenu));
            options.Add(new MenuItem("Select Player Names", playerNames.RunMenu));
            options.Add(new MenuItem("Select Who Starts", whoStarts.RunMenu));
            options.Add(new MenuItem("Select Player 1 Ships", () => { SelectShips(1);}));
            options.Add(new MenuItem("Select Player 2 Ships", () => {SelectShips(2);}));
            options.Add(new MenuItem("Ship Touch Options", ShipTouchOptions));
            options.Add(new MenuItem("Consecutive Moves Options", ConfigureConsecutiveMoves));

            var menu = new Menu("=====> BATTLESHIPS <=====", MenuLevel.Level0);
            menu.Add(new MenuItem("Play Game", battleShips.RunMenu));
            menu.Add(new MenuItem("Load Game", LoadGameMenu));
            menu.Add(new MenuItem("Game Options", options.RunMenu));
            menu.Add(new MenuItem("About", NoAction));
            menu.RunMenu();
        }


        private static void SelectShips(int player)
        {
            var shipsMenu = new Menu("Player Ship Editor", MenuLevel.Level1);
            shipsMenu.Add(new MenuItem("Show all Ships", () => {PrintShips(player);}));
            shipsMenu.Add(new MenuItem("Edit Ship Name", () => {EditShipName(player);}));
            shipsMenu.Add(new MenuItem("Delete Ship", () => {DeleteShip(player);}));
            shipsMenu.Add(new MenuItem("Add Ship", () => { AddShip(player);}));
            shipsMenu.RunMenu();
        }

        private static void ShipTouchOptions()
        {
            var confMenu = new Menu("Ship Touch Options", MenuLevel.Level2Plus);
            confMenu.Add(new MenuItem("Ships can touch", () => { GameOptions.BoatsCanTouch = EBoatsCanTouch.Yes;}));
            confMenu.Add(new MenuItem("Corners can touch", () => { GameOptions.BoatsCanTouch = EBoatsCanTouch.Corners;}));
            confMenu.Add(new MenuItem("Ships can't touch", () => { GameOptions.BoatsCanTouch = EBoatsCanTouch.No;}));
            confMenu.RunMenu();
        }

        private static void ConfigureConsecutiveMoves()
        {
            var confMenu = new Menu("Consecutive Moves on Hit", MenuLevel.Level2Plus);
            confMenu.Add(new MenuItem("Yes", () => { GameOptions.ConsecutiveMovesOnHit = true;}));
            confMenu.Add(new MenuItem("No", () => { GameOptions.ConsecutiveMovesOnHit = false;}));
            confMenu.RunMenu();
        }

        private static void AddShip(int player)
        {
            Console.WriteLine("Enter New Ship Name: ");
            var name = Console.ReadLine() ?? "Ship";
            Console.WriteLine("Enter Ship Size: ");
            var size = Console.ReadLine() ?? "1";
            if (int.TryParse(size, out var boatSize))
            {
                var id = GameOptions.Player1Ships.LastOrDefault()?.ID + 1 ?? 0;
                Ship s = new Ship(id, name, boatSize);
                switch (player)
                {
                    case 1:
                        GameOptions.Player1Ships.Add(s);
                        Console.WriteLine("Ship successfully added!");
                        return;
                    case 2:
                        GameOptions.Player2Ships.Add(s);
                        Console.WriteLine("Ship successfully added!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid Ship size. Must be numeric and > 0");
            }

        }

        private static void DeleteShip(int player)
        {
            while (true)
            {
                Console.WriteLine("Enter Ship ID: (must be a number >= 0)");
                Console.WriteLine("(To delete all ships, enter 'all')");
                var shipId = Console.ReadLine() ?? "";
                if (!int.TryParse(shipId, out var id))
                {
                    if (shipId.ToLower().Equals("all"))
                    {
                        switch (player)
                        {
                            case 1:
                                GameOptions.Player1Ships.RemoveAll(x => x.ID > -1);
                                break;
                            case 2:
                                GameOptions.Player2Ships.RemoveAll(x => x.ID > -1);
                                break;
                        }

                        Console.WriteLine("Successfully deleted all ships!");
                        break;
                    }
                    Console.WriteLine("Error: No such ID: " + shipId);
                    break;
                }

                if (player == 1)
                {
                    if (GameOptions.Player1Ships.Find(x => x.ID == id) == null)
                    {
                        Console.WriteLine("Error: No Ship with ID " + id);
                        break;
                    }
                    GameOptions.Player1Ships.RemoveAll(x => x.ID == id);
                    Console.WriteLine("Successfully Deleted the ship!");
                    break;
                }
                if (player != 2) continue;
                {
                    if (GameOptions.Player2Ships.Find(x => x.ID == id) == null)
                    {
                        Console.WriteLine("Error: No Ship with ID " + id);
                        break;
                    }
                    GameOptions.Player2Ships.RemoveAll(x => x.ID == id);
                    Console.WriteLine("Successfully Deleted the ship!");
                    break;
                }
            }
        }

        private static void EditShipName(int player)
        {
            while (true)
            {
                Console.WriteLine("Enter Ship ID: (must be a number >= 0)");
                var shipId = Console.ReadLine();
                if (!int.TryParse(shipId, out var id))
                {
                    Console.WriteLine("Error: ID must be numeric: " + shipId);
                    break;
                }

                if (player == 1)
                {
                    if (GameOptions.Player1Ships.Find(x => x.ID == id) == null)
                    {
                        Console.WriteLine("Error: No Ship with ID " + id);
                        break;
                    }
                    Console.WriteLine("Old ship name: " + GameOptions.Player1Ships.Find(x => x.ID == id)!.Name);
                    Console.WriteLine("Enter new Ship name: ");
                    var name = Console.ReadLine();
                    GameOptions.Player1Ships.Find(x => x.ID == id)!.Name = name ?? "Ship";
                    break;
                }
                if (player != 2) continue;
                {
                    if (GameOptions.Player2Ships.Find(x => x.ID == id) == null)
                    {
                        Console.WriteLine("Error: No Ship with ID " + id);
                        break;
                    }
                    Console.WriteLine("Old ship name: " + GameOptions.Player1Ships.Find(x => x.ID == id)!.Name);
                    Console.WriteLine("Enter new Ship name: ");
                    var name = Console.ReadLine();
                    GameOptions.Player2Ships.Find(x => x.ID == id)!.Name = name ?? "Ship";
                    break;
                }
            }
        }

        private static void PrintShips(int player)
        {
            var shipsMenu = new Menu("Your current ships", MenuLevel.Level2Plus);
            List<Ship> ships = player == 1 ? GameOptions.Player1Ships : GameOptions.Player2Ships;
            if (ships.Count == 0)
            {
                Console.WriteLine("This player has no ships!");
                return;
            }
            foreach (var ship in ships)
            {
                shipsMenu.Add(new MenuItem(ship.ID + ") " + ship, NoAction));
            }
            shipsMenu.RunMenu();
        }
        private static void ChangePlayerName(int player)
        {
            var name = "";
            while (name == "")
            {
                Console.WriteLine("Current Player name: " + (player == 1 ? GameOptions.Player1 : GameOptions.Player2));
                Console.WriteLine("Enter the desired name: ");
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Error: The name can't be empty!");
                }
            }
            switch (player)
            {
                case 1:
                    GameOptions.Player1 = char.ToUpper(name![0]) + name.Substring(1);
                    break;
                case 2:
                    GameOptions.Player2 = char.ToUpper(name![0]) + name.Substring(1);
                    break;
            }

            Console.WriteLine("Successfully set the Player name to " + name);
        }
        private static void NoAction()
        {
            Console.WriteLine("This is NoAction");
        }

        private static void Dimensions10By10()
        {
            GameOptions.Width = 10;
            GameOptions.Height = 10;
            Console.WriteLine("Successfully set Game Board dimensions to 10 x 10");
        }
        
        private static void Dimensions15By15()
        {
            GameOptions.Width = 15;
            GameOptions.Height = 15;
            Console.WriteLine("Successfully set Game Board dimensions to 15 x 15");
        }
        
        private static void Dimensions20By20()
        {
            GameOptions.Width = 20;
            GameOptions.Height = 20;
            Console.WriteLine("Successfully set Game Board dimensions to 20 x 20");
        }
        
        private static void CustomDimensions()
        {
            int x;
            int y;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter your desired Board dimensions:");
                Console.WriteLine("Syntax: 20x20");
                var dimensions = Console.ReadLine()!.Split("x");
                try
                {
                    x = int.Parse(dimensions[0]);
                    y = int.Parse(dimensions[1]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: Invalid syntax!");
                    continue;
                }

                if (x < 5 || y < 5)
                {
                    Console.WriteLine("Error: X and Y must both be >= 5");
                    Console.WriteLine();
                    continue;
                }
                if (x > 50 || y > 50)
                {
                    Console.WriteLine("Error: X and Y must both be <= 50");
                    continue;
                }
                break;
            }
            GameOptions.Width = x;
            GameOptions.Height = y;
            Console.WriteLine("Successfully set Game Board dimensions to " + x + " x " + y);
        }

        private static void PlaceShips(Battleships game, bool nextMoveByP1)
        {
            var ships = nextMoveByP1 ? game.GameOptions.Player1Ships : game.GameOptions.Player2Ships;
            var player = nextMoveByP1 ? game.GameOptions.Player1 : game.GameOptions.Player2;
            foreach (var ship in ships)
            {
                Console.WriteLine(player + " - Enter the position for ship: " + ship);
                Console.WriteLine("Notation: x_pos,y_pos,v/h - v is vertical, h is horizontal");
                Console.WriteLine("Examples: 8,5,v or 1,1,h");
                Console.WriteLine("Write 'random' to place all ships randomly.");
                string cmd;
                var enteredCorrectly = false;
                do
                {
                    BattleshipsUI.DrawBoard(game.GetBoard(!nextMoveByP1), true);
                    cmd = Console.ReadLine() ?? " ";
                    if (cmd.ToLower().Equals("random"))
                    {
                        var placed = false;
                        for (var i = 0; i < 1000; i++)
                        {
                            if (!game.PlaceRandomShips(nextMoveByP1)) continue;
                            placed = true;
                            Console.WriteLine("Placed all ships randomly.");
                            break;
                        }

                        if (!placed)
                        {
                            Console.WriteLine("Error placing ships randomly. Please try again.");
                            continue;   
                        }

                        break;
                    }
                    var cmds = cmd.Replace(" ", "").Split(",");
                    if (cmds.Length != 3)
                    {
                        Console.WriteLine("Invalid syntax: " + cmd);
                        continue;
                    }

                    if (!int.TryParse(cmds[0], out var x) || !int.TryParse(cmds[1], out var y)) continue;
                    if (x > 0 && x <= game.GameOptions.Width && y > 0 && y <= game.GameOptions.Height)
                    {
                        if (cmds[2].ToLower().Equals("v"))
                        {
                            if (game.PlaceShip(x, y, ship, nextMoveByP1, true))
                            {
                                Console.WriteLine("Placed ship successfully!");
                                enteredCorrectly = true;
                            }
                            else
                            {
                                Console.WriteLine("Error placing ship: Doesn't fit there on the board!");

                            }
                        } else if (cmds[2].ToLower().Equals("h"))
                        {
                            if (game.PlaceShip(x, y, ship, nextMoveByP1, false))
                            {
                                Console.WriteLine("Placed ship successfully!");
                                enteredCorrectly = true;   
                            }
                            else
                            {
                                Console.WriteLine("Error placing ship: Doesn't fit there on the board!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid syntax: " + cmd);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: invalid arguments: " + cmd);
                    }

                } while (!enteredCorrectly);

                if (cmd.ToLower().Equals("random"))
                {
                    break;
                }
            }
        }

        private static void Battleships(Battleships game)
        {
            game.GameOptions = GameOptions;
            if (game.GameOptions.GameCode == 0)
            {
                PlaceShips(game, game.NextMoveByP1);
                PlaceShips(game, !game.NextMoveByP1);
            }
            while (true)
            {
                Console.Clear();
                var currentPlayer = game.NextMoveByP1 ? game.GameOptions.Player1 : game.GameOptions.Player2;
                if (game.GetBoard(game.NextMoveByP1).GetLength(0) * 3 + 1 > Console.WindowWidth || game.GetBoard(game.NextMoveByP1).GetLength(1) * 2 + 2 > Console.WindowHeight)
                {
                    Console.WriteLine("Your Game Board is larger than your Console size!");
                    Console.WriteLine("Please resize your console or select different dimensions in the Game Options menu!");
                }
                Console.WriteLine(currentPlayer + " --- This is your Board:");
                BattleshipsUI.DrawBoard(game.GetBoard(!game.NextMoveByP1), true);
                Console.WriteLine("Press any key to continue!");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine(currentPlayer + "--- your turn!");
                BattleshipsUI.DrawBoard(game.GetBoard(game.NextMoveByP1), false);
                Console.WriteLine("Enter move coordinates: (x, y): ");
                var samePlayer = game.NextMoveByP1;
                var correctCommand = false;
                do
                {
                    var command = Console.ReadLine() ?? "";
                    var positions = command.Split(",");
                    if (positions.Length != 2)
                    {
                        Console.WriteLine("Wrong syntax: " + command);
                        Console.WriteLine("Correct syntax: x,y");
                    }

                    if (int.TryParse(positions[0], out var xx) && int.TryParse(positions[1], out var yy))
                    {
                        if (!game.PlaceBomb(xx, yy))
                        {
                            Console.WriteLine("Can't place a bomb there!");
                            continue;
                        }

                        correctCommand = true;
                    }
                    else
                    {
                        Console.WriteLine("Wrong syntax: " + command);
                        Console.WriteLine("Correct syntax: x,y");
                    }

                } while (!correctCommand);
                if (game.GameOver())
                {
                    var winner = game.GameOptions.ConsecutiveMovesOnHit ? game.NextMoveByP1 ? game.GameOptions.Player2 :
                        game.GameOptions.Player1 :
                        game.NextMoveByP1 ? game.GameOptions.Player1 : game.GameOptions.Player2;
                    Console.WriteLine("The Game is over! " + winner + " is the winner!");
                    break;
                }
                samePlayer = samePlayer.Equals(game.NextMoveByP1);
                Console.Clear();
                Console.WriteLine("");
                BattleshipsUI.DrawBoard(samePlayer ? game.GetBoard(game.NextMoveByP1) : game.GetBoard(!game.NextMoveByP1), false);
                Console.WriteLine("Press Any key to continue");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("Press S to save game - Any other key to continue");
                var choice = Console.ReadKey().Key;
                if (choice != ConsoleKey.S) continue;
                RunSaveGameMenu(game);
                break;
            }
        }

        private static void SaveGame(Battleships game, int slot)
        {
            var gameToSave = new Game
            {
                GameCode = slot,
                Name = DateTime.Now.ToLongTimeString(),
                CreatedAt = DateTime.Now,
                Width = game.GameOptions.Width,
                Height = game.GameOptions.Height,
                ConsecutiveMovesOnHit = game.GameOptions.ConsecutiveMovesOnHit,
                EBoatsCanTouch = game.GameOptions.BoatsCanTouch.ToString(),
                Player1 = game.GameOptions.Player1,
                Player2 = game.GameOptions.Player2,
                Player1Ships = JsonSerializer.Serialize(game.GameOptions.Player1Ships),
                Player2Ships = JsonSerializer.Serialize(game.GameOptions.Player2Ships),
                Player1Starts = game.GameOptions.Player1Starts
            };
            if (db.Games.SingleOrDefault(x => x.GameCode.Equals(slot)) == null)
            {
                //if (db.Games.Find(slot) == null)
                //{
                db.Games.Add(gameToSave);
                foreach (var dbGameState in game.GameStates.Select(gameState => new GameState
                {
                    GameId = slot,
                    Game = gameToSave,
                    State = gameState
                }))
                {
                    db.GameStates.Add(dbGameState);
                }
                db.SaveChanges();
                Console.WriteLine("Successfully saved the game.");
            }
            else
            {
                Console.WriteLine("This slot already contains saved data. Do you want to delete the previous save? [Y/N]");
                var cmd = Console.ReadLine() ?? "";
                if (cmd.ToLower().Equals("y"))
                {

                    var dbGame = db.Games.SingleOrDefault(x => x.GameCode == slot);
                    db.Remove(dbGame);
                    db.SaveChanges();
                    Console.WriteLine("Successfully deleted the previous save.");
                }
                else
                {
                    Console.WriteLine("Game saving cancelled.");
                }
            }
        }

        private static void LoadGame(int slot)
        {
            var dbGame = db.Games.SingleOrDefault(x => x.GameId == slot);
            if (dbGame == null)
            {
                Console.WriteLine("Error loading game...");
                return;
            }
            var dbGameStates = db.GameStates.Where(x => x.GameId == dbGame.GameId).ToList();
            foreach (var dbGameState in dbGameStates)
            {
                _game.GameStates.Add(dbGameState.State);
            }
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            _game.CreateGameFromJsonString(JsonSerializer.Serialize(dbGame, jsonOptions));
            Console.WriteLine(GameOptions.Player1Starts);
            Console.WriteLine(GameOptions.BoatsCanTouch);
            Battleships(_game);
        }

        private static void LoadGameMenu()
        {
            Menu loadGameMenu = new Menu("===> Load Game <===", MenuLevel.Level1);
            var games = db.Games.ToList();
            games.Sort((p, q) => p.GameCode.CompareTo(q.GameCode));
            if (games.Count == 0)
            {
                Console.WriteLine("Error: No Saved games found.");
                return;
            }
            for (var i = 1; i <= games.Count; i++)
            {
                var i1 = i - 1;
                var i2 = i;
                loadGameMenu.Add(new MenuItem(i2 + ") " + games[i1].Name, () => {LoadGame(games[i1].GameId);}));
            }
            loadGameMenu.RunMenu();
        }

        private static void RunSaveGameMenu(Battleships game)
        {
            Menu saveGameMenu = new Menu("===> Save Game <===", MenuLevel.Level1);
            for (var i = 1; i <= 10; i++)
            {
                var i1 = i;
                saveGameMenu.Add(new MenuItem("Save to Slot " + i, () => SaveGame(game, i1)));
            }
            saveGameMenu.RunMenu();
        }
    }
}