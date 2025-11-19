using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModel.
/// Implements the <see cref="ObservableValidatedClass" />
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="ObservableValidatedClass" />
/// <seealso cref="IViewModel" />
[Bindable(true)]
public abstract class ViewModel : ObservableValidatedClass, IViewModel
{
    protected readonly ICommonServices _commonServices;
    protected readonly ILogger _logger;

    public ICommonServices CommonServices => _commonServices;

    /// <summary>
    /// Occurs when [cancelled].
    /// </summary>
    public event EventHandler? Cancelled;
    /// <summary>
    /// Occurs when [closed].
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Handles the <see cref="E:Cancelled" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnCancelled(EventArgs e) => Cancelled?.Invoke(this, e);
    /// <summary>
    /// Handles the <see cref="E:Closed" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);

    /// <summary>
    /// Gets or sets the close command.
    /// </summary>
    /// <value>The close command.</value>
    public AsyncRelayCommand CloseCommand { get; protected set; }

    /// <summary>
    /// /// Gets or sets the cancel command.
    /// </summary>
    public AsyncRelayCommand CancelCommand { get; protected set; }

    /// <summary>
    /// Gets or sets the Title property value.
    /// </summary>
    /// <value>The title.</value>
    public virtual string Title
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsInitialized property value.
    /// </summary>
    /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
    public bool IsInitialized
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsRefreshing property value.
    /// </summary>
    public bool IsRefreshing
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Parameter property value.
    /// </summary>
    public object Parameter
    {
        get => GetValue<object>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModel(
        ICommonServices commonServices,
        ILogger<ViewModel> logger)
    {
        _commonServices = commonServices;
        _commonServices.ScopedContextService.ScopedChanged += ScopedContextService_ScopedChanged;

        _logger = logger;

        PropertyChanged += OnPropertyChanged;
        IsInitialized = false;

        CloseCommand = new AsyncRelayCommand(CloseAsync);
        CancelCommand = new AsyncRelayCommand(CancelAsync);

        _logger.LogTrace(GetType().Name);
    }

    private async void ScopedContextService_ScopedChanged(object? sender, Core.Events.ReturnEventArgs<bool> e)
    {
        if (e.Value)
        {
            IsInitialized = false;

            // Clear existing data
            Cleanup();

            // Reinitialize with new scope if needed
            if (!IsDisposed)
            {
                await InitializeAsync();
            }
        }
    }

    /// <summary>
    /// Initializes the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public virtual Task InitializeAsync()
    {
        if (!IsInitialized)
            _logger.LogTrace($"{GetType().Name} initialized.");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the enum description.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string GetEnumDescription(Enum value)
    {
        Argument.IsNotNull(value);

        var description = value.ToString();
        var fieldInfo = value.GetType().GetField(description);

        if (fieldInfo is null)
            return description;

        var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

        if (attributes is not null && attributes.Length > 0 && attributes[0].Description is not null)
        {
            description = _commonServices.LanguageService.GetString(attributes[0].Description!);
        }

        return description;
    }

    /// <summary>
    /// Handles the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
    public virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can close.
    /// </summary>
    /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
    public bool CanClose { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is cancelled.
    /// </summary>
    /// <value><c>true</c> if this instance is cancelled; otherwise, <c>false</c>.</value>
    public bool IsCancelled { get; protected set; }

    /// <summary>
    /// Cleans up the instance, for example by saving its state,
    /// removing resources, etc...
    /// </summary>
    public virtual void Cleanup(bool isClosing = true)
    {
        if (isClosing)
        {
            // Full cleanup for closing
            // Release all resources
        }
        else
        {
            // Partial cleanup for navigation
            ReleaseBackgroundResources();
        }
    }

    /// <summary>
    /// Releases heavy resources that aren't needed when the ViewModel is in the background.
    /// Called when the ViewModel is pushed to the backstack.
    /// </summary>
    protected virtual void ReleaseBackgroundResources()
    {
        // Base implementation is empty - derived classes should override
        // Examples of resources to release:
        // - Large collections that can be reloaded
        // - Image caches
        // - Background workers
    }

    /// <summary>
    /// Cancels the synchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public virtual Task CancelAsync()
    {
        IsCancelled = true;
        OnCancelled(EventArgs.Empty);
        return CloseAsync();
    }

    /// <summary>
    /// Closes the synchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public virtual Task CloseAsync()
    {
        Cleanup();
        OnClosed(EventArgs.Empty);
        return Task.CompletedTask;
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _commonServices.ScopedContextService.ScopedChanged -= ScopedContextService_ScopedChanged;

            PropertyChanged -= OnPropertyChanged;

            // Clear event handlers to prevent memory leaks
            Cancelled = null;
            Closed = null;

            // Clear commands
            CloseCommand?.Dispose();
            CancelCommand?.Dispose();

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Called when navigating away from this ViewModel.
    /// Cancel any running commands if needed.
    /// </summary>
    public virtual void OnNavigatedFrom() => CancelRunningCommands();

    /// <summary>
    /// Called when navigating to this ViewModel.
    /// Reset command states.
    /// </summary>
    public virtual void OnNavigatedTo() => ResetCommandStates();

    /// <summary>
    /// Cancels all running commands in this ViewModel.
    /// </summary>
    protected virtual void CancelRunningCommands() => this.CancelAllCommands();

    /// <summary>
    /// Resets the state of commands when returning to this ViewModel.
    /// </summary>
    protected virtual void ResetCommandStates() => this.ResetAllCommandStates();
}
