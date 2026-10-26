using Microsoft.Maui.Controls.Shapes;

namespace Sample.Views.Controls;

public partial class SidebarView
{
    private readonly List<Border> _hoverOverlays = [];
    private bool _interactionsAttached;

    public SidebarView()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    /// <summary>
    /// Sidebar rows are <see cref="Border"/>s with a tap gesture, so MAUI gives them no hover
    /// feedback. A rounded overlay behind each row's content is shown while the pointer is over the
    /// row; because it is a separate layer, the row's selected (accent) background is unaffected.
    /// </summary>
    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_interactionsAttached)
            return;

        _interactionsAttached = true;

        foreach (var row in NavRows.Children.Concat(FooterRows.Children).OfType<Border>())
        {
            if (row.Content is not Grid grid)
                continue;

            var overlay = new Border
            {
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                BackgroundColor = ResolveHoverColor(),
                IsVisible = false,
                InputTransparent = true
            };

            Grid.SetColumnSpan(overlay, Math.Max(1, grid.ColumnDefinitions.Count));
            Grid.SetRowSpan(overlay, Math.Max(1, grid.RowDefinitions.Count));
            grid.Children.Insert(0, overlay);
            _hoverOverlays.Add(overlay);

            var pointer = new PointerGestureRecognizer();
            pointer.PointerEntered += (_, _) => overlay.IsVisible = true;
            pointer.PointerExited += (_, _) => overlay.IsVisible = false;
            row.GestureRecognizers.Add(pointer);
        }

        if (Application.Current is { } application)
            application.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e) =>
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var color = ResolveHoverColor();

            foreach (var overlay in _hoverOverlays)
                overlay.BackgroundColor = color;
        });

    private static Color ResolveHoverColor()
    {
        var dark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var key = dark ? "AppSurface2Dark" : "AppSurface2Light";

        if (Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color)
            return color;

        return dark ? Color.FromArgb("#1C2A25") : Color.FromArgb("#E8EDE9");
    }
}
