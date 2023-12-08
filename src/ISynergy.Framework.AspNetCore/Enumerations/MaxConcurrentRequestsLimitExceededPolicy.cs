namespace ISynergy.Framework.AspNetCore.Enumerations;

/// <summary>
/// Enum MaxConcurrentRequestsLimitExceededPolicy
/// </summary>
public enum MaxConcurrentRequestsLimitExceededPolicy
{
    /// <summary>
    /// The drop
    /// </summary>
    Drop = 0,
    /// <summary>
    /// The fifo queue drop tail
    /// </summary>
    FifoQueueDropTail = 1,
    /// <summary>
    /// The fifo queue drop head
    /// </summary>
    FifoQueueDropHead = 2
}
