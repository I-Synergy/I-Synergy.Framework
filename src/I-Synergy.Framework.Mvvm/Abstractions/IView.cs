using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions
{
    public interface IView
    {
        IViewModel DataContext { get; set; }
        bool IsEnabled { get; set; }
        void View_Unloaded(object sender, object e);
        void View_Loaded(object sender, object e);
    }
}
