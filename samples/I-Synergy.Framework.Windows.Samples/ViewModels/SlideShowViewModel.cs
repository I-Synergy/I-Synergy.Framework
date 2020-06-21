using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Windows.Samples.Models;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;

namespace ISynergy.Framework.Windows.Samples.ViewModels
{
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
        public ObservableCollection<MediaItem> Items
        {
            get { return GetValue<ObservableCollection<MediaItem>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Timer property value.
        /// </summary>
        public ThreadPoolTimer SlideshowTimer
        {
            get { return GetValue<ThreadPoolTimer>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UpdateSourceTimer property value.
        /// </summary>
        public ThreadPoolTimer UpdateSourceTimer
        {
            get { return GetValue<ThreadPoolTimer>(); }
            set { SetValue(value); }
        }

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
                    new MediaItem { Index = 0, ImageUri = "http://3.bp.blogspot.com/-gxIdD54Xngg/UHcjjul0xHI/AAAAAAAAAA8/CkdJsPJ9qlQ/s1600/Microsoft-Windows-7-wallpaper-HD+(6).jpg" },
                    new MediaItem { Index = 1, ImageUri = "http://3.bp.blogspot.com/-mo_E98lebOM/UHcjgEm5vdI/AAAAAAAAAA0/zLbJOvWRa8M/s1600/Microsoft-Windows-7-wallpaper-HD+(5).jpg" },
                    new MediaItem { Index = 2, ImageUri = "https://wallpapercave.com/wp/W4ab0vD.jpg" },
                    new MediaItem { Index = 3, ImageUri = "http://getwallpapers.com/wallpaper/full/c/6/8/100549.jpg" }
                };

            // Set timer if images count is at least 1.
            if (Items.Count > 0)
            {
                SlideshowTimer = ThreadPoolTimer.CreateTimer(async (e) => await SlideshowTimeOutTimerTickAsync(e), TimeSpan.FromSeconds(5));
            }
        }

        private Task UpdateSourceTimeOutTimerTickAsync(ThreadPoolTimer e)
        {
            Items = new ObservableCollection<MediaItem>()
            {
                new MediaItem { Index = 0, ImageUri = "http://3.bp.blogspot.com/-gxIdD54Xngg/UHcjjul0xHI/AAAAAAAAAA8/CkdJsPJ9qlQ/s1600/Microsoft-Windows-7-wallpaper-HD+(6).jpg" },
                new MediaItem { Index = 1, ImageUri = "http://3.bp.blogspot.com/-mo_E98lebOM/UHcjgEm5vdI/AAAAAAAAAA0/zLbJOvWRa8M/s1600/Microsoft-Windows-7-wallpaper-HD+(5).jpg" },
                new MediaItem { Index = 2, ImageUri = "https://wallpapercave.com/wp/W4ab0vD.jpg" },
                new MediaItem { Index = 3, ImageUri = "http://getwallpapers.com/wallpaper/full/c/6/8/100549.jpg" }
            };

            return Task.CompletedTask;
        }

        private async Task SlideshowTimeOutTimerTickAsync(ThreadPoolTimer timer)
        {
            timer.Cancel();

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (SelectedItem is null || SelectedItem.Index == Items.Count - 1)
                    {
                        SelectedItem = Items.First();
                    }
                    else if (SelectedItem.Index < Items.Count - 1)
                    {
                        SelectedItem = Items.Where(q => q.Index == SelectedItem.Index + 1).Single();
                    }
                });

            timer = ThreadPoolTimer.CreateTimer(async (e) => await SlideshowTimeOutTimerTickAsync(e), TimeSpan.FromSeconds(5));
        }
    }
}
