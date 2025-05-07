using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult SetLanguage(string culture, string returnUrl = "/")
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        // Ensure returnUrl is not null
        return LocalRedirect(returnUrl ?? "/");
    }

}
