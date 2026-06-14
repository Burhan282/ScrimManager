using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerPresentation.Pages.Presentation.Teams
{
    public class TeamRequestModel : PageModel
    {
        private readonly TeamService _teamService;

        public TeamRequestModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public List<TeamJoinRequest> Requests { get; set; } = new();

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Presentation/Account/Login");
            }

            Requests = _teamService.GetPendingRequestsForCaptain(userId.Value);

            return Page();
        }

        public IActionResult OnPostAccept(int requestId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _teamService.AcceptJoinRequest(requestId, userId.Value);

            return RedirectToPage();
        }

        public IActionResult OnPostDecline(int requestId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Presentation/Account/Login");

            _teamService.DeclineJoinRequest(requestId, userId.Value);

            return RedirectToPage();
        }
    }
}
