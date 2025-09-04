using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;

namespace Sample.ViewModels;

public class CommandDemoViewModel : ViewModel
{
    /// <summary>
    /// Gets or sets the Counter property value.
    /// </summary>
    public int Counter
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public RelayCommand IncrementCommand { get; }
    public RelayCommand DecrementCommand { get; }
    public RelayCommand ResetCommand { get; }

    public CommandDemoViewModel(ICommonServices commonServices, ILogger<CommandDemoViewModel> logger)
        : base(commonServices, logger)
    {
        IncrementCommand = new RelayCommand(() => Counter++);
        DecrementCommand = new RelayCommand(() => Counter--, () => Counter > 0);
        ResetCommand = new RelayCommand(() => Counter = 0, () => Counter != 0);
    }
}
