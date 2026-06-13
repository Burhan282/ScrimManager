using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerPresentation.Pages.Presentation.Tournament
{
    public class MyTournamentsModel : PageModel
    {
        private readonly TournamentService _tournamentService;

        public MyTournamentsModel(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        public List<UserTournament> Tournaments { get; set; } = new();

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            Tournaments = _tournamentService.GetTournamentsByUserId(userId.Value);
            return Page();
        }
    }
}
