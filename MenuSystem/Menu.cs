#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    
    public enum MenuLevel
    {
        Level0, 
        Level1,
        Level2Plus
    }

    internal enum MenuReturn
    {
        Exit,
        Previous,
        Main,
        None
    }

    public class Menu
    {
        private static MenuReturn _returnCommand = MenuReturn.None;
        private readonly string _title;
        private MenuItem? _currentMenuItem;
        private List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private readonly MenuLevel _menuLevel;
        private bool _writeMenu;

        public Menu(string title, MenuLevel menuLevel)
        {
            _menuLevel = menuLevel;
            _title = title;
        }

        private static void Exit()
        {
            _returnCommand = MenuReturn.Exit;
        }

        private static void ReturnToPrevious()
        {
            _returnCommand = MenuReturn.Previous;
        }

        private static void ReturnToMain()
        {
            _returnCommand = MenuReturn.Main;
        }

        public void Add(MenuItem menuItem)
        {

            if (menuItem.Label.Length > 50)
            {
                throw new Exception("The label length can not exceed 50 characters");
            }

            if (menuItem.Label.Equals(""))
            {
                throw new Exception("The Label can not be empty!");
            }

            if (MenuItems.Any(item => menuItem.Label.ToLower().Equals(item.Label.ToLower())))
            {
                throw new Exception("Same Labels are not allowed in the same menu!");
            }
            
            if (MenuItems.Count.Equals(0))
            {
                _currentMenuItem = menuItem;
            }
            
            MenuItems.Add(menuItem);
        }

        private void BuildMenu()
        {
            //MenuItems is already loaded with the correct items through the Add() function
            //So now I just add the System default menu items to MenuItems
            // I do this so navigating the menu with arrow keys is easier.
            
            _writeMenu = true;
            
            if (!MenuItems.Any(item => item.Label.Equals("Back")) && _menuLevel == MenuLevel.Level2Plus)
            {
                MenuItems.Add(new MenuItem("Back", ReturnToPrevious));
            }
            
            if (!MenuItems.Any(item => item.Label.Equals("Return to Main")) && (_menuLevel == MenuLevel.Level1 || _menuLevel == MenuLevel.Level2Plus))
            {
                MenuItems.Add(new MenuItem("Return to Main", ReturnToMain));
            }
            
            if (!MenuItems.Any(item => item.Label.Equals("Exit")))
            {
                MenuItems.Add(new MenuItem("Exit", Exit));
            }

        }

        private void RewriteMenuItem(int previousOrNext)
        {
            
            var lastCursorPosition = Console.CursorTop;
            var currentMenuItem = MenuItems.FindIndex(a => a.Equals(_currentMenuItem));
            
            var longestString = MenuItems.Max(s => s.Label.Length);
            longestString = _title.Length > longestString ? _title.Length + 1: longestString + 1;
            var spacesAfter = (longestString - _currentMenuItem!.Label.Length) / 2;
            var spacesBefore = (longestString - _currentMenuItem.Label.Length) % 2 == 0 ? spacesAfter : spacesAfter + 1;
            
            Console.SetCursorPosition(0, Console.CursorTop - MenuItems.Count - 1 + currentMenuItem);
            Console.Write("| " + new string(' ', spacesBefore));
            Console.ForegroundColor = _currentMenuItem.IsDisabled ? ConsoleColor.Red : ConsoleColor.Gray;
            Console.Write(_currentMenuItem);
            Console.ResetColor();
            Console.WriteLine(new string(' ', spacesAfter) + " |");
            Console.SetCursorPosition(0, lastCursorPosition);
            _currentMenuItem = MenuItems[currentMenuItem + previousOrNext];

            spacesAfter = (longestString - _currentMenuItem!.Label.Length) / 2;
            spacesBefore = (longestString - _currentMenuItem.Label.Length) % 2 == 0 ? spacesAfter : spacesAfter + 1;
            
            Console.SetCursorPosition(0, Console.CursorTop - MenuItems.Count - 1 + currentMenuItem + previousOrNext);
            Console.Write("| " + new string(' ', spacesBefore));
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = _currentMenuItem.IsDisabled ? ConsoleColor.Red : ConsoleColor.Gray;
            Console.Write(_currentMenuItem);
            Console.ResetColor();
            Console.WriteLine(new string(' ', spacesAfter) + " |");
            Console.SetCursorPosition(0, lastCursorPosition);
            _writeMenu = false;
        }

        private void WriteMenu()
        {

            var longestString = MenuItems.Max(s => s.Label.Length);
            longestString = _title.Length > longestString ? _title.Length + 1: longestString + 1;

            var spacesAfter = (longestString - _title.Length) / 2;
            var spacesBefore = (longestString - _title.Length) % 2 == 0 ? spacesAfter : spacesAfter + 1;
            
            //Console.Clear();
            //Can't clear console because if the MenuSystem user wants to print out anything, this just clears it.
            //TODO - Find a way to clear the console after displaying what the user wants to display!
            
            Console.WriteLine(new string('_', longestString + 4));
            Console.WriteLine("| " + new string(' ', spacesBefore) + _title + new string(' ', spacesAfter) + " |");
            Console.WriteLine(new string('-', longestString + 4));

            foreach (var menuItem in MenuItems)
            {
                spacesAfter = (longestString - menuItem.Label.Length) / 2;
                spacesBefore = (longestString - menuItem.Label.Length) % 2 == 0 ? spacesAfter : spacesAfter + 1;
                if (menuItem.Equals(_currentMenuItem))
                {
                    Console.Write("| " + new string(' ', spacesBefore));
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = menuItem.IsDisabled ? ConsoleColor.Red : ConsoleColor.Gray;
                    Console.Write(menuItem);
                    Console.ResetColor();
                    Console.WriteLine(new string(' ', spacesAfter) + " |");
                }
                else
                {
                    Console.Write("| " + new string(' ', spacesBefore));
                    Console.ForegroundColor = menuItem.IsDisabled ? ConsoleColor.Red : ConsoleColor.Gray;
                    Console.Write(menuItem);
                    Console.ResetColor();
                    Console.WriteLine(new string(' ', spacesAfter) + " |");
                }
            }
            Console.WriteLine(new string('-', longestString + 4));
        }

        public void RunMenu()
        {
            if (MenuItems.Count == 0)
            {
                throw new Exception("The Menu has to have at least 1 MenuItem");
            }
            
            BuildMenu();

            while (true)
            {
                if (_writeMenu)
                {
                    WriteMenu();
                }

                var key = Console.ReadKey().Key;
                var currentMenuItem = MenuItems.FindIndex(a => a.Equals(_currentMenuItem));
                if ((key == ConsoleKey.DownArrow || key == ConsoleKey.S) && currentMenuItem < MenuItems.Count - 1)
                {
                    RewriteMenuItem(1);
                } 
                else if ((key == ConsoleKey.UpArrow || key == ConsoleKey.W) && currentMenuItem > 0)
                {
                    RewriteMenuItem(-1);
                } else if (key == ConsoleKey.Enter || key == ConsoleKey.Tab)
                {
                    if (_currentMenuItem!.IsDisabled)
                    {
                        _writeMenu = false;
                    } 
                    else
                    {
                        _currentMenuItem.MethodToExecute();
                        _writeMenu = true;
                        
                        if (_returnCommand == MenuReturn.Exit)
                        {
                            if (_menuLevel == MenuLevel.Level0)
                            {
                                Console.WriteLine("Closing down...");
                            }
                            break;
                        }

                        if (_returnCommand == MenuReturn.Main)
                        {
                            if (_menuLevel != MenuLevel.Level0)
                            {
                                break;
                            }

                            _returnCommand = MenuReturn.None;
                        }

                        if (_returnCommand != MenuReturn.Previous) continue;
                        _returnCommand = MenuReturn.None;
                        break;
                    }
                }
                else
                {
                    _writeMenu = false;
                }
            }
        }
    }
}