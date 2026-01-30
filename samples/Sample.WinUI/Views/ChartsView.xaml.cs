using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ChartsView : ISynergy.Framework.UI.Controls.View, IView
{
    public ChartsView()
    {
        this.InitializeComponent();
    }
}
