using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TezosSignatureAuth.AspNetCore.Services;

namespace TezosSignatureAuth.AspNetCore.Pages.Auth
{
    public class SignInModel : PageModel
    {
        readonly AuthService Auth;

        public SignInModel(AuthService auth) => Auth = auth;

        public IActionResult OnGet() => NotFound();

        public async Task<IActionResult> OnPostAsync([FromForm] AuthMessage message, [FromForm] string redirectUrl)
        {
            if (!ModelState.IsValid)
                return LocalRedirect("/");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!Auth.TryAuthenticate(message, out var address, out var role))
                return LocalRedirect("/");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, address!),
                new Claim(ClaimTypes.Role, role!),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return LocalRedirect(redirectUrl);
        }
    }
}
