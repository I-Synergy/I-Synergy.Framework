using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertersViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public ConvertersViewModel(ICommonServices commonServices, ILogger<ConvertersViewModel> logger)
        : base(commonServices, logger)
    {
    }
}
