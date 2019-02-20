using GalaSoft.MvvmLight.Command;
using ISynergy.Services;
using System.ComponentModel;

namespace ISynergy.Mvvm
{
    public abstract class ViewModelBladeWizard<TEntity> : ViewModelBlade<TEntity>
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

        public RelayCommand Back_Command { get; private set; }
        public RelayCommand Next_Command { get; private set; }

        protected ViewModelBladeWizard(
            IContext context,
            IBaseService baseService)
            : base(context, baseService) 
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
            if (Page > 1)
            {
                Page = Page - 1;
            }
        }

        private void PerformNextAction()
        {
            if (Page < Pages)
            {
                Page = Page + 1;
            }
        }
    }
}