using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

using TournamentModel = ScrimManagerApplication.Application.Models.Tournament;
using TeamModel = ScrimManagerApplication.Application.Models.Team;

namespace ScrimManagerPresentation.Pages.Presentation.Tournament
{
    public class JoinTournamentModel : PageModel
    {
        private readonly TournamentService _tournamentService;
        private readonly TeamService _teamService;

        public JoinTournamentModel(TournamentService tournamentService, TeamService teamService)
        {
            _tournamentService = tournamentService;
            _teamService = teamService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public JoinTournamentRequestDTO JoinRequest { get; set; } = new();

        public TournamentModel? Tournament { get; set; }
        public List<TeamModel> UserTeams { get; set; } = new();
        public Dictionary<int, List<User>> TeamMembers { get; set; } = new();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToPage("/Presentation/Account/Login");

            Tournament = _tournamentService.GetTournamentById(Id);

            if (Tournament == null)
                return RedirectToPage("/Presentation/Tournament/TournamentIndex");

            LoadUserTeams();
            JoinRequest.TournamentId = Id;

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            Tournament = _tournamentService.GetTournamentById(Id);

            if (Tournament == null)
                return RedirectToPage("/Presentation/Tournament/TournamentIndex");

            JoinRequest.TournamentId = Id;

            if (!ModelState.IsValid)
            {
                LoadUserTeams();
                return Page();
            }

            string? error = _tournamentService.JoinTournament(JoinRequest, userId.Value);

            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                LoadUserTeams();
                return Page();
            }

            return RedirectToPage("/Presentation/Tournament/TournamentIndex");
        }

        private void LoadUserTeams()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || Tournament?.IsSolo != false)
                return;

            UserTeams = _teamService.GetTeamsCreatedByUser(userId.Value);

            TeamMembers = UserTeams.ToDictionary(
                team => team.Id,
                team => _teamService.GetTeamMembers(team.Id));
        }
    }
}
