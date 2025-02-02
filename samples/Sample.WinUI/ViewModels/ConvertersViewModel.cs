using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

/// <summary>
/// Class ConvertersViewModel.
/// </summary>
public class ConvertersViewModel : ViewModelNavigation<object>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("Converters"); } }


    /// <summary>
    /// Gets or sets the File property value.
    /// </summary>
    public byte[] FileBytes
    {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the FileDescription property value.
    /// </summary>
    public string Description
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the FileContentType property value.
    /// </summary>
    public string ContentType
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public AsyncRelayCommand<TestItem> NavigateToDetailCommand { get; private set; }
    public AsyncRelayCommand<TestItem> NavigateToPivotCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertersViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ConvertersViewModel(
        ICommonServices commonServices,
        ILoggerFactory loggerFactory)
        : base(commonServices, loggerFactory)
    {
        SelectedSoftwareEnvironment = (int)SoftwareEnvironments.Production;
        NavigateToDetailCommand = new AsyncRelayCommand<TestItem>(NavigateToDetailAsync);
        NavigateToPivotCommand = new AsyncRelayCommand<TestItem>(NavigateToPivotAsync);
    }

    /// <summary>
    /// Gets or sets the SoftwareEnvironments property value.
    /// </summary>
    /// <value>The software environments.</value>
    public SoftwareEnvironments SoftwareEnvironments
    {
        get { return GetValue<SoftwareEnvironments>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the SelectedSoftwareEnvironment property value.
    /// </summary>
    /// <value>The selected software environment.</value>
    public int SelectedSoftwareEnvironment
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the SoftwareEnvironments property value by enum value.
    /// </summary>
    /// <value>The software environments.</value>
    public SoftwareEnvironments SelectedSoftwareEnvironmentByEnum
    {
        get { return GetValue<SoftwareEnvironments>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IntegerValue property value.
    /// </summary>
    public int IntegerValue
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the DecimalValue property value.
    /// </summary>
    public decimal DecimalValue
    {
        get => GetValue<decimal>();
        set => SetValue(value);
    }

    private async Task NavigateToDetailAsync(TestItem item)
    {
        var detailsVm = new DetailsViewModel(_commonServices, _loggerFactory);
        await _commonServices.NavigationService.NavigateAsync(detailsVm);
    }

    private async Task NavigateToPivotAsync(TestItem item)
    {
        var detailsVm = new PivotViewModel(_commonServices, _loggerFactory);
        await _commonServices.NavigationService.NavigateAsync(detailsVm);
    }
}
