namespace Sample.Views.Controls;

public partial class AppHeaderView
{
    public AppHeaderView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Aligns the header's leading edge with the sidebar's icon column and drops the right-hand
    /// caption-button padding. When the header is a page row, the native caption buttons overlay
    /// it and need the right padding; when the window title bar hosts the header, the title bar
    /// already reserves that space.
    /// </summary>
    public void ApplyTitleBarPadding()
    {
        // Sidebar menu icons: 16 (panel padding) + 8 (row padding) = 24, 36 wide → centre 42.
        // A 44-wide header button centred on 42 starts at 20; the title bar itself insets its
        // content by ~18px, so the header only adds the remainder.
        HeaderBar.Padding = DeviceInfo.Platform == DevicePlatform.WinUI
            ? new Thickness(2, 0, 0, 0)
            : new Thickness(80, 0, 0, 0);
    }
}
