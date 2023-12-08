using Android.App;
using Android.Views;

namespace Sample.Droid;

[Activity(
    MainLauncher = true,
    ConfigurationChanges = Uno.UI.ActivityHelper.AllConfigChanges,
    WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
)]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
}