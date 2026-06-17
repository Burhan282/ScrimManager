using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using TeamModel = ScrimManagerApplication.Application.Models.Team;
using System.Collections.Generic;

namespace ScrimManagerPresentation.Pages.Presentation.Teams
{
    public class MyTeamModel : PageModel
    {
        private readonly TeamService _teamService;

        public MyTeamModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public List<TeamModel> UserTeams { get; set; } = new();

        public IActionResult OnGet()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                {
                    return RedirectToPage("/Presentation/Account/Login");
                }

                UserTeams = _teamService.GetTeamsByUserId(userId.Value);

                return Page();
            }
            catch
            {
                UserTeams = new List<TeamModel>();

                TempData["ToastMessage"] = "Your teams could not be loaded.";
                TempData["ToastType"] = "failed";

                return Page();
            }
        }
    }
}
