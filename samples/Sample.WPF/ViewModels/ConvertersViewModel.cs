using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertersViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    public ConvertersViewModel(ICommonServices commonServices)
        : base(commonServices)
    {
        SelectedSoftwareEnvironment = (int)SoftwareEnvironments.Production;
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

}
