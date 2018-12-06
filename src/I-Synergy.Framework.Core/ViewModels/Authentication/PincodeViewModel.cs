using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public class PincodeViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return BaseService.LanguageService.GetString("Generic_Pin");
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
        /// Gets or sets the Result property value.
        /// </summary>
        public bool Result
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
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
            IBaseService baseService,
            object property)
            : base(context, baseService)
        {
            Property = property;
        }

        public override Task SubmitAsync(object e)
        {
            if (Pincode.Equals(BaseService.BaseSettingsService.GetSetting("PointOfSales_Pincode", "0000")))
            {
                Result = true;
            }
            else
            {
                Result = false;
            }

            Messenger.Default.Send(new OnSubmittanceMessage(this, Result));

            return Task.CompletedTask;
        }
    }
}