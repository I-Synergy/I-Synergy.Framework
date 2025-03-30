using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace ISynergy.Framework.UI.Common;

/// <summary>
/// Class DeferredNavigation.
/// </summary>
public class DeferredNavigation
{
    /// <summary>
    /// The is busy
    /// </summary>
    private bool _isBusy = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeferredNavigation"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    public DeferredNavigation(Frame frame)
    {
        Frame = frame;
        Frame.Navigating += OnFrameNavigating;
    }

    /// <summary>
    /// Gets the frame.
    /// </summary>
    /// <value>The frame.</value>
    public Frame Frame { get; }

    /// <summary>
    /// Gets or sets the on navigating.
    /// </summary>
    /// <value>The on navigating.</value>
    public Func<NavigatingCancelEventArgs, Task>? OnNavigating { get; set; }

    /// <summary>
    /// Handles the <see cref="E:FrameNavigating" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> instance containing the event data.</param>
    private async void OnFrameNavigating(object? sender, NavigatingCancelEventArgs e)
    {
        e.Cancel = true;
        if (!_isBusy)
        {
            _isBusy = true;

            if (OnNavigating is not null)
            {
                await OnNavigating(e);

                if (!e.Cancel)
                {
                    switch (e.NavigationMode)
                    {
                        case NavigationMode.New:
                        case NavigationMode.Refresh:
                            Frame.Navigating -= OnFrameNavigating;
                            Frame.Navigate(e.SourcePageType, e.Parameter);
                            break;
                        case NavigationMode.Back:
                            Frame.Navigating -= OnFrameNavigating;
                            Frame.GoBack();
                            break;
                    }
                }
            }

            _isBusy = false;
        }
    }
}
