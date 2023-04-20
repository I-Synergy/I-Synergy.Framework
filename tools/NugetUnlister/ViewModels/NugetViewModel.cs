using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using NugetUnlister.Abstractions;
using NugetUnlister.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NugetUnlister.ViewModels
{
    public class NugetViewModel : ViewModelNavigation<PackageVersion>
    {
        private readonly INugetService _nugetService;

        public AsyncRelayCommand ListVersionCommand { get; set; }
        public RelayCommand SelectAllCommand { get; set; }
        public RelayCommand DeselectAllCommand { get; set; }
        public AsyncRelayCommand UnlistCommand { get; set; }


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

        public RelayCommand Settings_Command { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public NugetViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            INugetService nugetService,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            Items = new ObservableCollection<PackageVersion>();
            Logs = new ObservableCollection<string>();

            _nugetService = nugetService;

            ListVersionCommand = new AsyncRelayCommand(async () => await ListVersionAsync());
            UnlistCommand = new AsyncRelayCommand(async () => await UnlistPackageAsync());
            SelectAllCommand = new RelayCommand(() =>
            {
                foreach (var item in Items)
                {
                    item.Selected = true;
                }
            });
            DeselectAllCommand = new RelayCommand(() =>
            {
                foreach (var item in Items)
                {
                    item.Selected = false;
                }
            });
        }

        private async Task UnlistPackageAsync()
        {
            Argument.IsNotNullOrEmpty(PackageId);

            BaseCommonServices.BusyService.StartBusy();

            var i = 0;
            var selected = Items.Where(q => q.Selected);
            var count = selected.Count();

            foreach (var item in selected)
            {
                i++;
                await _nugetService.UnlistPackageAsync(item.PackageId, item.Version);
                BaseCommonServices.BusyService.BusyMessage = $"Processed package ({item.Version}) {i} from {count}...";
            }

            BaseCommonServices.BusyService.EndBusy();
        }

        private async Task ListVersionAsync()
        {
            Argument.IsNotNullOrEmpty(PackageId);
            var versions = await _nugetService.ListVersionAsync(PackageId);
            Items.AddNewRange(versions);
        }
    }
}
