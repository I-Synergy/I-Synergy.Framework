using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.UI.ViewModels;

[Lifetime(Lifetimes.Singleton)]
public class SplashScreenViewModel : ObservableClass, IViewModel
{
    private readonly ICommonServices _commonServices;
    private readonly ILogger _logger;

    private SplashScreenOptions? _splashScreenOptions;
    private TaskCompletionSource<bool>? _taskCompletion;
    private Action? _onLoadingComplete;
    private DispatcherQueue? _dispatcherQueue;
    private Func<DispatcherQueue, Task>[]? _tasks;

    public ICommonServices CommonServices => _commonServices;
    public SplashScreenOptions Configuration => _splashScreenOptions ?? new SplashScreenOptions() { SplashScreenType = SplashScreenTypes.None };

    /// <summary>
    /// Occurs when [cancelled].
    /// </summary>
    public event EventHandler? Cancelled;
    /// <summary>
    /// Occurs when [closed].
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Gets or sets the close command.
    /// </summary>
    /// <value>The close command.</value>
    public AsyncRelayCommand CloseCommand { get; private set; }

    /// <summary>
    /// /// Gets or sets the cancel command.
    /// </summary>
    public AsyncRelayCommand CancelCommand { get; private set; }

    /// <summary>
    /// Gets or sets the Title property value.
    /// </summary>
    /// <value>The title.</value>
    public string Title
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
    /// Gets or sets the CanSkip property value.
    /// </summary>
    public bool CanSkip
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the CurrentTaskDescription property value.
    /// </summary>
    public string CurrentTaskDescription
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public SplashScreenViewModel(
        ICommonServices commonServices,
        bool automaticValidation = false)
        : base(automaticValidation)
    {
        _commonServices = commonServices;
        _logger = _commonServices.LoggerFactory.CreateLogger<SplashScreenViewModel>();

        _taskCompletion = new TaskCompletionSource<bool>();

        IsInitialized = false;

        CloseCommand = new AsyncRelayCommand(CloseAsync);
        CancelCommand = new AsyncRelayCommand(CancelAsync);

        _logger.LogTrace(GetType().Name);
    }

    public void Initialize(DispatcherQueue dispatcherQueue, Action onLoadingComplete, SplashScreenOptions splashScreenOptions, params Func<DispatcherQueue, Task>[] tasks)
    {
        _dispatcherQueue = dispatcherQueue;
        _onLoadingComplete = onLoadingComplete;
        _splashScreenOptions = splashScreenOptions;
        _tasks = tasks;

        if (_splashScreenOptions is null)
        {
            CanSkip = false;
        }
        else
        {
            CanSkip = _splashScreenOptions.SplashScreenType switch
            {
                Enumerations.SplashScreenTypes.Video => true,
                _ => false
            };
        }
    }

    public async Task StartInitializationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (_tasks?.Length > 0)
            {
                for (int i = 0; i < _tasks.Length; i++)
                {
                    if (IsCancelled)
                        break;

                    CurrentTaskDescription = $"Executing task {i + 1} of {_tasks.Length}";
                    await _tasks[i](_dispatcherQueue!);
                }

                IsInitialized = true;

                _taskCompletion!.SetResult(true);

                // Automatically complete loading if there are no loading options
                if ((_splashScreenOptions is null || _splashScreenOptions.SplashScreenType == SplashScreenTypes.None) && !CanSkip)
                {
                    await CompleteLoadingAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initialization failed");
            _taskCompletion!.SetException(ex);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    public Task CompleteLoadingAsync()
    {
        if (IsInitialized)
        {
            _onLoadingComplete?.Invoke();
            return CloseAsync();
        }

        return Task.CompletedTask;
    }

    public Task WaitForCompletionAsync() => _taskCompletion!.Task;

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

        if (fieldInfo is null)
            return description;

        var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

        if (attributes is not null && attributes.Length > 0)
        {
            description = LanguageService.Default.GetString(attributes[0].Description!);
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
        _taskCompletion!.TrySetCanceled();
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

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            PropertyChanged -= OnPropertyChanged;

            _taskCompletion = null;
            _onLoadingComplete = null;
            _dispatcherQueue = null;

            CloseCommand?.Dispose();
            CancelCommand?.Dispose();
        }
    }
}
