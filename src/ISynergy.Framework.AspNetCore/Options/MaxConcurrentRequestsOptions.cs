using ISynergy.Framework.AspNetCore.Enumerations;

namespace ISynergy.Framework.AspNetCore.Options;

/// <summary>
/// Class MaxConcurrentRequestsOptions.
/// </summary>
public class MaxConcurrentRequestsOptions
{
    /// <summary>
    /// The concurrent requests unlimited
    /// </summary>
    public const int ConcurrentRequestsUnlimited = -1;
    /// <summary>
    /// The maximum time in queue unlimited
    /// </summary>
    public const int MaxTimeInQueueUnlimited = -1;

    /// <summary>
    /// The limit
    /// </summary>
    private int _limit;
    /// <summary>
    /// The maximum queue length
    /// </summary>
    private int _maxQueueLength;
    /// <summary>
    /// The maximum time in queue
    /// </summary>
    private int _maxTimeInQueue;

    /// <summary>
    /// Gets or sets the limit.
    /// </summary>
    /// <value>The limit.</value>
    public int Limit
    {
        get { return _limit; }

        set { _limit = (value < ConcurrentRequestsUnlimited) ? ConcurrentRequestsUnlimited : value; }
    }

    /// <summary>
    /// Gets or sets the limit exceeded policy.
    /// </summary>
    /// <value>The limit exceeded policy.</value>
    public MaxConcurrentRequestsLimitExceededPolicy LimitExceededPolicy { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the queue.
    /// </summary>
    /// <value>The maximum length of the queue.</value>
    public int MaxQueueLength
    {
        get { return _maxQueueLength; }

        set { _maxQueueLength = (value < 0) ? 0 : value; }
    }

    /// <summary>
    /// Gets or sets the maximum time in queue.
    /// </summary>
    /// <value>The maximum time in queue.</value>
    public int MaxTimeInQueue
    {
        get { return _maxTimeInQueue; }

        set { _maxTimeInQueue = (value <= 0) ? MaxTimeInQueueUnlimited : value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxConcurrentRequestsOptions"/> class.
    /// </summary>
    public MaxConcurrentRequestsOptions()
    {
        _limit = ConcurrentRequestsUnlimited;
        LimitExceededPolicy = MaxConcurrentRequestsLimitExceededPolicy.Drop;
        _maxQueueLength = 0;
        _maxTimeInQueue = MaxTimeInQueueUnlimited;
    }
}
