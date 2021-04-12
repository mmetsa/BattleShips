using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain;

namespace WebApp.Pages_GameStates
{
    public class CreateModel : PageModel
    {
        private readonly Domain.BattleShipsDb _context;

        public CreateModel(Domain.BattleShipsDb context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["GameId"] = new SelectList(_context.Games, "GameId", "EBoatsCanTouch");
            return Page();
        }

        [BindProperty] public GameState? GameState { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GameStates.Add(GameState!);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
