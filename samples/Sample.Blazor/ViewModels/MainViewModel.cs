using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Sample.ViewModels;

public class MainViewModel : ViewModel
{
    private readonly NavigationManager _navigationService;

    public RelayCommand NavigateToHomeCommand { get; private set; }
    public RelayCommand NavigateToCounterCommand { get; private set; }
    public RelayCommand NavigateToWeatherCommand { get; private set; }
    public RelayCommand NavigateToCommandDemoCommand { get; private set; }

    public MainViewModel(ICommonServices commonServices, NavigationManager navigationService, ILogger<ViewModel> logger)
        : base(commonServices, logger)
    {
        _navigationService = navigationService;

        NavigateToHomeCommand = new RelayCommand(ExecuteNavigateToHome);
        NavigateToCounterCommand = new RelayCommand(ExecuteNavigateToCounter);
        NavigateToWeatherCommand = new RelayCommand(ExecuteNavigateToWeather);
        NavigateToCommandDemoCommand = new RelayCommand(ExecuteNavigateToCommandDemo);
    }

    private void ExecuteNavigateToCommandDemo()
    {
        _navigationService.NavigateTo("/commanddemoview");
    }

    private void ExecuteNavigateToWeather()
    {
        throw new NotImplementedException();
    }

    private void ExecuteNavigateToCounter()
    {
        throw new NotImplementedException();
    }

    private void ExecuteNavigateToHome()
    {
        throw new NotImplementedException();
    }
}
