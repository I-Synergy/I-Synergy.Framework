using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface IThemeService
{
    bool IsLightThemeEnabled { get; }
    Style Style { get; }
    void SetStyle();
}
