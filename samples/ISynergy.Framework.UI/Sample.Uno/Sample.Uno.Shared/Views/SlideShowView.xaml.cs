using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

/// <summary>
/// Class SlideShowView. This class cannot be inherited.
/// </summary>
public sealed partial class SlideShowView : ISynergy.Framework.UI.Controls.View, IView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SlideShowView"/> class.
    /// </summary>
    public SlideShowView()
    {
        this.InitializeComponent();
    }
}
