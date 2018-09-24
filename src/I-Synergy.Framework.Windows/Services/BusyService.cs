using ISynergy.Core.Classes;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class BusyService : ObservableClass, IBusyService
    {
        protected readonly ILanguageService Language;

        public BusyService(ILanguageService language)
        {
            Language = language;
        }

        /// <summary>
        /// Gets or sets the IsBusy property value.
        /// </summary>
        public bool IsBusy
        {
            get { return GetValue<bool>(); }
            set
            {
                SetValue(value);
                IsEnabled = !value;
            }
        }

        /// <summary>
        /// Gets or sets the IsEnabled property value.
        /// </summary>
        public bool IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BusyMessage property value.
        /// </summary>
        public string BusyMessage
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        public Task StartBusyAsync(string message = null)
        {
            if (message != null)
            {
                BusyMessage = message;
            }
            else
            {
                BusyMessage = Language.GetString("PleaseWait");
            }

            IsBusy = true;
            return Task.CompletedTask;
        }

        public Task EndBusyAsync()
        {
            IsBusy = false;
            return Task.CompletedTask;
        }
    }
}
