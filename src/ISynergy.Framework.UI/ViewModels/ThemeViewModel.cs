using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
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
    public class ThemeViewModel : ViewModelDialog<ApplicationColors>
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
            Color_Command = new RelayCommand<string>((e) => SelectedItem = SetColor(e));
            SelectedItem = SetColor(BaseCommonServices.ApplicationSettingsService.Color);
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="e">The e.</param>
        private ApplicationColors SetColor(string e)
        {
            if (Enum.TryParse(e, out ApplicationColors color))
            {
                return color;
            }
            else
            {
                return ApplicationColors.Default;
            }
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">if set to <c>true</c> [e].</param>
        /// <returns>Task.</returns>
        public override Task SubmitAsync(ApplicationColors e)
        {
            BaseCommonServices.ApplicationSettingsService.Color = e.ToString();
            return base.SubmitAsync(e);
        }
    }
}
