using ISynergy.Framework.Mvvm.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.ComponentModel;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Validation;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.Mvvm
{
    public abstract class ViewModel : ObservableClass, IViewModel
    {
        public event EventHandler Cancelled;
        public event EventHandler Closed;

        protected virtual void OnCancelled(EventArgs e) => Cancelled?.Invoke(this, e);
        protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);

        public IContext Context { get; }
        public IBaseCommonServices BaseCommonServices { get; }
        public ILogger Logger { get; }

        protected readonly ILoggerFactory _loggerFactory;

        public RelayCommand Close_Command { get; protected set; }

        /// <summary>
        /// Gets or sets the Title property value.
        /// </summary>
        public virtual string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsInitialized property value.
        /// </summary>
        public bool IsInitialized
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        
        protected ViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            ValidationTriggers validation = ValidationTriggers.Manual)
            : base(validation)
        {
            _loggerFactory = loggerFactory;

            Context = context;
            BaseCommonServices = commonServices;
            Logger = loggerFactory.CreateLogger(GetType());

            PropertyChanged += OnPropertyChanged;
            IsInitialized = false;

            Close_Command = new RelayCommand(async () => await CloseAsync());
        }

        public virtual Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                Logger.LogTrace($"ViewModel: {GetType().Name.Replace("ViewModel", "")}.");
            }

            return Task.CompletedTask;
        }

        public string GetEnumDescription(Enum value)
        {
            Argument.IsNotNull(nameof(value), value);

            var description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = BaseCommonServices.LanguageService.GetString(attributes[0].Description);
            }

            return description;
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public bool CanClose { get; set; }
        public bool IsCancelled { get; protected set; }

        public virtual void Cleanup()
        {
            PropertyChanged -= OnPropertyChanged;
            ServiceLocator.Default.Unregister(this);
        }

        public virtual Task CancelAsync()
        {
            IsCancelled = true;
            OnCancelled(EventArgs.Empty);
            return CloseAsync();
        }

        public virtual Task CloseAsync()
        {
            OnClosed(EventArgs.Empty);
            return Task.CompletedTask;
        }

        public virtual Task OnDeactivateAsync()
        {
            Cleanup();
            return Task.CompletedTask;
        }

        public virtual Task OnActivateAsync(object parameter, bool isBack) => Task.CompletedTask;

        protected override void Dispose(bool disposing)
        {
            Cleanup();
            base.Dispose(disposing);
        }
    }
}
