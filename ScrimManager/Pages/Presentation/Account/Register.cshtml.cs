using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using Microsoft.AspNetCore.Http;
using ScrimManagerApplication.Application.Models.DTOModels;

namespace ScrimManagerPresentation.Pages.Presentation.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AuthService _authService;

        public RegisterModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public CreateUserDTO CreateUserDTO { get; set; } = new();

        [BindProperty]
        public IFormFile? LogoFile { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
           if (LogoFile != null && LogoFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await LogoFile.CopyToAsync(memoryStream);
                CreateUserDTO.UserLogo = memoryStream.ToArray();
            }

            _authService.Register(CreateUserDTO);
                
            

            return RedirectToPage("/Presentation/HomePage/Index");
        }
    }
}