using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

/// <summary>
/// Class InfoView. This class cannot be inherited.
/// </summary>
public sealed partial class ConvertersView : ISynergy.Framework.UI.Controls.View, IView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfoView"/> class.
    /// </summary>
    public ConvertersView()
    {
        this.InitializeComponent();
    }
}
