using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrimManager.Application;

namespace ScrimManager.Pages;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost(string userName, string email, string password)
    {
        try
        {
            _authService.Register(userName, email, password);
            // Later kun je hier een success boodschap meegeven
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
