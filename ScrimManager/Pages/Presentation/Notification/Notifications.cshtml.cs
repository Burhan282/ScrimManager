using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerPresentation.Pages.Presentation.Notification
{
    public class NotificationsModel : PageModel
    {
        private readonly TeamService _teamService;
        private readonly TournamentService _tournamentService;

        public NotificationsModel(
            TeamService teamService,
            TournamentService tournamentService)
        {
            _teamService = teamService;
            _tournamentService = tournamentService;
        }

        public List<TeamJoinRequest> TeamRequests { get; set; } = new();
        public List<TournamentInvitation> TournamentInvitations { get; set; } = new();

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            LoadNotifications(userId.Value);
            return Page();
        }

        public IActionResult OnPostAcceptTeam(int requestId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _teamService.AcceptJoinRequest(requestId, userId.Value);
            return RedirectToPage();
        }

        public IActionResult OnPostDeclineTeam(int requestId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _teamService.DeclineJoinRequest(requestId, userId.Value);
            return RedirectToPage();
        }

        public IActionResult OnPostAcceptTournament(int invitationId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _tournamentService.AcceptInvitation(invitationId, userId.Value);
            return RedirectToPage();
        }

        public IActionResult OnPostDeclineTournament(int invitationId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _tournamentService.DeclineInvitation(invitationId, userId.Value);
            return RedirectToPage();
        }

        private void LoadNotifications(int userId)
        {
            TeamRequests = _teamService.GetPendingRequestsForCaptain(userId);
            TournamentInvitations = _tournamentService.GetPendingInvitations(userId);
        }
    }
}
