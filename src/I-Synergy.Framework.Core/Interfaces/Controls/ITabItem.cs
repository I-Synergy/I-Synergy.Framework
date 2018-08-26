using ISynergy.ViewModels.Base;

namespace ISynergy.Controls.Interfaces
{
    public interface ITabItem
    {
        IView Content { get; set; }
        IViewModel DataContext { get; set; }
        string Header { get; set; }
        bool CanClose { get; set; }
    }
}