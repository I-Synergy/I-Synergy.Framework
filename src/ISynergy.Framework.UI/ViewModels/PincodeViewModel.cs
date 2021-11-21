using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.ViewModels
{
    /// <summary>
    /// Class PincodeViewModel.
    /// </summary>
    public class PincodeViewModel : ViewModelDialog<bool>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title => BaseCommonServices.LanguageService.GetString("Pin");

        /// <summary>
        /// Gets or sets the Language property value.
        /// </summary>
        /// <value>The pincode.</value>
        public string Pincode
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Property property value.
        /// </summary>
        /// <value>The property.</value>
        public object Property
        {
            get => GetValue<object>();
            private set => SetValue(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PincodeViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="property">The property.</param>
        public PincodeViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger,
            object property)
            : base(context, commonServices, logger)
        {
            Property = property;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">if set to <c>true</c> [e].</param>
        /// <returns>Task.</returns>
        public override Task SubmitAsync(bool e)
        {
            //ToDo: Change pincode.
            if (Pincode.Equals("0000"))
            {
                e = true;
            }
            else
            {
                e = false;
            }

            return base.SubmitAsync(e);
        }
    }
}
