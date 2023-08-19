using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
    public class AppShellViewModel : BaseShellViewModel, IShellViewModel
    {
        public AsyncRelayCommand ControlsCommand { get; set; }
        public AsyncRelayCommand DialogsCommand { get; set; }

        public AppShellViewModel(
            IContext context,
            ICommonServices commonServices,
            IBaseApplicationSettingsService baseApplicationSettingsService,
            IAuthenticationService authenticationService,
            ILogger<AppShellViewModel> logger,
            IThemeService themeService,
            ILocalizationService localizationService)
            : base(context, commonServices, baseApplicationSettingsService, authenticationService, logger, themeService, localizationService)
        {
            ControlsCommand = new AsyncRelayCommand(ControlsAsync);
            DialogsCommand = new AsyncRelayCommand(DialogsAsync);
        }

        private async Task DialogsAsync()
        {
            var e = new ObservableCollection<TestItem>()
            {
                new TestItem { Id = 1, Description = "Test 1"},
                new TestItem { Id = 2, Description = "Test 2"},
                new TestItem { Id = 3, Description = "Test 3"},
                new TestItem { Id = 4, Description = "Test 4"},
                new TestItem { Id = 5, Description = "Test 5"},
                new TestItem { Id = 1, Description = "Test 6"},
                new TestItem { Id = 2, Description = "Test 7"},
                new TestItem { Id = 3, Description = "Test 8"},
                new TestItem { Id = 4, Description = "Test 9"},
                new TestItem { Id = 5, Description = "Test 10"},
                new TestItem { Id = 1, Description = "Test 11"},
                new TestItem { Id = 2, Description = "Test 12"},
                new TestItem { Id = 3, Description = "Test 13"},
                new TestItem { Id = 4, Description = "Test 14"},
                new TestItem { Id = 5, Description = "Test 15"}
            };

            await BaseCommonServices.NavigationService.NavigateAsync<DialogsViewModel>(e);
        }

        private Task ControlsAsync() =>
            BaseCommonServices.NavigationService.NavigateAsync<ControlsViewModel>();

        protected override Task OpenSettingsAsync() => throw new NotImplementedException();

        protected override void PopulateNavItems() => throw new NotImplementedException();

        protected override async Task RestartApplicationAsync()
        {
            await base.RestartApplicationAsync();
            Application.Current.Quit();
        }
    }
}
