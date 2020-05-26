using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.Windows.Navigation
{
    public abstract class NavigationBase { }

    public class NavigationItem : NavigationBase
    {
        public Visibility SelectedVisibility { get; set; }
        public SolidColorBrush SelectedForeground { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public string ToolTipMenu { get; set; }

        public NavigationItem(string name, string symbol, SolidColorBrush foreground, ICommand command, object commandParameter = null)
        {
            Name = name;
            ToolTipMenu = name;
            Symbol = symbol;
            Foreground = foreground;
            Command = command;
            CommandParameter = commandParameter;
        }
    }

    public class Separator : NavigationBase { }

    public class Header : NavigationBase
    {
        public string Name { get; set; }
    }
}
