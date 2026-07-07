using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class HelpViewModel : ViewModelNavigation<object>
{
    private readonly INavigationService _navigationService;

    public override string Title { get => _commonServices.LanguageService.GetString("Help"); }

    public string ProductName
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public Version Version
    {
        get => GetValue<Version>();
        set => SetValue(value);
    }

    public string CompanyName
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string Copyrights
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public HelpViewModel(
        ICommonServices commonServices,
        INavigationService navigationService,
        ILogger<HelpViewModel> logger)
        : base(commonServices, logger)
    {
        _navigationService = navigationService;

        ProductName = _commonServices.InfoService.ProductName;
        Version = _commonServices.InfoService.ProductVersion;
        CompanyName = _commonServices.InfoService.CompanyName;
        Copyrights = _commonServices.InfoService.Copyrights;
    }

    public override async Task CancelAsync()
    {
        await base.CancelAsync();
        await _navigationService.GoBackAsync();
    }
}
