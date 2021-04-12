using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BattleShipsDb _context;

        public IndexModel(ILogger<IndexModel> logger, BattleShipsDb context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Game> Game { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Game = await _context.Games.OrderBy(x => x.CreatedAt).ToListAsync();
        }
        
    }
}