using System.Threading.Tasks;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class BusyService.
    /// Implements the <see cref="ObservableClass" />
    /// Implements the <see cref="IBusyService" />
    /// </summary>
    /// <seealso cref="ObservableClass" />
    /// <seealso cref="IBusyService" />
    public class BusyService : ObservableClass, IBusyService
    {
        /// <summary>
        /// The language service
        /// </summary>
        protected readonly ILanguageService LanguageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyService"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        public BusyService(ILanguageService language)
        {
            LanguageService = language;
        }

        /// <summary>
        /// Gets or sets the IsBusy property value.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
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
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BusyMessage property value.
        /// </summary>
        /// <value>The busy message.</value>
        public string BusyMessage
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Starts the busy asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public Task StartBusyAsync(string message = null)
        {
            if (message != null)
            {
                BusyMessage = message;
            }
            else
            {
                BusyMessage = LanguageService.GetString("PleaseWait");
            }

            IsBusy = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts the busy asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task StartBusyAsync()
        {
            return this.StartBusyAsync(LanguageService.GetString("PleaseWait"));
        }

        /// <summary>
        /// Ends the busy asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task EndBusyAsync()
        {
            IsBusy = false;
            return Task.CompletedTask;
        }
    }
}
