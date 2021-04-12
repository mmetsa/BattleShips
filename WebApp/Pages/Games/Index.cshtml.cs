using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages_Games
{
    public class IndexModel : PageModel
    {
        private readonly Domain.BattleShipsDb _context;

        public IndexModel(Domain.BattleShipsDb context)
        {
            _context = context;
        }

        public IList<Game> Game { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Game = await _context.Games.ToListAsync();
        }
    }
}
