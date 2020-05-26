using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Windows.ViewModels
{
    public class ThemeViewModel : ViewModelDialog<bool>
    {
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Colors");
            }
        }

        public RelayCommand<string> Color_Command { get; set; }
        public RelayCommand<byte[]> Background_Command { get; set; }

        public ThemeViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            Color_Command = new RelayCommand<string>((e) => SetColor(e));
            Background_Command = new RelayCommand<byte[]>((e) => SetWallpaper(e));

            Color = BaseCommonServices.ApplicationSettingsService.Color;
            Wallpaper = BaseCommonServices.ApplicationSettingsService.Wallpaper;
        }

        /// <summary>
        /// Gets or sets the Color property value.
        /// </summary>
        public string Color
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Wallpaper property value.
        /// </summary>
        public byte[] Wallpaper
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }

        private void SetColor(string e)
        {
            if (Enum.TryParse(e, out ApplicationColors color))
            {
                Color = e;
            }
        }

        private void SetWallpaper(byte[] wallpaper)
        {
            Wallpaper = wallpaper;
        }

        public override Task SubmitAsync(bool e)
        {
            BaseCommonServices.ApplicationSettingsService.Color = Color;
            BaseCommonServices.ApplicationSettingsService.Wallpaper = Wallpaper;
            return base.SubmitAsync(e);
        }
    }
}
