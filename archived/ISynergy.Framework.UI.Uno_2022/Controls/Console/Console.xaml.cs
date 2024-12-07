using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Windows.Storage.Streams;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class Console. This class cannot be inherited.
/// Implements the <see cref="UserControl" />
/// Implements the <see cref="IComponentConnector" />
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IComponentConnector" />
public sealed partial class Console : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Console"/> class.
    /// </summary>
    public Console()
    {
        InitializeComponent();
    }

    /// <summary>
    /// The show diagnostics property
    /// </summary>
    private static readonly DependencyProperty ShowDiagnosticsProperty =
      DependencyProperty.Register(nameof(ShowDiagnostics), typeof(bool), typeof(Console),
      new PropertyMetadata(false, OnShowDiagnosticsChanged));

    /// <summary>
    /// Gets or sets a value indicating whether to show diagnostics.
    /// </summary>
    /// <value><c>true</c> if show diagnostics; otherwise, <c>false</c>.</value>
    public bool ShowDiagnostics
    {
        get => (bool)GetValue(ShowDiagnosticsProperty);
        set => SetValue(ShowDiagnosticsProperty, value);
    }

    /// <summary>
    /// Handles the <see cref="E:ShowDiagnosticsChanged" /> event.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnShowDiagnosticsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
    }

    /// <summary>
    /// The output stream property
    /// </summary>
    internal static readonly DependencyProperty OutputStreamProperty =
        DependencyProperty.Register(nameof(OutputStream), typeof(InMemoryRandomAccessStream), typeof(Console),
        new PropertyMetadata(default));

    /// <summary>
    /// Gets the output stream.
    /// </summary>
    /// <value>The output stream.</value>
    public InMemoryRandomAccessStream OutputStream
    {
        get => (InMemoryRandomAccessStream)GetValue(OutputStreamProperty);
        private set => SetValue(OutputStreamProperty, value);
    }

    /// <summary>
    /// The error stream property
    /// </summary>
    internal static readonly DependencyProperty ErrorStreamProperty =
        DependencyProperty.Register(nameof(ErrorStream), typeof(InMemoryRandomAccessStream), typeof(Console),
        new PropertyMetadata(default));

    /// <summary>
    /// Gets the error stream.
    /// </summary>
    /// <value>The error stream.</value>
    public InMemoryRandomAccessStream ErrorStream
    {
        get => (InMemoryRandomAccessStream)GetValue(ErrorStreamProperty);
        private set => SetValue(ErrorStreamProperty, value);
    }
}
