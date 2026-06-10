using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerPresentation.Pages.Presentation.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            
            User? user = _authService.Login(Email, Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
                return Page();
            }

            
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToPage("/Presentation/HomePage/Index");
        }
    }
}