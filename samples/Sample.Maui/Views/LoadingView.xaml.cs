using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.ViewModels;

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

    public void ApplicationInitialized(ApplicationInitializedMessage message) =>
        SignInButton.IsEnabled = true;

    private void MediaElement_MediaEnded(object sender, EventArgs e) => 
        Complete();

    private void View_Loaded(object sender, EventArgs e) =>
        BackgroundMediaElement.Play();

    private void View_Unloaded(object sender, EventArgs e) =>
        BackgroundMediaElement.Handler?.DisconnectHandler();

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (SignInButton.IsEnabled)
            Complete();
    }

    public void SignInClicked(object sender, EventArgs e) => 
        Complete();

    private void Complete()
    {
        if (BackgroundMediaElement.CurrentState != MediaElementState.Stopped)
            BackgroundMediaElement.Pause();

        MessageService.Default.Send(new ApplicationLoadedMessage());
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            MessageService.Default.Unregister<ApplicationInitializedMessage>(this);
    }
}