namespace ISynergy.Framework.UI.Abstractions.Services;

public interface IThemeService
{
    bool IsLightThemeEnabled { get; }
    Style Style { get; }
    void SetStyle();
}
