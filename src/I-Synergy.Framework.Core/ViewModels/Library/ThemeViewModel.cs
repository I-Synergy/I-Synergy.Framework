using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public class ThemeViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return BaseService.LanguageService.GetString("Generic_Colors");
            }
        }

        public RelayCommand<string> Color_Command { get; set; }
        public RelayCommand<byte[]> Background_Command { get; set; }

        public ThemeViewModel(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            Color_Command = new RelayCommand<string>(async (e) => await SetColorAsync(e));
            Background_Command = new RelayCommand<byte[]>(async (e) => await SetWallpaperAsync(e));
        }

        private Task SetColorAsync(string color)
        {
            BaseService.BaseSettingsService.Application_Color = color;
            Messenger.Default.Send(new OnSubmittanceMessage(this, null));
            return Task.CompletedTask;
        }

        private Task SetWallpaperAsync(byte[] wallpaper)
        {
            BaseService.BaseSettingsService.Application_Wallpaper = wallpaper;
            Messenger.Default.Send(new OnSubmittanceMessage(this, null));
            return Task.CompletedTask;
        }

        public override Task SubmitAsync(object e)
        {
            Messenger.Default.Send(new OnCancellationMessage(this));
            return Task.CompletedTask;
        }
    }
}