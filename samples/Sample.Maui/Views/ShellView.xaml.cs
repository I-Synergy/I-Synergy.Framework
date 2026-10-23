using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class ShellView : Shell, IShellView
{
    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel ViewModel
    {
        get
        {
            if (BindingContext is IViewModel viewModel)
                return viewModel;

            // Return null instead of throwing — BindingContext can be null after
            // CleanupPage clears it during modal navigation. As a Singleton, this
            // ShellView is reused, and callers handle null gracefully.
            return null!;
        }
        set
        {
            BindingContext = value;
            SetBinding(Shell.TitleProperty, new Binding(nameof(value.Title)));
        }
    }

    public ShellView(ShellViewModel viewModel)
    {
        InitializeComponent();

        // Subscribe to LocalizedStrings changes for UI text binding updates
        var localizedStrings = LocalizedStrings.Default;
        UpdateShellContentTitles();
        localizedStrings.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
                UpdateShellContentTitles();
        };

        ViewModel = viewModel;

        // The framework never raises BaseShellViewModel.ShellLoadedAsync for this shell, so the
        // saved theme and the first-run flow are started here, once the visual tree is available.
        Loaded += OnShellLoaded;

#if MACCATALYST
        var fileMenu = new MenuBarItem { Text = "File" };
        fileMenu.Add(new MenuFlyoutItem { Text = "Settings", Command = viewModel.SettingsCommand });
        fileMenu.Add(new MenuFlyoutSeparator());
        fileMenu.Add(new MenuFlyoutItem { Text = "Sign Out", Command = viewModel.SignInCommand });
        MenuBarItems.Add(fileMenu);

        var viewMenu = new MenuBarItem { Text = "View" };
        viewMenu.Add(new MenuFlyoutItem { Text = "Language", Command = viewModel.LanguageCommand });
        viewMenu.Add(new MenuFlyoutItem { Text = "Theme", Command = viewModel.ThemeCommand });
        MenuBarItems.Add(viewMenu);

        var helpMenu = new MenuBarItem { Text = "Help" };
        helpMenu.Add(new MenuFlyoutItem { Text = "Help", Command = viewModel.HelpCommand });
        helpMenu.Add(new MenuFlyoutItem { Text = "Feedback", Command = viewModel.FeedbackCommand });
        MenuBarItems.Add(helpMenu);
#endif
    }

    private async void OnShellLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnShellLoaded;

        if (BindingContext is ShellViewModel viewModel)
            await viewModel.ShellLoadedAsync();
    }

    private static readonly string[] TitleKeys = ["Dashboard", "CustomControls", "MauiControls", "SlideShow"];

    private void UpdateShellContentTitles()
    {
        var localizedStrings = LocalizedStrings.Default;

        // Each page is an implicit ShellItem > ShellSection > ShellContent chain; the flyout
        // reads the title at any of the three levels, so keep them in step.
        for (var i = 0; i < Items.Count && i < TitleKeys.Length; i++)
        {
            var title = localizedStrings[TitleKeys[i]];
            var item = Items[i];
            item.Title = title;

            foreach (var section in item.Items)
            {
                section.Title = title;

                foreach (var content in section.Items)
                    content.Title = title;
            }
        }

        if (this.FindByName<NavigationItem>("HelpItem") is NavigationItem helpItem)
            helpItem.Text = localizedStrings["Help"];
    }

    #region IDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            ViewModel?.Dispose();
        }

        // free native resources if there are any.
    }
    #endregion
}