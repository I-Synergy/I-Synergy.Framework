using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using NugetUnlister.Abstractions;
using NugetUnlister.Models;
using System.Collections.ObjectModel;

namespace NugetUnlister.ViewModels;

public class NugetViewModel : ViewModelNavigation<PackageVersion>
{
    private readonly INugetService _nugetService;

    public AsyncRelayCommand ListVersionCommand { get; private set; }
    public RelayCommand SelectAllCommand { get; private set; }
    public RelayCommand DeselectAllCommand { get; private set; }
    public AsyncRelayCommand UnlistCommand { get; private set; }


    /// <summary>
    /// Gets or sets the PackageId property value.
    /// </summary>
    public string PackageId
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Items property value.
    /// </summary>
    public ObservableCollection<PackageVersion> Items
    {
        get => GetValue<ObservableCollection<PackageVersion>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Logs property value.
    /// </summary>
    public ObservableCollection<string> Logs
    {
        get => GetValue<ObservableCollection<string>>();
        set => SetValue(value);
    }

    public RelayCommand SettingsCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public NugetViewModel(
        ICommonServices commonServices,
        ILogger<NugetViewModel> logger,
        INugetService nugetService)
        : base(commonServices, logger)
    {
        Items = [];
        Logs = [];

        _nugetService = nugetService;

        ListVersionCommand = new AsyncRelayCommand(async () => await ListVersionAsync());
        UnlistCommand = new AsyncRelayCommand(async () => await UnlistPackageAsync());
        SelectAllCommand = new RelayCommand(() =>
        {
            foreach (PackageVersion item in Items.EnsureNotNull())
            {
                item.Selected = true;
            }
        });
        DeselectAllCommand = new RelayCommand(() =>
        {
            foreach (PackageVersion item in Items.EnsureNotNull())
            {
                item.Selected = false;
            }
        });
    }

    private async Task UnlistPackageAsync()
    {
        Argument.IsNotNullOrEmpty(PackageId);

        _commonServices.BusyService.StartBusy();

        int i = 0;
        System.Collections.Generic.IEnumerable<PackageVersion> selected = Items.Where(q => q.Selected);
        int count = selected.Count();

        foreach (PackageVersion item in selected.EnsureNotNull())
        {
            i++;
            await _nugetService.UnlistPackageAsync(item.PackageId, item.Version);
            _commonServices.BusyService.UpdateMessage($"Processed package ({item.Version}) {i} from {count}...");
        }

        _commonServices.BusyService.StopBusy();
    }

    private async Task ListVersionAsync()
    {
        Argument.IsNotNullOrEmpty(PackageId);
        System.Collections.Generic.List<PackageVersion> versions = await _nugetService.ListVersionAsync(PackageId);
        Items.AddNewRange(versions);
    }
}
