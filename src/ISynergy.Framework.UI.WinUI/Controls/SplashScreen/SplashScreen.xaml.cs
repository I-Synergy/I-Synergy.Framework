using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Configuration;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Playback;

namespace ISynergy.Framework.UI.Controls;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Lifetime(Lifetimes.Singleton)]
public sealed partial class SplashScreen
{
    private bool _initializationComplete;
    private bool _userInteracted;

    public SplashScreen()
    {
        this.InitializeComponent();
        this.Loaded += SplashScreen_Loaded;
        this.Unloaded += SplashScreen_Unloaded;
    }

    private async void SplashScreen_Loaded(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is SplashScreenViewModel viewModel && viewModel.Configuration?.AssetStreamProvider != null)
        {
            await StartMediaPlayback(viewModel.Configuration);
        }

        // Monitor the initialization task completion
        if (ViewModel is SplashScreenViewModel loadingViewModel)
        {
            try
            {
                await loadingViewModel.WaitForCompletionAsync();
                _initializationComplete = true;
                DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
                {
                    SignInButton.IsEnabled = true;
                });
            }
            catch (Exception)
            {
                // Handle initialization failure if needed
            }
        }
    }

    private async Task StartMediaPlayback(SplashScreenOptions configuration)
    {
        if (configuration.AssetStreamProvider != null)
        {
            using var stream = await configuration.AssetStreamProvider();

            switch (configuration.SplashScreenType)
            {
                case SplashScreenTypes.Video:
                    ConfigureVideoPlayback(stream, configuration);
                    break;

                case SplashScreenTypes.Image:
                    ConfigureImageDisplay(stream);
                    break;
            }
        }
    }

    private void ConfigureVideoPlayback(Stream stream, SplashScreenOptions configuration)
    {
        BackgroundMediaElement.Visibility = Visibility.Visible;
        BackgroundImage.Visibility = Visibility.Collapsed;

        BackgroundMediaElement.Source = configuration.CreateMediaSource(stream);
        BackgroundMediaElement.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        BackgroundMediaElement.MediaPlayer.Play();
    }

    private void ConfigureImageDisplay(Stream stream)
    {
        BackgroundMediaElement.Visibility = Visibility.Collapsed;
        BackgroundImage.Visibility = Visibility.Visible;

        var bitmap = new BitmapImage();
        stream.Position = 0;
        bitmap.SetSource(stream.AsRandomAccessStream());
        BackgroundImage.Source = bitmap;
    }

    private void SplashScreen_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (BackgroundMediaElement?.MediaPlayer != null)
        {
            BackgroundMediaElement.MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
            BackgroundMediaElement.MediaPlayer.Dispose();
        }
    }

    private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            if (_initializationComplete && !_userInteracted)
            {
                Complete();
            }
        });
    }

    private void Grid_Tapped(object? sender, TappedRoutedEventArgs e)
    {
        if (_initializationComplete && SignInButton.IsEnabled)
        {
            _userInteracted = true;
            Complete();
        }
    }

    public void SignInClicked(object? sender, RoutedEventArgs e)
    {
        if (_initializationComplete)
        {
            _userInteracted = true;
            Complete();
        }
    }

    private async void Complete()
    {
        if (BackgroundMediaElement.MediaPlayer is not null &&
            BackgroundMediaElement.MediaPlayer.CurrentState != MediaPlayerState.Stopped)
        {
            BackgroundMediaElement.MediaPlayer.Pause();
        }

        if (ViewModel is SplashScreenViewModel viewModel)
        {
            await viewModel.CompleteLoadingAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            this.Loaded -= SplashScreen_Loaded;
            this.Unloaded -= SplashScreen_Unloaded;
        }
    }
}
