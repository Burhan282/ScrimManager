using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManager.Application;
using ScrimManager.Domain;

namespace ScrimManager.Pages.Tournaments
{
    public class TournamentIndexModel : PageModel
    {
        private readonly TournamentService _tournamentService;

        public TournamentIndexModel(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        public List<Tournament> Tournaments { get; set; } = new();

        public void OnGet()
        {
            Tournaments = _tournamentService.GetTournaments();
        }

        public IActionResult OnPostJoin(int tournamentId)
        {
            _tournamentService.JoinTournament(tournamentId);

            return RedirectToPage();
        }
    }
}