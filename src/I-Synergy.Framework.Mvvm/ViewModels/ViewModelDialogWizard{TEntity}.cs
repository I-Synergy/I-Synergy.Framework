using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm
{
    public abstract class ViewModelDialogWizard<TEntity> : ViewModelDialog<TEntity>, IViewModelDialogWizard<TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// Gets or sets the Page property value.
        /// </summary>
        public int Page
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Pages property value.
        /// </summary>
        public int Pages
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Next_IsEnabled property value.
        /// </summary>
        public bool Next_IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Back_IsEnabled property value.
        /// </summary>
        public bool Back_IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Submit_IsEnabled property value.
        /// </summary>
        public bool Submit_IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public RelayCommand Back_Command { get; }
        public RelayCommand Next_Command { get; }

        protected ViewModelDialogWizard(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory) 
            : base(context, commonServices, loggerFactory)
        {
            Back_Command = new RelayCommand(() => PerformBackAction());
            Next_Command = new RelayCommand(() => PerformNextAction());

            Page = 1;
        }

        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Page))
            {
                if (Page == 1 && Page != Pages)
                {
                    Back_IsEnabled = false;
                    Next_IsEnabled = true;
                    Submit_IsEnabled = false;
                }
                else if (Page == Pages && Page != 1)
                {
                    Back_IsEnabled = true;
                    Next_IsEnabled = false;
                    Submit_IsEnabled = true;
                }
                else if (Page == Pages && Page == 1)
                {
                    Back_IsEnabled = false;
                    Next_IsEnabled = false;
                    Submit_IsEnabled = true;
                }
                else
                {
                    Back_IsEnabled = true;
                    Next_IsEnabled = true;
                    Submit_IsEnabled = false;
                }
            }
        }

        private void PerformBackAction()
        {
            Validate();

            if (Page > 1)
            {
                Page -= 1;
            }
        }

        private void PerformNextAction()
        {
            Validate();

            if (Page < Pages)
            {
                Page += 1;
            }
        }
    }
}
