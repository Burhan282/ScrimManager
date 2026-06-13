using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using TournamentModel = ScrimManagerApplication.Application.Models.Tournament;

namespace ScrimManagerPresentation.Pages.Presentation.Tournament
{
    public class TournamentDetailsModel : PageModel
    {
        private readonly TournamentService _tournamentService;

        public TournamentDetailsModel(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        public TournamentModel? Tournament { get; set; }
        public List<TournamentParticipationDetails> Participations { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Tournament = _tournamentService.GetTournamentById(id);

            if (Tournament == null)
                return NotFound();

            Participations = _tournamentService.GetParticipationDetails(id);
            return Page();
        }
    }
}
