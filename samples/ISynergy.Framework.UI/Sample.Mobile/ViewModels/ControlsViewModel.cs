using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels
{
    public class ControlsViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Controls"); } }


        /// <summary>
        /// Gets or sets the DecimalValue property value.
        /// </summary>
        public decimal DecimalValue
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        public ControlsViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            DecimalValue = 2910.1975m;
        }
    }
}
