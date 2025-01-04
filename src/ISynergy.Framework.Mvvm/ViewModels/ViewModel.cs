using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModel.
/// Implements the <see cref="ObservableClass" />
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="ObservableClass" />
/// <seealso cref="IViewModel" />
[Bindable(true)]
public abstract class ViewModel : ObservableClass, IViewModel
{
    protected readonly IContext _context;
    protected readonly IBaseCommonServices _commonServices;
    protected readonly ILogger _logger;

    public IBaseCommonServices CommonServices => _commonServices;

    /// <summary>
    /// Occurs when [cancelled].
    /// </summary>
    public event EventHandler Cancelled;
    /// <summary>
    /// Occurs when [closed].
    /// </summary>
    public event EventHandler Closed;

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
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="automaticValidation">The validation.</param>
    protected ViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(automaticValidation)
    {
        _context = context;
        _commonServices = commonServices;
        _logger = logger;

        PropertyChanged += OnPropertyChanged;
        IsInitialized = false;

        CloseCommand = new AsyncRelayCommand(CloseAsync);
        CancelCommand = new AsyncRelayCommand(CancelAsync);

        _logger.LogTrace(GetType().Name);
    }

    /// <summary>
    /// Initializes the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public virtual Task InitializeAsync()
    {
        if (!IsInitialized)
            _logger.LogTrace("{0} initialized.", GetType().Name);

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
        var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

        if (attributes is not null && attributes.Length > 0)
        {
            description = LanguageService.Default.GetString(attributes[0].Description);
        }

        return description;
    }

    /// <summary>
    /// Handles the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
    public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
    public virtual void Cleanup()
    {
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
            PropertyChanged -= OnPropertyChanged;

            // Clear commands
            if (CloseCommand is IDisposable closeCommand)
            {
                closeCommand.Dispose();
                CloseCommand = null;
            }

            if (CancelCommand is IDisposable cancelCommand)
            {
                cancelCommand.Dispose();
                CancelCommand = null;
            }

            base.Dispose(disposing);
        }
    }
}
