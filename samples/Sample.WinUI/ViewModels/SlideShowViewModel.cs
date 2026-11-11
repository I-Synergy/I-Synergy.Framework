using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Sample.Models;
using System.Collections.ObjectModel;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Sample.ViewModels;

/// <summary>
/// Class SlideShowViewModel.
/// </summary>
public partial class SlideShowViewModel : ViewModelNavigation<MediaItem>
{
    private readonly DispatcherQueue? _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return _commonServices.LanguageService.GetString("Display"); } }

    /// <summary>
    /// Gets or sets the Items property value.
    /// </summary>
    /// <value>The items.</value>
    public ObservableCollection<MediaItem> Items
    {
        get { return GetValue<ObservableCollection<MediaItem>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Timer property value.
    /// </summary>
    /// <value>The slideshow timer.</value>
    public Timer SlideshowTimer
    {
        get { return GetValue<Timer>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the UpdateSourceTimer property value.
    /// </summary>
    /// <value>The update source timer.</value>
    public Timer UpdateSourceTimer
    {
        get { return GetValue<Timer>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SlideShowViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public SlideShowViewModel(ICommonServices commonServices, ILogger<SlideShowViewModel> logger)
        : base(commonServices, logger)
    {
        UpdateSourceTimer = new Timer(TimeSpan.FromMinutes(30).TotalMilliseconds);
        UpdateSourceTimer.Elapsed += UpdateSourceTimer_Tick;
        UpdateSourceTimer.AutoReset = true;
        UpdateSourceTimer.Start();

        // Get initial images from source.
        Items = CreateItemsFromConfiguration();

        // Set timer if images count is at least 1.
        if (Items.Count > 0)
        {
            SlideshowTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            SlideshowTimer.Elapsed += SlideshowTimer_Tick;
            UpdateSourceTimer.AutoReset = true;
            SlideshowTimer.Start();
        }
    }

    /// <summary>
    /// Creates the slide show items from the demo image locations configured in the application
    /// settings, so no absolute URIs are hardcoded in the application itself.
    /// </summary>
    private static ObservableCollection<MediaItem> CreateItemsFromConfiguration()
    {
        var imageUris = Application.Services
            .GetRequiredService<IConfiguration>()
            .GetSection("SlideShow:Images")
            .Get<string[]>() ?? [];

        return new ObservableCollection<MediaItem>(
            imageUris.Select((imageUri, index) => new MediaItem { Index = index, ImageUri = imageUri }));
    }

    /// <summary>
    /// slideshow time out timer tick as an asynchronous operation.
    /// </summary>
    private void SlideshowTimer_Tick(object? sender, ElapsedEventArgs e)
    {
        SlideshowTimer.Enabled = false;


        Task.Run(() =>
        {
            _dispatcherQueue?.TryEnqueue(() =>
            {
                if ((SelectedItem is null && Items is not null) || (SelectedItem is not null && Items is not null && SelectedItem.Index == Items.Count - 1))
                {
                    SetSelectedItem(Items[0]);
                }
                else if (SelectedItem is not null && Items is not null && SelectedItem.Index < (Items.Count - 1))
                {
                    SetSelectedItem(Items.Single(q => q.Index == SelectedItem.Index + 1));
                }
            });
        });

        SlideshowTimer.Enabled = true;
    }

    /// <summary>
    /// Updates the source time out timer tick synchronous.
    /// </summary>
    private void UpdateSourceTimer_Tick(object? sender, ElapsedEventArgs e)
    {
        UpdateSourceTimer.Enabled = false;
        _dispatcherQueue?.TryEnqueue(() =>
        {
            Items = CreateItemsFromConfiguration();
        });
        UpdateSourceTimer.Enabled = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (SlideshowTimer is not null)
            {
                SlideshowTimer.Stop();
                SlideshowTimer.Elapsed -= SlideshowTimer_Tick;
                SlideshowTimer.Dispose();
            }

            if (UpdateSourceTimer is not null)
            {
                UpdateSourceTimer.Stop();
                UpdateSourceTimer.Elapsed -= UpdateSourceTimer_Tick;
                UpdateSourceTimer.Dispose();
            }
        }

        base.Dispose(disposing);
    }
}
