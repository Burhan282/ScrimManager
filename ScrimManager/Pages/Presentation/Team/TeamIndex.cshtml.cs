using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;

namespace ScrimManagerPresentation.Pages.Presentation.Team
{
    public class TeamIndexModel : PageModel
    {
        private readonly TeamService _teamService;

        public TeamIndexModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public List<ScrimManagerApplication.Application.Models.Team> Teams { get; set; } = new();

        public void OnGet()
        {
            Teams = _teamService.GetTeams();
        }
    }
}