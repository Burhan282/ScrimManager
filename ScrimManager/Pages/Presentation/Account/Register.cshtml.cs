using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using Microsoft.AspNetCore.Http;

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
        public List<string> SelectedRoles { get; set; } = new();

        [BindProperty]
        public Rank Rank { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            string roles = string.Join(",", SelectedRoles);

            _authService.Register(
                Username,
                Email,
                Password,
                roles,
                Rank);

            HttpContext.Session.SetString("Username", Username);

            return RedirectToPage("/Presentation/HomePage/Index");
        }
    }
}