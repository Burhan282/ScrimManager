using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using System;

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
                TempData["ToastMessage"] = "Email or password is incorrect.";
                TempData["ToastType"] = "failed";
                return Page();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            if (user.UserLogo != null && user.UserLogo.Length > 0)
            {
                HttpContext.Session.SetString("UserLogoBase64", Convert.ToBase64String(user.UserLogo));
            }
            else
            {
                HttpContext.Session.Remove("UserLogoBase64");
            }

            TempData["ToastMessage"] = "Login successful.";
            TempData["ToastType"] = "success";

            return RedirectToPage("/Presentation/HomePage/Index");
        }
    }
}