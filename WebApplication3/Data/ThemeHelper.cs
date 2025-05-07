public static class ThemeHelper
{
    public static string GetTheme(IHttpContextAccessor httpContextAccessor)
    {
        var theme = httpContextAccessor.HttpContext?.Session.GetString("Theme");
        return theme ?? "light-mode"; // Default to "light-mode" if no theme is set
    }
}
