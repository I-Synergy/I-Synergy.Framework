using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;

namespace Sample.ViewModels
{
    public class AppShellViewModel : BaseShellViewModel, IShellViewModel
    {
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
        }

        protected override Task OpenSettingsAsync() => throw new NotImplementedException();

        protected override void PopulateNavItems() => throw new NotImplementedException();
    }
}
