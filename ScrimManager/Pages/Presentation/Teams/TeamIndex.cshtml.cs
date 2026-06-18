using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerDataAcces.DataAcces.Repositories;
using ScrimManagerDataAccess;
using Microsoft.Extensions.Configuration;

namespace ScrimManagerPresentation.Pages.Presentation.Teams
{
    public class TeamIndexModel : PageModel
    {
        private readonly TeamService _teamService;

        public TeamIndexModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;
            ITeamRepository teamRepository = new TeamRepository(connectionString); 
            IUserRepository userRepository = new UserRepository(connectionString);
            _teamService = new TeamService(teamRepository, userRepository);
        }
        //leeg
        public List<Team> Teams { get; set; } = new();

        public void OnGet()
        {
            try
            {
                Teams = _teamService.GetTeams();
            }
            catch
            {
                Teams = new List<Team>();
                TempData["ToastMessage"] = "Teams could not be loaded. Please try again later.";
                TempData["ToastType"] = "failed";
            }
        }
    }
}
 