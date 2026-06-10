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
            // Zorg dat de userId correct wordt opgehaald
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || userId.Value <= 0)
            {
                return RedirectToPage("/Presentation/Account/Login");
            }

            // Logo bestand verwerken
            if (LogoFile != null && LogoFile.Length > 0)
            {
                using MemoryStream memoryStream = new MemoryStream();
                await LogoFile.CopyToAsync(memoryStream);
                CreateTeamDTO.LogoData = memoryStream.ToArray();
            }

            // Model check
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Team aanmaken met userId
            _teamService.CreateTeam(CreateTeamDTO, userId.Value);

            // Redirect naar TeamIndex
            return RedirectToPage("/Presentation/Team/TeamIndex");
        }
    }
}