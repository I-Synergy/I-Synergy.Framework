using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
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
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        Units =
        [
            new TileItem() { Description = "A", Header = "Header A" },
            new TileItem() { Description = "B", Header = "Header B" },
            new TileItem() { Description = "C", Header = "Header C" }
        ];
    }
}
