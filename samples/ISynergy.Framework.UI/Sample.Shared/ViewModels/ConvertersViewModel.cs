using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Sample.ViewModels
{
    public class ConvertersViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Converters"); } }

        public ConvertersViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            SelectedSoftwareEnvironment = (int)SoftwareEnvironments.Production;
        }

        /// <summary>
        /// Gets or sets the SoftwareEnvironments property value.
        /// </summary>
        public SoftwareEnvironments SoftwareEnvironments
        {
            get { return GetValue<SoftwareEnvironments>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedSoftwareEnvironment property value.
        /// </summary>
        public int SelectedSoftwareEnvironment
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

    }
}
