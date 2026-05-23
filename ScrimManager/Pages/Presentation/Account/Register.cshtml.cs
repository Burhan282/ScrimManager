using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

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
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public Role Role { get; set; }

        [BindProperty]
        public Rank Rank { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            _authService.Register(
                Username,
                Email,
                Password,
                Role,
                Rank);

            return RedirectToPage("/Presentation/HomePage/Index");
        }
    }
}