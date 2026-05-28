using ISynergy.Framework.Core.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides shared time-of-day greeting resolution for dialog services.
/// </summary>
public abstract class BaseDialogService
{
    /// <summary>
    /// Resolves the appropriate greeting based on the current time of day.
    /// </summary>
    /// <param name="name">The user's name.</param>
    /// <param name="languageService">The language service for localized strings.</param>
    /// <returns>A tuple of (greetingMessage, title) for the resolved greeting.</returns>
    protected static (string message, string title) ResolveGreeting(string name, ILanguageService languageService)
    {
        var hour = DateTime.Now.Hour;

        string greeting;
        if (hour < 6)
            greeting = languageService.GetString("Greeting_Night");
        else if (hour < 12)
            greeting = languageService.GetString("Greeting_Morning");
        else if (hour < 18)
            greeting = languageService.GetString("Greeting_Afternoon");
        else
            greeting = languageService.GetString("Greeting_Evening");

        return (string.Format(greeting, name), languageService.GetString("TitleWelcome"));
    }
}
