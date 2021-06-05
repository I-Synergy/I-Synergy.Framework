using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.UI.Abstractions.Services;

namespace ISynergy.Framework.UI.ViewModels
{
    /// <summary>
    /// Class ThemeViewModel.
    /// </summary>
    public class ThemeViewModel : ViewModelDialog<ThemeColors>
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
        /// The settings service.
        /// </summary>
        private readonly IBaseSettingsService _settingsService;

        /// <summary>
        /// Gets or sets the color command.
        /// </summary>
        /// <value>The color command.</value>
        public Command<string> Color_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="settingsService">The settings services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ThemeViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            IBaseSettingsService settingsService,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            _settingsService = settingsService;

            Color_Command = new Command<string>((e) => SelectedItem = SetColor(e));
            SelectedItem = _settingsService.Color.ToEnum(ThemeColors.Default);
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="e">The e.</param>
        private ThemeColors SetColor(string e)
        {
            if (Enum.TryParse(e, out ThemeColors color))
            {
                return color;
            }
            else
            {
                return ThemeColors.Default;
            }
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">if set to <c>true</c> [e].</param>
        /// <returns>Task.</returns>
        public override Task SubmitAsync(ThemeColors e)
        {
            _settingsService.Color = e.ToString();
            return base.SubmitAsync(e);
        }
    }
}
