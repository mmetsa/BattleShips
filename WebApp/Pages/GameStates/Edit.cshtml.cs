using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages_GameStates
{
    public class EditModel : PageModel
    {
        private readonly Domain.BattleShipsDb _context;

        public EditModel(Domain.BattleShipsDb context)
        {
            _context = context;
        }

        [BindProperty]
        public GameState? GameState { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameState = await _context.GameStates
                .Include(g => g.Game).FirstOrDefaultAsync(m => m.GameStateId == id);

            if (GameState == null)
            {
                return NotFound();
            }
           ViewData["GameId"] = new SelectList(_context.Games, "GameId", "EBoatsCanTouch");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(GameState).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameStateExists(GameState!.GameStateId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool GameStateExists(int id)
        {
            return _context.GameStates.Any(e => e.GameStateId == id);
        }
    }
}
