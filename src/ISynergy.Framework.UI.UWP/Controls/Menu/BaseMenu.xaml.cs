using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;


namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class BaseMenu. This class cannot be inherited.
/// Implements the <see cref="CommandBar" />
/// Implements the <see cref="IComponentConnector" />
/// </summary>
/// <seealso cref="CommandBar" />
/// <seealso cref="IComponentConnector" />
public sealed partial class BaseMenu : CommandBar
{
    /// <summary>
    /// The refresh enabled property
    /// </summary>
    private static readonly DependencyProperty Refresh_EnabledProperty =
        DependencyProperty.Register(nameof(Refresh_Enabled), typeof(Visibility), typeof(BaseMenu), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Gets or sets the refresh enabled.
    /// </summary>
    /// <value>The refresh enabled.</value>
    public Visibility Refresh_Enabled
    {
        get => (Visibility)GetValue(Refresh_EnabledProperty);
        set => SetValue(Refresh_EnabledProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMenu"/> class.
    /// </summary>
    public BaseMenu()
    {
        InitializeComponent();
    }
}
