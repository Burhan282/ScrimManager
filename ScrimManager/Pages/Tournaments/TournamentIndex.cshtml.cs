using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManager.Data;
using ScrimManager.Models;
using System.Linq;

namespace ScrimManager.Pages.Tournaments
{
    public class TournamentIndexModel : PageModel
    {
        private readonly ScrimManagerDbContext _context;

        public TournamentIndexModel(ScrimManagerDbContext context)
        {
            _context = context;
        }

        public List<Tournament> Tournaments { get; set; } = new();

        public void OnGet()
        {
            Tournaments = _context.Tournaments
                .OrderBy(t => t.Datum)
                .ToList();
        }

        public IActionResult OnPostJoin(int tournamentId)
        {
            var tournament = _context.Tournaments.FirstOrDefault(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return RedirectToPage();
            }

            if (tournament.ParticipatingTeams < tournament.MaxTeams)
            {
                tournament.ParticipatingTeams += 1;
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}