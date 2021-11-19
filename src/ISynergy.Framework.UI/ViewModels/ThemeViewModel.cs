using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ISynergy.Framework.UI.ViewModels
{
    /// <summary>
    /// Class ThemeViewModel.
    /// </summary>
    public class ThemeViewModel : ViewModelDialog<Style>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title => BaseCommonServices.LanguageService.GetString("Colors");

        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly IThemeService _themeService;

        /// <summary>
        /// Gets or sets the Items property value.
        /// </summary>
        public ThemeColors ThemeColors
        {
            get => GetValue<ThemeColors>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Themes property value.
        /// </summary>
        public Themes Themes
        {
            get => GetValue<Themes>();
            set => SetValue(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="themeService">The settings services.</param>
        /// <param name="logger">The logger factory.</param>
        public ThemeViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            IThemeService themeService,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            _themeService = themeService;

            ThemeColors = new ThemeColors();

            SelectedItem.Color = _themeService.Style.Color;
            SelectedItem.Theme = _themeService.Style.Theme;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">if set to <c>true</c> [e].</param>
        /// <returns>Task.</returns>
        public override Task SubmitAsync(Style e)
        {
            _themeService.SetStyle(e);
            return base.SubmitAsync(e);
        }
    }
}
