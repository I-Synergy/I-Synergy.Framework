using ISynergy.Models.Base;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Models
{
    public class NavigationItem : BaseModel
    {
        /// <summary>
        /// Gets or sets the SelectedVisibility property value.
        /// </summary>
        public Visibility SelectedVisibility
        {
            get { return GetValue<Visibility>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedForeground property value.
        /// </summary>
        public SolidColorBrush SelectedForeground
        {
            get { return GetValue<SolidColorBrush>() ?? GetStandardTextColorBrush(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsSelected property value.
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set
            {
                SetValue(value);
                SelectedVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                SelectedForeground = value
                    ? Application.Current.Resources["PrimaryMediumBrush"] as SolidColorBrush
                    : GetStandardTextColorBrush();
            }
        }

        /// <summary>
        /// Gets or sets the Label property value.
        /// </summary>
        public string Label
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Symbol property value.
        /// </summary>
        public string Symbol
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Command property value.
        /// </summary>
        public ICommand Command
        {
            get { return GetValue<ICommand>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CommandParameter property value.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ToolTipMenu property value.
        /// </summary>
        public string ToolTipMenu
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        public NavigationItem(string name, string symbol, ICommand command, object commandParameter = null)
        {
            Label = name;
            ToolTipMenu = name;
            Symbol = symbol;
            Command = command;
            CommandParameter = commandParameter;

            Services.ThemeSelectorService.OnThemeChanged += (s, e) => { if (!IsSelected) SelectedForeground = GetStandardTextColorBrush(); };
        }

        private SolidColorBrush GetStandardTextColorBrush()
        {
            var brush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;

            if (!Services.ThemeSelectorService.IsLightThemeEnabled)
            {
                brush = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as SolidColorBrush;
            }

            return brush;
        }
    }
}
