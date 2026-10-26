using Android.App;
using Android.Content.PM;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Sample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    /// <summary>
    /// Routes the Android hardware back button through the framework navigation service, so it
    /// walks the shared backstack (modals first) exactly like the header's back button. When there
    /// is nothing to go back to, the platform's default handling wins (it closes the app).
    /// </summary>
    public override void OnBackPressed()
    {
        var navigationService = IPlatformApplication.Current?.Services?.GetService<INavigationService>();

        if (navigationService?.CanGoBack == true)
        {
            _ = navigationService.GoBackAsync();
            return;
        }

        base.OnBackPressed();
    }
}
