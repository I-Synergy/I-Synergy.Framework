using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

/// <summary>
/// Class TestItemsListView. This class cannot be inherited.
/// </summary>
public sealed partial class TestItemsListView : ISynergy.Framework.UI.Controls.View, IView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestItemsListView"/> class.
    /// </summary>
    public TestItemsListView()
    {
        this.InitializeComponent();
    }
}
