using ISynergy.Services;
using ISynergy.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using System.Globalization;
using Windows.UI.Xaml.Resources;
using Microsoft.ApplicationInsights;
using Serilog;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using ISynergy.ViewModels.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using ISynergy.Events;

namespace ISynergy
{
    public static class ApplicationHandlers
    {
        public static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
                return null;

            for (var i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name)
                        return frameworkElement;

                    frameworkElement = GetDescendantFromName(frameworkElement, name);

                    if (frameworkElement != null)
                        return frameworkElement;
                }
            }

            return null;
        }

        public static void Register(Assembly[] assemblies)
        {
            List<Type> viewTypes = new List<Type>();
            List<Type> viewmodelTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                viewTypes.AddRange(assembly.GetTypes().Where(q => q.Name.EndsWith(Constants.View) && q.GetInterface(nameof(IView), false) != null).ToList());
                viewmodelTypes.AddRange(assembly.GetTypes().Where(q => q.Name.EndsWith(Constants.ViewModel) && q.GetInterface(nameof(IViewModel), false) != null).ToList());
            }

            foreach (Type view in viewTypes)
            {
                Type viewmodel = viewmodelTypes.Where(q => q.Name == view.Name.ReplaceLastOf(Constants.View, Constants.ViewModel)).FirstOrDefault();

                if (viewmodel != null)
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().Configure(viewmodel.FullName, view);
                }
            }
        }

        public static void SetContext(
            IContext context, 
            ILogger logger, 
            bool preview,
            string previewApiUrl = @"https://app-test.i-synergy.nl/api",
            string previewAccountUrl = @"https://app-test.i-synergy.nl/account",
            string previewTokenUrl = @"https://app-test.i-synergy.nl/oauth/token",
            string previewWebUrl = @"http://test.i-synergy.nl/")
        {
            context = ServiceLocator.Current.GetInstance<IContext>();

            if (preview)
            {
                context.Environment = ".preview";
                context.ApiUrl = previewApiUrl;
                context.AccountUrl = previewAccountUrl;
                context.TokenUrl = previewTokenUrl;
                context.WebUrl = previewWebUrl;
            }

            logger.Information("Update settings");
            ServiceLocator.Current.GetInstance<ISettingsServiceBase>().CheckForUpgrade();

            string culture = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Culture;

            if (culture == null) culture = "en";

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).InstrumentationKey = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().ApplicationInsights_InstrumentationKey;
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.User.UserAgent = ServiceLocator.Current.GetInstance<IInfoService>().ProductName;
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.Session.Id = Guid.NewGuid().ToString();
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.Component.Version = ServiceLocator.Current.GetInstance<IInfoService>().ProductVersion.ToString();
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            CustomXamlResourceLoader.Current = new CustomResourceLoader(ServiceLocator.Current.GetInstance<ILanguageService>());
        }

        public static async Task HandleException(Exception ex)
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();

            if (!(connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess))
            {
                await ServiceLocator.Current.GetInstance<IDialogService>().ShowInformationAsync(
                    ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_DEFAULT_INTERNET"));
            }
            else
            {
                if (ex is NotImplementedException)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowInformationAsync(
                        ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_FUTURE_MODULE"));
                }
                else if(ex is UnauthorizedAccessException)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ex.Message);
                }
                else if (ex is IOException)
                {
                    if (ex.Message.Contains("The process cannot access the file") && ex.Message.Contains("because it is being used by another process"))
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                    }
                }
                else if (ex is ArgumentException)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            ((ArgumentException)ex).ParamName)
                        );
                }
                else
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                        ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                }
            }

            try
            {
                ServiceLocator.Current.GetInstance<ITelemetryService>().TrackException(ex);
                ServiceLocator.Current.GetInstance<ITelemetryService>().Flush();
            }
            catch { }
            finally
            {
                Messenger.Default.Send(new ExceptionHandledMessage());
            }
        }
    }
}
