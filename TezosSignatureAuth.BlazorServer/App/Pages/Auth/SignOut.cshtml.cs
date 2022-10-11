using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TezosSignatureAuth.BlazorServer.App.Pages.Auth
{
    [IgnoreAntiforgeryToken]
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet() => NotFound();

        public async Task<IActionResult> OnPostAsync([FromForm] string redirectUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(redirectUrl);
        }
    }
}
