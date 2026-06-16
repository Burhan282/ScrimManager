using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerDataAcces.DataAcces.Repositories;
using ScrimManagerDataAccess;

namespace ScrimManagerPresentation.Pages.Presentation.Teams
{
    public class TeamIndexModel : PageModel
    {
        private readonly TeamService _teamService;

        public TeamIndexModel()
        {
            string connectionString = "Host=aws-0-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.hhsxuzlqfkvvkmpxxmka;Password=Persembe1907;SSL Mode=Require;Trust Server Certificate=true";
            ITeamRepository teamRepository = new TeamRepository(connectionString);
            IUserRepository userRepository = new UserRepository(connectionString);
            _teamService = new TeamService(teamRepository, userRepository);
        }
        //leeg
        public List<Team> Teams { get; set; } = new();

        public void OnGet()
        {
            Teams = _teamService.GetTeams();
        }
    }
}
