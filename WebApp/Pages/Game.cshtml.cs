using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class Test : PageModel
    {
        // BindProperty - try to get the value from incoming HTML data in case of POST
        [BindProperty]
        public string? Name { get; set; }

        public int[,] GameBoard { get; set; } = new int[10, 10];
        
        public void OnGet()
        {
            for (var y = 0; y < GameBoard.GetLength(0); y++)
            {
                for (var x = 0; x < GameBoard.GetLength(1); x++)
                {
                    GameBoard[x, y] = new Random().Next(0, 100);
                }
            }
        }

        public void OnPost()
        {
        }
    }
}