using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
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

        MessageService.Default.Register<ApplicationInitializedMessage>(this, ApplicationInitialized);
    }

    private void LoadingView_Loaded(object sender, RoutedEventArgs e)
    {
        Assembly currentAssembly = Assembly.GetAssembly(typeof(LoadingView));
        var stream = currentAssembly.GetManifestResourceStream("Sample.Assets.gta.mp4").AsRandomAccessStream();
        BackgroundMediaElement.Source = MediaSource.CreateFromStream(stream, "video/mp4");
        BackgroundMediaElement.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        BackgroundMediaElement.MediaPlayer.Play();
    }

    private void LoadingView_Unloaded(object sender, RoutedEventArgs e)
    {
        BackgroundMediaElement.MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
        BackgroundMediaElement.MediaPlayer.Dispose();
    }

    public void ApplicationInitialized(ApplicationInitializedMessage message) =>
        SignInButton.IsEnabled = true;

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

        MessageService.Default.Send(new ApplicationLoadedMessage());
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            this.Loaded -= LoadingView_Loaded;
            this.Unloaded -= LoadingView_Unloaded;

            MessageService.Default.Unregister<ApplicationInitializedMessage>(this);
        }
    }
}
