using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Interaction logic for DurationSpinner.xaml
/// </summary>
public partial class DurationSpinner : UserControl
{
    /// <summary>
    /// Interval in minutes used to increase or decrease the value.
    /// </summary>
    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register(nameof(Interval), typeof(int), typeof(DurationSpinner), new PropertyMetadata(5));

    /// <summary>
    /// Current TimeSpan value.
    /// </summary>
    public TimeSpan Value
    {
        get { return (TimeSpan)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(TimeSpan), typeof(DurationSpinner), new PropertyMetadata(TimeSpan.FromMinutes(5)));

    /// <summary>
    /// Regex Mask value.
    /// </summary>
    public string Mask
    {
        get { return (string)GetValue(MaskProperty); }
        private set { SetValue(MaskProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MaskProperty =
        DependencyProperty.Register(nameof(Mask), typeof(string), typeof(DurationSpinner), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DurationSpinner()
    {
        InitializeComponent();
        Mask = "((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)";
    }

    private void RepeatButtonDecrease(object sender, RoutedEventArgs e)
    {
        Value = Value.Subtract(TimeSpan.FromMinutes(Interval));
    }

    private void RepeatButtonIncrease(object sender, RoutedEventArgs e)
    {
        Value = Value.Add(TimeSpan.FromMinutes(Interval));
    }
}
