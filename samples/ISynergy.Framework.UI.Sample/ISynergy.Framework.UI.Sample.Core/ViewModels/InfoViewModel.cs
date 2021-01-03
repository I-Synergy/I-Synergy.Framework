using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Sample.ViewModels
{
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
        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ProductName property value.
        /// </summary>
        public string ProductName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        public string Version
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Copyrights property value.
        /// </summary>
        public string Copyrights
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


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
