using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

namespace ScrimManager.Pages.Tournaments
{
    public class CreateModel : PageModel
    {
        private readonly TournamentService _tournamentService;

        [BindProperty]
        public Tournament Tournament { get; set; } = new Tournament();

        [BindProperty]
        public DateTime? SelectedDate { get; set; }

        [BindProperty]
        public TimeSpan? SelectedTime { get; set; }

        public CreateModel(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        public IActionResult OnPost()
        {
            if (!SelectedDate.HasValue || !SelectedTime.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Select a valid date and time.");
                return Page();
            }

            _tournamentService.CreateTournament(
                Tournament,
                SelectedDate.Value,
                SelectedTime.Value
            );

            return RedirectToPage("/Presentation/Tournament/TournamentIndex");
        }
    }
}