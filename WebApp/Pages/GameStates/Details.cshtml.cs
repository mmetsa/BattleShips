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
    public class DetailsModel : PageModel
    {
        private readonly Domain.BattleShipsDb _context;

        public DetailsModel(Domain.BattleShipsDb context)
        {
            _context = context;
        }

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
    }
}
