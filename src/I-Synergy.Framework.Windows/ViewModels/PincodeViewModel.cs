using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Windows.ViewModels
{
    public class PincodeViewModel : ViewModelDialog<bool>
    {
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Pin");
            }
        }

        /// <summary>
        /// Gets or sets the Language property value.
        /// </summary>
        public string Pincode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Property property value.
        /// </summary>
        public object Property
        {
            get { return GetValue<object>(); }
            private set { SetValue(value); }
        }

        public PincodeViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            object property)
            : base(context, commonServices, loggerFactory)
        {
            Property = property;
        }

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
