using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TezosSignatureAuth.AspNetCore.Pages
{
    public class IndexModel : PageModel
    {
        public bool IsAuthenticated
            => User.Identity?.IsAuthenticated == true;

        public string Address
            => User.Identity?.Name ?? string.Empty;
    }
}