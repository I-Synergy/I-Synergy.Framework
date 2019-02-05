using ISynergy.ViewModels.Base;

namespace ISynergy
{
    public interface IView
    {
        IViewModel DataContext { get; set; }
    }
}
