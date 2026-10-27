# Platform-Specific Code
Partial classes, conditional compilation and platform folders for Android, iOS, Windows and MacCatalyst. Load this when code must differ per platform.


### Permissions (Android)
```xml
<!-- File: Platforms/Android/AndroidManifest.xml -->
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true"></application>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
</manifest>
```

### Permissions (iOS)
```xml
<!-- File: Platforms/iOS/Info.plist -->
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSCameraUsageDescription</key>
    <string>This app needs access to the camera to take photos of receipts.</string>
    <key>NSPhotoLibraryUsageDescription</key>
    <string>This app needs access to your photo library to select images.</string>
    <key>NSLocationWhenInUseUsageDescription</key>
    <string>This app needs your location to tag transactions.</string>
</dict>
</plist>
```

### Platform-Specific Service
```csharp
// File: Services/IPlatformService.cs
namespace {ApplicationName}.Mobile.Services;

public interface IPlatformService
{
    Task<byte[]> TakePhotoAsync();
    Task<Location?> GetCurrentLocationAsync();
    Task ShowNotificationAsync(string title, string message);
}
```

```csharp
// File: Platforms/Android/Services/PlatformService.cs
#if ANDROID
using Android.Content;
using AndroidX.Core.App;

namespace {ApplicationName}.Mobile.Platforms.Android.Services;

public class PlatformService : IPlatformService
{
    public async Task<byte[]> TakePhotoAsync()
    {
        var photo = await MediaPicker.CapturePhotoAsync();

        if (photo is null)
            return Array.Empty<byte>();

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            return location;
        }
        catch (Exception ex)
        {
            // Handle error
            return null;
        }
    }

    public async Task ShowNotificationAsync(string title, string message)
    {
        var notificationManager = NotificationManagerCompat.From(Platform.CurrentActivity!);

        var notification = new NotificationCompat.Builder(Platform.CurrentActivity!, "default")
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(Resource.Drawable.notification_icon)
            .SetPriority(NotificationCompat.PriorityDefault)
            .Build();

        notificationManager.Notify(0, notification);
    }
}
#endif
```

```csharp
// File: Platforms/iOS/Services/PlatformService.cs
#if IOS
using UserNotifications;

namespace {ApplicationName}.Mobile.Platforms.iOS.Services;

public class PlatformService : IPlatformService
{
    public async Task<byte[]> TakePhotoAsync()
    {
        var photo = await MediaPicker.CapturePhotoAsync();

        if (photo is null)
            return Array.Empty<byte>();

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            return location;
        }
        catch (Exception ex)
        {
            // Handle error
            return null;
        }
    }

    public async Task ShowNotificationAsync(string title, string message)
    {
        var content = new UNMutableNotificationContent
        {
            Title = title,
            Body = message,
            Sound = UNNotificationSound.Default
        };

        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
        var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);

        await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
    }
}
#endif
```

