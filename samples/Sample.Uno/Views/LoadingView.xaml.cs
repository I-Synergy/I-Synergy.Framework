using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Windows.Media.Playback;
using MPlayer = Windows.Media.Playback.MediaPlayer;

namespace Sample.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Singleton(true)]
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
#if BROWSERWASM
        BackgroundMediaElement.MediaPlayer.IsMuted = true;
#endif

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

    private void MediaPlayer_MediaEnded(MPlayer sender, object args) =>
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
            BackgroundMediaElement.MediaPlayer.PlaybackSession is not null &&
            BackgroundMediaElement.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
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
