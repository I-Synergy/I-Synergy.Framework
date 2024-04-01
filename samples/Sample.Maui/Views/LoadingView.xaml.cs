using CommunityToolkit.Maui.Views;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.ViewModels;
using Sample.Models;

namespace Sample.Views;

[Singleton(true)]
public partial class LoadingView : ILoadingView
{
    public LoadingView(IContext context, LoadingViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();

        MessageService.Default.Register<ApplicationInitializedMessage>(this, ApplicationInitialized);
        BackgroundMediaElement.Source = MediaSource.FromResource("gta.mp4");
    }

    private void ApplicationInitialized(ApplicationInitializedMessage message) =>
        SkipButton.IsEnabled = true;

    private void SendApplicationLoadedMessage() =>
        MessageService.Default.Send(new ApplicationLoadedMessage());

    private void MediaElement_MediaEnded(object sender, EventArgs e) =>
        SendApplicationLoadedMessage();

    private void View_Loaded(object sender, EventArgs e) => 
        BackgroundMediaElement.Play();

    private void View_Unloaded(object sender, EventArgs e) =>
        BackgroundMediaElement.Handler?.DisconnectHandler();

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (SkipButton.IsEnabled)
        {
            BackgroundMediaElement.Pause();
            SendApplicationLoadedMessage();
        }
    }

    private void SkipClicked(object sender, EventArgs e)
    {
        BackgroundMediaElement.Pause();
        SendApplicationLoadedMessage();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MessageService.Default.Unregister<ApplicationInitializedMessage>(this);
        }
    }
}