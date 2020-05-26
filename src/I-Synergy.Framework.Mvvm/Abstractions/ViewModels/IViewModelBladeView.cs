using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface IViewModelBladeView : IViewModel
    {
        ObservableCollection<IView> Blades { get; set; }
        bool IsPaneEnabled { get; set; }
    }
}
