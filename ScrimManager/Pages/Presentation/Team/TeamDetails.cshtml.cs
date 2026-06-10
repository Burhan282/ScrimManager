using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;

namespace ScrimManagerPresentation.Pages.Presentation.Team
{
    public class TeamDetailsModel : PageModel
    {
        private readonly TeamService _teamService;

        public ScrimManagerApplication.Application.Models.Team? Team { get; set; }

        public TeamDetailsModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public IActionResult OnGet(int id)
        {
            Team = _teamService.GetTeamById(id);

            if (Team == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}