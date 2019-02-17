using ISynergy.Mvvm;

namespace ISynergy
{
    public interface IView
    {
        IViewModel DataContext { get; set; }
        void View_Unloaded(object sender, object e);
        void View_Loaded(object sender, object e);
    }
}
