using ISynergy.Framework.Core.Models;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfWindow = System.Windows.Window;

namespace Sample.Views;

/// <summary>
/// Interaction logic for ShellView.xaml
/// </summary>
public partial class ShellView : View, IShellView
{
    private bool _isReady;
    private bool _isNormalMaximized;
    private Rect _restoreBounds;

    /// <summary>
    /// Default constructor to initialize the view
    /// </summary>
    public ShellView()
    {
        InitializeComponent();
        Loaded += (_, _) => _isReady = true;
        Dispatcher.BeginInvoke(new Action(NavigateInitial), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void NavigateInitial()
    {
        _isReady = true;

        // Show the initial page (the first navigation entry) once the shell is loaded.
        var selected = NavigationList.SelectedItem as NavigationItem;

        if (selected is not null &&
            selected.Command is not null &&
            selected.Command.CanExecute(selected.CommandParameter))
        {
            selected.Command.Execute(selected.CommandParameter);
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleMaximize();
            return;
        }

        // Let the caption buttons handle their own clicks.
        if (IsButtonSource(e.OriginalSource))
            return;

        if (WpfWindow.GetWindow(this) is { } window)
            window.DragMove();
    }

    private static bool IsButtonSource(object source)
    {
        var current = source as DependencyObject;

        while (current is not null)
        {
            if (current is Button)
                return true;

            current = VisualTreeHelper.GetParent(current);
        }

        return false;
    }

    private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
    {
        if (WpfWindow.GetWindow(this) is { } window)
            window.WindowState = WindowState.Minimized;
    }

    private void MaximizeWindow_Click(object sender, RoutedEventArgs e) => ToggleMaximize();

    private void CloseWindow_Click(object sender, RoutedEventArgs e) => WpfWindow.GetWindow(this)?.Close();

    private void ToggleMaximize()
    {
        if (WpfWindow.GetWindow(this) is not { } window)
            return;

        if (_isNormalMaximized)
        {
            _isNormalMaximized = false;
            window.Tag = null;
            window.Left = _restoreBounds.Left;
            window.Top = _restoreBounds.Top;
            window.Width = _restoreBounds.Width;
            window.Height = _restoreBounds.Height;
            if (System.Windows.Shell.WindowChrome.GetWindowChrome(window) is { } chrome)
                chrome.CornerRadius = new CornerRadius(8);
            MaximizeButton.Content = "";
            return;
        }

        _restoreBounds = new Rect(window.Left, window.Top, window.Width, window.Height);
        var workArea = SystemParameters.WorkArea;

        _isNormalMaximized = true;
        window.Tag = "NormalMaximized";
        window.WindowState = WindowState.Normal;
        window.Left = workArea.Left;
        window.Top = workArea.Top;
        window.Width = workArea.Width;
        window.Height = workArea.Height;
        if (System.Windows.Shell.WindowChrome.GetWindowChrome(window) is { } maximizedChrome)
            maximizedChrome.CornerRadius = new CornerRadius(0);
        MaximizeButton.Content = "";
    }

    private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Ignore the initial selection that is applied while the view is being built;
        // executing navigation before the shell is the MainWindow content fails.
        if (!_isReady)
            return;

        if (sender is ListBox { SelectedItem: NavigationItem navigationItem } &&
            navigationItem.Command is not null &&
            navigationItem.Command.CanExecute(navigationItem.CommandParameter))
        {
            navigationItem.Command.Execute(navigationItem.CommandParameter);
        }
    }
}
