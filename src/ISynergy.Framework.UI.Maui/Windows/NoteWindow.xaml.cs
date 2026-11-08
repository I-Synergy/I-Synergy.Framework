using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI.Windows;

public partial class NoteWindow : INoteWindow // NOSONAR - Dispose is implemented by the framework Window base class
{
    public NoteWindow()
    {
        InitializeComponent();
    }
}