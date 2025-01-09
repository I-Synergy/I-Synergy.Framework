using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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
    public override string Title { get { return LanguageService.Default.GetString("Info"); } }

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
    /// <param name="scopedContextService">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    public InfoViewModel(
        IScopedContextService scopedContextService,
        ICommonServices commonServices,
        ILogger logger)
        : base(scopedContextService, commonServices, logger)
    {
        CompanyName = InfoService.Default.CompanyName;
        ProductName = InfoService.Default.ProductName;
        Version = InfoService.Default.ProductVersion;
        Copyrights = InfoService.Default.Copyrights;

        var context = scopedContextService.GetService<IContext>();
        Startup = context.Environment.ToString();
    }
}
