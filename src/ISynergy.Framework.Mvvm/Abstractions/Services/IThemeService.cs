namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface IThemeService
{
    bool IsLightThemeEnabled { get; }
    void ApplyTheme();
}
