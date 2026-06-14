using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerPresentation.Pages.Presentation.Teams
{
    public class TeamIndexModel : PageModel
    {
        private readonly TeamService _teamService;

        public TeamIndexModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public List<Team> Teams { get; set; } = new();

        public void OnGet()
        {
            Teams = _teamService.GetTeams();
        }
    }
}
