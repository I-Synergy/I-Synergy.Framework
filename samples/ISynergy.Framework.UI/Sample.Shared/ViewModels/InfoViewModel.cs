using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels
{
    /// <summary>
    /// Class InfoViewModel.
    /// Implements the <see cref="ViewModelNavigation{Object}" />
    /// </summary>
    /// <seealso cref="ViewModelNavigation{Object}" />
    public class InfoViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Info"); } }

        /// <summary>
        /// Gets or sets the CompanyName property value.
        /// </summary>
        /// <value>The name of the company.</value>
        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ProductName property value.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        /// <value>The version.</value>
        public string Version
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Copyrights property value.
        /// </summary>
        /// <value>The copyrights.</value>
        public string Copyrights
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public InfoViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            CompanyName = commonServices.InfoService.CompanyName;
            ProductName = commonServices.InfoService.ProductName;
            Version = commonServices.InfoService.ProductVersion;
            Copyrights = commonServices.InfoService.Copyrights;
        }
    }
}
