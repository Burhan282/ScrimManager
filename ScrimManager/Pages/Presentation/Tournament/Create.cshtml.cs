using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using System;
using TournamentModel = ScrimManagerApplication.Application.Models.Tournament;

namespace ScrimManagerPresentation.Pages.Presentation.Tournament
{
    public class CreateModel : PageModel
    {
        private readonly TournamentService _tournamentService;

        public CreateModel(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [BindProperty]
        public TournamentModel Tournament { get; set; } = new TournamentModel();

        [BindProperty]
        public DateTime? SelectedDate { get; set; }

        [BindProperty]
        public TimeSpan? SelectedTime { get; set; }

        public IActionResult OnPost()
        {
            if (!SelectedDate.HasValue || !SelectedTime.HasValue)
            {
                TempData["ToastMessage"] = "Select a valid date and time.";
                TempData["ToastType"] = "failed";

                return Page();
            }

            _tournamentService.CreateTournament(
                Tournament,
                SelectedDate.Value,
                SelectedTime.Value
            );

            TempData["ToastMessage"] = "Tournament created successfully.";
            TempData["ToastType"] = "success";

            return RedirectToPage("/Presentation/Tournament/TournamentIndex");
        }
    }
}