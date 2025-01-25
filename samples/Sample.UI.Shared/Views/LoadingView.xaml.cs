using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Reflection;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sample.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Lifetime(Lifetimes.Singleton)]
public sealed partial class LoadingView : ILoadingView
{
    public LoadingView()
    {
        this.InitializeComponent();
        this.Loaded += LoadingView_Loaded;
        this.Unloaded += LoadingView_Unloaded;

        if (Application.Current is IBaseApplication baseApplication)
            baseApplication.ApplicationInitialized += ApplicationInitialized;
    }

    private void ApplicationInitialized(object sender, ReturnEventArgs<bool> e)
    {
        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => SignInButton.IsEnabled = true);
    }

    private async void LoadingView_Loaded(object sender, RoutedEventArgs e)
    {
        var currentAssembly = Assembly.GetAssembly(typeof(LoadingView));
        using var stream = currentAssembly.GetManifestResourceStream("Sample.Assets.gta.mp4");
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var randomAccessStream = new InMemoryRandomAccessStream();
        var outputStream = randomAccessStream.GetOutputStreamAt(0);
        var dataWriter = new DataWriter(outputStream);

        dataWriter.WriteBytes(memoryStream.ToArray());
        await dataWriter.StoreAsync();
        await outputStream.FlushAsync();

        BackgroundMediaElement.Source = MediaSource.CreateFromStream(randomAccessStream, "video/mp4");
        BackgroundMediaElement.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        BackgroundMediaElement.MediaPlayer.Play();
    }

    private void LoadingView_Unloaded(object sender, RoutedEventArgs e)
    {
        if (BackgroundMediaElement?.MediaPlayer != null)
        {
            BackgroundMediaElement.MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
            BackgroundMediaElement.MediaPlayer.Dispose();
        }
    }

    private void MediaPlayer_MediaEnded(MediaPlayer sender, object args) =>
        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => Complete());

    private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (SignInButton.IsEnabled)
            Complete();
    }

    public void SignInClicked(object sender, RoutedEventArgs e) =>
        Complete();

    private void Complete()
    {
        if (BackgroundMediaElement.MediaPlayer is not null &&
            BackgroundMediaElement.MediaPlayer.CurrentState != MediaPlayerState.Stopped)
            BackgroundMediaElement.MediaPlayer.Pause();

        if (Application.Current is IBaseApplication baseApplication)
            baseApplication.RaiseApplicationLoaded();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            this.Loaded -= LoadingView_Loaded;
            this.Unloaded -= LoadingView_Unloaded;

            if (Application.Current is IBaseApplication baseApplication)
                baseApplication.ApplicationInitialized -= ApplicationInitialized;
        }
    }
}
