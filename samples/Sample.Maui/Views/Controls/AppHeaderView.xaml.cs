namespace Sample.Views.Controls;

public partial class AppHeaderView
{
    public AppHeaderView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Hides the shell controls so the in-page header only shows the breadcrumb. Used when the
    /// Windows title bar already hosts the menu, back, theme and settings controls.
    /// </summary>
    public void ShowBreadcrumbOnly()
    {
        ShellButtons.IsVisible = false;
        TrailingButtons.IsVisible = false;
    }
}
