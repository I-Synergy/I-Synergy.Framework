using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
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
        ILogger<TestViewModel> logger)
        : base(commonServices, logger)
    {
        Units =
        [
            new TileItem() { Description = "A", Header = "Header A" },
            new TileItem() { Description = "B", Header = "Header B" },
            new TileItem() { Description = "C", Header = "Header C" }
        ];
    }
}
