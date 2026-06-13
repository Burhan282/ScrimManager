using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models.DTOModels;

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
        public CreateTournamentDTO CreateTournamentDTO { get; set; } = new();

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Complete all required tournament fields.";
                TempData["ToastType"] = "failed";

                return Page();
            }

            _tournamentService.CreateTournament(CreateTournamentDTO);

            TempData["ToastMessage"] = "Tournament created successfully.";
            TempData["ToastType"] = "success";

            return RedirectToPage("/Presentation/Tournament/TournamentIndex");
        }
    }
}
