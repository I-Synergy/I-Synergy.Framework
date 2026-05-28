using ISynergy.Framework.IO.Events;

namespace ISynergy.Framework.IO.Watchers;

/// <summary>
/// Delegate PathAvailabilityHandler
/// </summary>
/// <param name="sender">The sender.</param>
/// <param name="e">The <see cref="PathAvailablitiyEventArgs"/> instance containing the event data.</param>
public delegate void PathAvailabilityHandler(object? sender, PathAvailablitiyEventArgs e);

/// <summary>
/// Class FileSystemWatcherEx.
/// Implements the <see cref="FileSystemWatcher" />
/// </summary>
/// <seealso cref="FileSystemWatcher" />
public class FileSystemWatcherEx : FileSystemWatcher
{
    /// <summary>
    /// The maximum interval
    /// </summary>
    private readonly int _maxInterval = 60000;

    /// <summary>
    /// Occurs when [path availability event].
    /// </summary>
    public event PathAvailabilityHandler PathAvailabilityEvent = delegate { };

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; private set; } = nameof(FileSystemWatcherEx);

    /// <summary>
    /// The thread
    /// </summary>
    private Thread? _thread = null;
    /// <summary>
    /// The run
    /// </summary>
    private bool _run = false;
    /// <summary>
    /// The is network available
    /// </summary>
    private bool _isNetworkAvailable = true;
    /// <summary>
    /// The interval
    /// </summary>
    private int _interval = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherEx"/> class.
    /// </summary>
    public FileSystemWatcherEx()
        : base()
    {
        CreateThread();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherEx"/> class.
    /// </summary>
    /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
    public FileSystemWatcherEx(string path)
        : base(path)
    {
        CreateThread();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherEx"/> class.
    /// </summary>
    /// <param name="interval">The interval.</param>
    public FileSystemWatcherEx(int interval)
        : base()
    {
        _interval = interval;

        CreateThread();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherEx"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="interval">The interval.</param>
    public FileSystemWatcherEx(string path, int interval)
        : base(path)
    {
        _interval = interval;

        CreateThread();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherEx"/> class.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="name">The name.</param>
    public FileSystemWatcherEx(int interval, string name)
        : base()
    {
        _interval = interval;

        Name = name;
        CreateThread();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherEx"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="name">The name.</param>
    public FileSystemWatcherEx(string path, int interval, string name)
        : base(path)
    {
        _interval = interval;

        Name = name;
        CreateThread();
    }

    /// <summary>
    /// Creates the thread.
    /// </summary>
    private void CreateThread()
    {
        _interval = Math.Max(0, Math.Min(_interval, _maxInterval));

        if (_interval > 0)
        {
            _thread = new Thread(new ThreadStart(MonitorFolderAvailability))
            {
                Name = Name,
                IsBackground = true
            };
        }
    }

    /// <summary>
    /// Starts the folder monitor.
    /// </summary>
    public void StartFolderMonitor()
    {
        _run = true;

        if (_thread is not null)
            _thread.Start();
    }

    /// <summary>
    /// Stops the folder monitor.
    /// </summary>
    public void StopFolderMonitor() =>
        _run = false;

    /// <summary>
    /// Monitors the folder availability.
    /// </summary>
    private void MonitorFolderAvailability()
    {
        while (_run)
        {
            if (_isNetworkAvailable)
            {
                if (!Directory.Exists(Path))
                {
                    _isNetworkAvailable = false;
                    RaiseNetworkPathAvailablityEvent();
                }
            }
            else
            {
                if (Directory.Exists(Path))
                {
                    _isNetworkAvailable = true;
                    RaiseNetworkPathAvailablityEvent();
                }
            }

            Thread.Sleep(_interval);
        }
    }

    /// <summary>
    /// Raises the network path availablity event.
    /// </summary>
    private void RaiseNetworkPathAvailablityEvent() =>
        PathAvailabilityEvent(this, new PathAvailablitiyEventArgs(_isNetworkAvailable));
}
