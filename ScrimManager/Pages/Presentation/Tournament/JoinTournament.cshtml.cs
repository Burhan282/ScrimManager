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

            if (Tournament.ParticipatingTeams >= Tournament.MaxTeams)
            {
                ModelState.AddModelError(string.Empty, "This tournament is full.");
                LoadUserTeams();
                return Page();
            }

            JoinRequest.TournamentId = Id;

            if (Tournament.Format == "1v1")
            {
                JoinRequest.TeamId = null;
                JoinRequest.PlayerIds = new List<int> { userId.Value };
            }
            else
            {
                int requiredPlayers = Tournament.Format == "3v3" ? 3 : 2;
                TeamModel? selectedTeam = _teamService.GetTeamById(JoinRequest.TeamId ?? 0);

                if (selectedTeam == null || selectedTeam.CreatedByUserId != userId.Value)
                    ModelState.AddModelError(string.Empty, "Select a team that you created.");

                List<int> memberIds = selectedTeam == null
                    ? new List<int>()
                    : _teamService.GetTeamMembers(selectedTeam.Id).Select(member => member.Id).ToList();

                JoinRequest.PlayerIds = JoinRequest.PlayerIds.Distinct().ToList();

                if (JoinRequest.PlayerIds.Count != requiredPlayers)
                    ModelState.AddModelError(string.Empty, $"Select exactly {requiredPlayers} players.");

                if (JoinRequest.PlayerIds.Any(playerId => !memberIds.Contains(playerId)))
                    ModelState.AddModelError(string.Empty, "Every selected player must belong to the selected team.");
            }

            if (!ModelState.IsValid)
            {
                LoadUserTeams();
                return Page();
            }

            bool joined = _tournamentService.JoinTournament(
                JoinRequest.TournamentId,
                JoinRequest.TeamId,
                userId,
                JoinRequest.EntryName,
                JoinRequest.PlayerIds
            );

            if (!joined)
            {
                ModelState.AddModelError(string.Empty, "The tournament could not be joined.");
                LoadUserTeams();
                return Page();
            }

            return RedirectToPage("/Presentation/Tournament/TournamentIndex");
        }

        private void LoadUserTeams()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || Tournament?.Format == "1v1")
                return;

            UserTeams = _teamService.GetTeamsByUserId(userId.Value)
                .Where(team => team.CreatedByUserId == userId.Value)
                .ToList();

            TeamMembers = UserTeams.ToDictionary(
                team => team.Id,
                team => _teamService.GetTeamMembers(team.Id));
        }
    }
}
