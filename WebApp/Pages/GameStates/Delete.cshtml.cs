using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages_GameStates
{
    public class DeleteModel : PageModel
    {
        private readonly Domain.BattleShipsDb _context;

        public DeleteModel(Domain.BattleShipsDb context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameState = await _context.GameStates.FindAsync(id);

            if (GameState != null)
            {
                _context.GameStates.Remove(GameState);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
