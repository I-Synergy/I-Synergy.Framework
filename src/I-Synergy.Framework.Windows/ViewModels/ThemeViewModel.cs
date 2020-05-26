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
    /// <summary>
    /// Class ThemeViewModel.
    /// Implements the <see cref="ViewModelDialog{bool}" />
    /// </summary>
    /// <seealso cref="ViewModelDialog{bool}" />
    public class ThemeViewModel : ViewModelDialog<bool>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Colors");
            }
        }

        /// <summary>
        /// Gets or sets the color command.
        /// </summary>
        /// <value>The color command.</value>
        public RelayCommand<string> Color_Command { get; set; }
        /// <summary>
        /// Gets or sets the background command.
        /// </summary>
        /// <value>The background command.</value>
        public RelayCommand<byte[]> Background_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
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
        /// <value>The color.</value>
        public string Color
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Wallpaper property value.
        /// </summary>
        /// <value>The wallpaper.</value>
        public byte[] Wallpaper
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="e">The e.</param>
        private void SetColor(string e)
        {
            if (Enum.TryParse(e, out ApplicationColors color))
            {
                Color = e;
            }
        }

        /// <summary>
        /// Sets the wallpaper.
        /// </summary>
        /// <param name="wallpaper">The wallpaper.</param>
        private void SetWallpaper(byte[] wallpaper)
        {
            Wallpaper = wallpaper;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">if set to <c>true</c> [e].</param>
        /// <returns>Task.</returns>
        public override Task SubmitAsync(bool e)
        {
            BaseCommonServices.ApplicationSettingsService.Color = Color;
            BaseCommonServices.ApplicationSettingsService.Wallpaper = Wallpaper;
            return base.SubmitAsync(e);
        }
    }
}
