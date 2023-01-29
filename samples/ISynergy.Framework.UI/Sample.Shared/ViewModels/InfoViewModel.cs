using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
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
        public Version Version
        {
            get { return GetValue<Version>(); }
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
        /// Gets or sets the Startup property value.
        /// </summary>
        public string Startup
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public RelayCommand BusyOn_Command { get; set; }
        public RelayCommand BusyOff_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        public InfoViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            CompanyName = commonServices.InfoService.CompanyName;
            ProductName = commonServices.InfoService.ProductName;
            Version = commonServices.InfoService.ProductVersion;
            Copyrights = commonServices.InfoService.Copyrights;
            Startup = ((Context)context).Environment.ToString();
            
            BusyOn_Command = new RelayCommand(() => commonServices.BusyService.StartBusy());
            BusyOff_Command = new RelayCommand(() => commonServices.BusyService.EndBusy());

            //throw new Exception("Test exception for logging!");
        }
    }
}
