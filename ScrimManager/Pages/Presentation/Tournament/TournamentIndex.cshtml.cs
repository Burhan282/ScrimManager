using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

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
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _tournamentService.JoinTournament( 
                tournamentId,
                null,
                userId,
                null,
                new List<int>()
            );

            return RedirectToPage("/Presentation/Tournament/TournamentIndex");
        }
    }
}
