using CommonServiceLocator;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Models.Base;
using ISynergy.Extensions;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Events;

namespace ISynergy.ViewModels.Base
{
    public abstract class ViewModel : BaseModel, IViewModel
    {
        public delegate Task Submit_Action(object e);

        public IContext Context { get; protected set; }
        public IBusyService Busy { get; protected set; }

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
        /// Gets or sets the NumberInfo property value.
        /// </summary>
        public NumberFormatInfo NumberFormat
        {
            get { return GetValue<NumberFormatInfo>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Culture property value.
        /// </summary>
        public CultureInfo Culture
        {
            get { return GetValue<CultureInfo>(); }
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

        /// <summary>
        /// Gets or sets the Errors property value.
        /// </summary>
        public List<string> Errors
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mode_IsAdvanced property value.
        /// </summary>
        public bool Mode_IsAdvanced
        {
            get { return ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Advanced; }
            set
            {
                ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Advanced = value;

                if (value)
                {
                    Mode_ToolTip = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Advanced");
                }
                else
                {
                    Mode_ToolTip = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Basic");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mode_ToolTip property value.
        /// </summary>
        public string Mode_ToolTip
        {
            get
            {
                string result = GetValue<string>();

                if (string.IsNullOrWhiteSpace(result))
                {
                    if (Mode_IsAdvanced)
                    {
                        result = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Advanced");
                    }
                    else
                    {
                        result = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Basic");
                    }
                }

                return result;
            }
            set { SetValue(value); }
        }

        public ViewModel(IContext context, IBusyService busy)
            : base()
        {
            base.PropertyChanged += OnPropertyChanged;
            base.ErrorsChanged += (s, e) => Errors = FlattenErrors();

            Context = context;
            Busy = busy;

            TrackView();

            Messenger.Default.Register<ExceptionHandledMessage>(this, i => Busy.EndBusyAsync());

            Culture = Thread.CurrentThread.CurrentCulture;
            Culture.NumberFormat.CurrencySymbol = $"{Context.CurrencySymbol} ";
            Culture.NumberFormat.CurrencyNegativePattern = 1;

            NumberFormat = Culture.NumberFormat;
            
            IsInitialized = false;

            Close_Command = new RelayCommand(() => Messenger.Default.Send(new OnCancellationMessage(this)));
        }

        public virtual void TrackView() => ServiceLocator.Current.GetInstance<ITelemetryService>().TrackPageView(this.GetType().Name.Replace("ViewModel", ""));

        protected List<string> FlattenErrors()
        {
            List<string> errors = new List<string>();
            Dictionary<string, List<string>> allErrors = this.GetAllErrors();

            foreach (string propertyName in allErrors.Keys)
            {
                foreach (var errorString in allErrors[propertyName].EnsureNotNull())
                {
                    errors.Add(propertyName + ": " + errorString);
                }
            }
            return errors;
        }

        protected string GetEnumDescription(Enum value)
        {
            Argument.IsNotNull(nameof(value), value);

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = ServiceLocator.Current.GetInstance<ILanguageService>().GetString(attributes[0].Description);
            }

            return description;
        }

        protected virtual void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        public bool CanClose { get; set; }
        public bool IsCancelled { get; protected set; }

        protected virtual Task CancelViewModelAsync()
        {
            IsCancelled = true;
            return Task.CompletedTask;
        }

        public virtual void OnDeactivate()
        {
        }

        protected object Parameter;

        public virtual void OnActivate(object parameter, bool isBack)
        {
            Parameter = parameter;
        }
    }
}