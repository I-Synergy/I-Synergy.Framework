using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
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
public class SlideShowViewModel : ViewModelNavigation<MediaItem>
{
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("Display"); } }

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
        Items = new ObservableCollection<MediaItem>()
        {
            new MediaItem { Index = 0, ImageUri = "http://3.bp.blogspot.com/-gxIdD54Xngg/UHcjjul0xHI/AAAAAAAAAA8/CkdJsPJ9qlQ/s1600/Microsoft-Windows-7-wallpaper-HD+(6).jpg" },
            new MediaItem { Index = 1, ImageUri = "http://3.bp.blogspot.com/-mo_E98lebOM/UHcjgEm5vdI/AAAAAAAAAA0/zLbJOvWRa8M/s1600/Microsoft-Windows-7-wallpaper-HD+(5).jpg" },
            new MediaItem { Index = 2, ImageUri = "https://wallpapercave.com/wp/W4ab0vD.jpg" },
            new MediaItem { Index = 3, ImageUri = "http://getwallpapers.com/wallpaper/full/c/6/8/100549.jpg" }
        };

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
    /// slideshow time out timer tick as an asynchronous operation.
    /// </summary>
    private void SlideshowTimer_Tick(object? sender, ElapsedEventArgs e)
    {
        SlideshowTimer.Enabled = false;


        Task.Run(() =>
        {
            _dispatcherQueue.TryEnqueue(() =>
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

        Items = new ObservableCollection<MediaItem>()
        {
            new MediaItem { Index = 0, ImageUri = "http://3.bp.blogspot.com/-gxIdD54Xngg/UHcjjul0xHI/AAAAAAAAAA8/CkdJsPJ9qlQ/s1600/Microsoft-Windows-7-wallpaper-HD+(6).jpg" },
            new MediaItem { Index = 1, ImageUri = "http://3.bp.blogspot.com/-mo_E98lebOM/UHcjgEm5vdI/AAAAAAAAAA0/zLbJOvWRa8M/s1600/Microsoft-Windows-7-wallpaper-HD+(5).jpg" },
            new MediaItem { Index = 2, ImageUri = "https://wallpapercave.com/wp/W4ab0vD.jpg" },
            new MediaItem { Index = 3, ImageUri = "http://getwallpapers.com/wallpaper/full/c/6/8/100549.jpg" }
        };

        UpdateSourceTimer.Enabled = true;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

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
    }
}
