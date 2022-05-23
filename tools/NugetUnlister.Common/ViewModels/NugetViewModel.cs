using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using NugetUnlister.Common.Abstractions;
using NugetUnlister.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Extensions;

namespace NugetUnlister.Common.ViewModels
{
    public class NugetViewModel : ViewModelNavigation<PackageVersion>
    {
        private readonly INugetService _nugetService;

        public Command ListVersionCommand { get; set; }
        public Command SelectAllCommand { get; set; }
        public Command DeselectAllCommand { get; set; }
        public Command UnlistCommand { get; set; }


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

        public Command Settings_Command { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public NugetViewModel(
            IContext context,
            ICommonServices commonServices,
            INugetService nugetService,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            Items = new ObservableCollection<PackageVersion>();
            Logs = new ObservableCollection<string>();

            _nugetService = nugetService;

            ListVersionCommand = new Command(async () => await ListVersionAsync());
            UnlistCommand = new Command(async () => await UnlistPackageAsync());
            SelectAllCommand = new Command(() =>
            {
                foreach (var item in Items)
                {
                    item.Selected = true;
                }
            });
            DeselectAllCommand = new Command(() =>
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
