using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManager.Data;
using ScrimManager.Models;

namespace ScrimManager.Pages.Tournaments
{
    public class CreateModel : PageModel
    {
        private readonly ScrimManagerDbContext _context;

        public CreateModel(ScrimManagerDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tournament Tournament { get; set; } = new Tournament();

        [BindProperty]
        public DateTime? SelectedDate { get; set; }

        [BindProperty]
        public TimeSpan? SelectedTime { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Tournament.Naam))
            {
                ModelState.AddModelError("Tournament.Naam", "The name field is required.");
            }

            if (string.IsNullOrWhiteSpace(Tournament.Organisator))
            {
                ModelState.AddModelError("Tournament.Organisator", "The organizer field is required.");
            }

            if (string.IsNullOrWhiteSpace(Tournament.Format))
            {
                ModelState.AddModelError("Tournament.Format", "Please select a format.");
            }

            if (Tournament.MaxTeams <= 0)
            {
                ModelState.AddModelError("Tournament.MaxTeams", "Please select the maximum number of teams.");
            }

            if (Tournament.ParticipatingTeams < 0)
            {
                ModelState.AddModelError("Tournament.ParticipatingTeams", "Participating teams cannot be negative.");
            }

            if (Tournament.ParticipatingTeams > Tournament.MaxTeams && Tournament.MaxTeams > 0)
            {
                ModelState.AddModelError("Tournament.ParticipatingTeams", "Participating teams cannot be higher than max teams.");
            }

            if (Tournament.PrizeMoney < 0)
            {
                ModelState.AddModelError("Tournament.PrizeMoney", "Prize money cannot be negative.");
            }

            if (!SelectedDate.HasValue)
            {
                ModelState.AddModelError("SelectedDate", "Please select a date.");
            }

            if (!SelectedTime.HasValue)
            {
                ModelState.AddModelError("SelectedTime", "Please select a time.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            DateTime selectedDate = SelectedDate!.Value;
            TimeSpan selectedTime = SelectedTime!.Value;

            try
            {
                var combinedDateTime = selectedDate.Date + selectedTime;
                Tournament.Datum = DateTime.SpecifyKind(combinedDateTime, DateTimeKind.Utc);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The selected date or time is not valid.");
                return Page();
            }

            Tournament.Status = "Open";

            _context.Tournaments.Add(Tournament);
            _context.SaveChanges();

            return Redirect("/tournaments");
        }
    }
}