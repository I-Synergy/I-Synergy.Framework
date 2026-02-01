using ISynergy.Framework.Core.Abstractions.Services;
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
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return _commonServices.LanguageService.GetString("Converters"); } }


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
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="logger"></param>
    public ConvertersViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, ILogger<ConvertersViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;

        NavigateToDetailCommand = new AsyncRelayCommand<TestItem>(NavigateToDetailAsync);
        NavigateToPivotCommand = new AsyncRelayCommand<TestItem>(NavigateToPivotAsync);
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
        var detailsVm = _commonServices.ScopedContextService.GetRequiredService<DetailsViewModel>();
        await _navigationService.NavigateAsync(detailsVm);
    }

    private async Task NavigateToPivotAsync(TestItem item)
    {
        var detailsVm = _commonServices.ScopedContextService.GetRequiredService<PivotViewModel>();
        await _navigationService.NavigateAsync(detailsVm);
    }
}
