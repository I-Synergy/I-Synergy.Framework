using GalaSoft.MvvmLight.Ioc;
using ISynergy.Models.Base;
using ISynergy.Services;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Models
{
    public class NavigationItem : ModelBase
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
            get { return GetValue<SolidColorBrush>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedForeground property value.
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return GetValue<SolidColorBrush>(); }
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
                    : Foreground;
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
        
        public NavigationItem(string name, string symbol, SolidColorBrush foreground, ICommand command, object commandParameter = null)
        {
            Label = name;
            ToolTipMenu = name;
            Symbol = symbol;
            Foreground = foreground;
            Command = command;
            CommandParameter = commandParameter;
        }
    }
}
