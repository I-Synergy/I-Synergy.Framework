using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
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

        ViewModel = viewModel;

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