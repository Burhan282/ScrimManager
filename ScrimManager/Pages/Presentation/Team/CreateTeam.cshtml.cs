using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models.DTOModels;
using System.IO;
using System.Threading.Tasks;

namespace ScrimManagerPresentation.Pages.Presentation.Team
{
    public class CreateTeamModel : PageModel
    {
        private readonly TeamService _teamService;

        [BindProperty]
        public CreateTeamDTO CreateTeamDTO { get; set; } = new();

        [BindProperty]
        public IFormFile? LogoFile { get; set; }

        public CreateTeamModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || userId <= 0)
            {
                return RedirectToPage("/Presentation/Account/Login");
            }

            if (LogoFile != null && LogoFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await LogoFile.CopyToAsync(memoryStream);
                CreateTeamDTO.LogoData = memoryStream.ToArray();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _teamService.CreateTeam(CreateTeamDTO, userId.Value);

            return RedirectToPage("/Presentation/Team/TeamIndex");
        }
    }
}