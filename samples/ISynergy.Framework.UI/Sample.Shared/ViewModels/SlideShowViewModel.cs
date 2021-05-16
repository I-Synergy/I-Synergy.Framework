using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Sample.Models;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Sample.ViewModels
{
    /// <summary>
    /// Class SlideShowViewModel.
    /// </summary>
    public class SlideShowViewModel : ViewModelNavigation<MediaItem>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Display"); } }

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
        /// Gets or sets the SelectedIndex property value.
        /// </summary>
        public int SelectedIndex
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Timer property value.
        /// </summary>
        /// <value>The slideshow timer.</value>
        public ThreadPoolTimer SlideshowTimer
        {
            get { return GetValue<ThreadPoolTimer>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UpdateSourceTimer property value.
        /// </summary>
        /// <value>The update source timer.</value>
        public ThreadPoolTimer UpdateSourceTimer
        {
            get { return GetValue<ThreadPoolTimer>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideShowViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SlideShowViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            UpdateSourceTimer = ThreadPoolTimer.CreateTimer(async (e) => await UpdateSourceTimeOutTimerTickAsync(e), TimeSpan.FromMinutes(30));

            // Get initial images from source.
            Items = new ObservableCollection<MediaItem>()
                {
                    new MediaItem { ImageUri = "http://3.bp.blogspot.com/-gxIdD54Xngg/UHcjjul0xHI/AAAAAAAAAA8/CkdJsPJ9qlQ/s1600/Microsoft-Windows-7-wallpaper-HD+(6).jpg" },
                    new MediaItem { ImageUri = "http://3.bp.blogspot.com/-mo_E98lebOM/UHcjgEm5vdI/AAAAAAAAAA0/zLbJOvWRa8M/s1600/Microsoft-Windows-7-wallpaper-HD+(5).jpg" },
                    new MediaItem { ImageUri = "https://wallpapercave.com/wp/W4ab0vD.jpg" },
                    new MediaItem { ImageUri = "http://getwallpapers.com/wallpaper/full/c/6/8/100549.jpg" }
                };

            // Set timer if images count is at least 1.
            if (Items.Count > 0)
            {
                SlideshowTimer = ThreadPoolTimer.CreateTimer(async (e) => await SlideshowTimeOutTimerTickAsync(e), TimeSpan.FromSeconds(5));
            }
        }

        /// <summary>
        /// Updates the source time out timer tick asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        private Task UpdateSourceTimeOutTimerTickAsync(ThreadPoolTimer e)
        {
            Items = new ObservableCollection<MediaItem>()
            {
                new MediaItem { ImageUri = "http://3.bp.blogspot.com/-gxIdD54Xngg/UHcjjul0xHI/AAAAAAAAAA8/CkdJsPJ9qlQ/s1600/Microsoft-Windows-7-wallpaper-HD+(6).jpg" },
                new MediaItem { ImageUri = "http://3.bp.blogspot.com/-mo_E98lebOM/UHcjgEm5vdI/AAAAAAAAAA0/zLbJOvWRa8M/s1600/Microsoft-Windows-7-wallpaper-HD+(5).jpg" },
                new MediaItem { ImageUri = "https://wallpapercave.com/wp/W4ab0vD.jpg" },
                new MediaItem { ImageUri = "http://getwallpapers.com/wallpaper/full/c/6/8/100549.jpg" }
            };

            return Task.CompletedTask;
        }

        /// <summary>
        /// slideshow time out timer tick as an asynchronous operation.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SlideshowTimeOutTimerTickAsync(ThreadPoolTimer timer)
        {
            timer.Cancel();

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (SelectedIndex == Items.Count - 1)
                    {
                        SelectedIndex = 0;
                    }
                    else if (SelectedIndex < Items.Count - 1)
                    {
                        SelectedIndex += 1;
                    }
                });

            timer = ThreadPoolTimer.CreateTimer(async (e) => await SlideshowTimeOutTimerTickAsync(e), TimeSpan.FromSeconds(5));
        }
    }
}
