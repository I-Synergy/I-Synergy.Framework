using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;

namespace Sample.ViewModels;

public class MainViewModel : ViewModel
{
    public AsyncRelayCommand NavigateToHomeCommand { get; private set; }
    public AsyncRelayCommand NavigateToCounterCommand { get; private set; }
    public AsyncRelayCommand NavigateToWeatherCommand { get; private set; }
    public AsyncRelayCommand NavigateToCommandDemoCommand { get; private set; }

    public MainViewModel(ICommonServices commonServices, ILogger<ViewModel> logger)
        : base(commonServices, logger)
    {
        NavigateToHomeCommand = new AsyncRelayCommand(ExecuteNavigateToHomeAsync);
        NavigateToCounterCommand = new AsyncRelayCommand(ExecuteNavigateToCounterAsync);
        NavigateToWeatherCommand = new AsyncRelayCommand(ExecuteNavigateToWeatherAsync);
        NavigateToCommandDemoCommand = new AsyncRelayCommand(ExecuteNavigateToCommandDemoAsync);
    }

    private Task ExecuteNavigateToCommandDemoAsync()
    {
        return _commonServices.NavigationService.NavigateAsync<CommandDemoViewModel>();
    }

    private Task ExecuteNavigateToWeatherAsync()
    {
        throw new NotImplementedException();
    }

    private Task ExecuteNavigateToCounterAsync()
    {
        throw new NotImplementedException();
    }

    private Task ExecuteNavigateToHomeAsync()
    {
        throw new NotImplementedException();
    }
}
