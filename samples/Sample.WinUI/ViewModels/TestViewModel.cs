using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

public class TestViewModel : ViewModelDialog<object>
{

    /// <summary>
    /// Gets or sets the Units property value.
    /// </summary>
    public ObservableCollection<TileItem> Units
    {
        get => GetValue<ObservableCollection<TileItem>>();
        set => SetValue(value);
    }

    public TestViewModel(
        ICommonServices commonServices,
        bool automaticValidation = false)
        : base(commonServices, automaticValidation)
    {
        Units =
        [
            new TileItem() { Description = "A", Header = "Header A" },
            new TileItem() { Description = "B", Header = "Header B" },
            new TileItem() { Description = "C", Header = "Header C" }
        ];
    }
}
