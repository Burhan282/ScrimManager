using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using System.Collections.Generic;
using TeamModel = ScrimManagerApplication.Application.Models.Team;

namespace ScrimManagerPresentation.Pages.Presentation.Teams
{
    public class TeamDetailsModel : PageModel
    {
        private readonly TeamService _teamService;

        public TeamDetailsModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public TeamModel? Team { get; set; }

        public List<User> TeamMembers { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Team = _teamService.GetTeamById(id);

            if (Team == null)
            {
                return NotFound();
            }

            TeamMembers = _teamService.GetTeamMembers(id);

            return Page();
        }

        public IActionResult OnPostApply(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ToastMessage"] = "You must be logged in to apply.";
                TempData["ToastType"] = "failed";

                return RedirectToPage("/Presentation/Account/Login");
            }

            _teamService.ApplyToTeam(userId.Value, id);

            TempData["ToastMessage"] = "Request sent. Waiting for captain approval.";
            TempData["ToastType"] = "pending";

            return RedirectToPage("/Presentation/Teams/TeamDetails", new { id });
        }
    }
}
