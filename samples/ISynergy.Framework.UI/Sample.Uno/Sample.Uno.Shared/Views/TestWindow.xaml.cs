using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

public partial class TestWindow : ISynergy.Framework.UI.Controls.Window, IWindow
{
    public TestWindow()
    {
        InitializeComponent();
    }
}