using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ScrimManager.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect("/Tournaments");
        }
    }
}