using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

/// <summary>
/// Class InfoViewModel.
/// Implements the <see cref="ViewModelNavigation{Object}" />
/// </summary>
/// <seealso cref="ViewModelNavigation{Object}" />
public class InfoViewModel : ViewModelNavigation<object>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return _commonServices.LanguageService.GetString("Info"); } }

    /// <summary>
    /// Gets or sets the CompanyName property value.
    /// </summary>
    /// <value>The name of the company.</value>
    public string CompanyName
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the ProductName property value.
    /// </summary>
    /// <value>The name of the product.</value>
    public string ProductName
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Version property value.
    /// </summary>
    /// <value>The version.</value>
    public Version Version
    {
        get { return GetValue<Version>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Copyrights property value.
    /// </summary>
    /// <value>The copyrights.</value>
    public string Copyrights
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Startup property value.
    /// </summary>
    public string Startup
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public InfoViewModel(ICommonServices commonServices, ILogger<InfoViewModel> logger)
        : base(commonServices, logger)
    {
        CompanyName = _commonServices.InfoService.CompanyName;
        ProductName = _commonServices.InfoService.ProductName;
        Version = _commonServices.InfoService.ProductVersion;
        Copyrights = _commonServices.InfoService.Copyrights;
        Startup = Environment.GetEnvironmentVariable(nameof(Environment)) ?? "";
    }
}
