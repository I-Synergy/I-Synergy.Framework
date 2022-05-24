using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels
{
    public class InfoViewModel : ViewModelNavigation<InfoModel>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Info"); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        public InfoViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger<InfoViewModel> logger)
            : base(context, commonServices, logger)
        {
            SelectedItem = new InfoModel()
            {
                CompanyName = commonServices.InfoService.CompanyName,
                ProductName = commonServices.InfoService.ProductName,
                Version = commonServices.InfoService.ProductVersion,
                Copyrights = commonServices.InfoService.Copyrights
            };
        }
    }
}
