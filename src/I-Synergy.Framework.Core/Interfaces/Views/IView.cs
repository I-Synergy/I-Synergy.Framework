using ISynergy.Mvvm;

namespace ISynergy
{
    public interface IView
    {
        IViewModel DataContext { get; set; }
    }
}
