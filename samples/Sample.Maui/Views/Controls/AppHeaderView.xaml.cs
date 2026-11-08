namespace Sample.Views.Controls;

public partial class AppHeaderView
{
    public AppHeaderView()
    {
        InitializeComponent();

        // The header padding and the menu-button nudge are physical, so they have to flip with the
        // shell's flow direction. The XAML values are the LTR defaults.
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == FlowDirectionProperty.PropertyName)
                UpdateFlowChrome();
        };

        UpdateFlowChrome();
    }

    private void UpdateFlowChrome()
    {
        var isRtl = FlowDirection == FlowDirection.RightToLeft;

        HeaderBar.Padding = isRtl ? new Thickness(16, 0, 0, 0) : new Thickness(0, 0, 16, 0);

        MenuButton.Margin = DeviceInfo.Idiom == DeviceIdiom.Phone
            ? (isRtl ? new Thickness(0, 0, -6, 0) : new Thickness(-6, 0, 0, 0))
            : new Thickness(0);
    }
}
