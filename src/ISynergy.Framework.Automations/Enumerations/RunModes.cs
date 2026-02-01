namespace ISynergy.Framework.Automations.Enumerations;

/// <summary>
/// Run modes.
/// </summary>
public enum RunModes
{
    /// <summary>
    /// Runs single instance once.
    /// </summary>
    Single,
    /// <summary>
    /// Restarts after it finished task.
    /// </summary>
    Restart,
    /// <summary>
    /// Is put on a FIFO queue.
    /// </summary>
    Queued,
    /// <summary>
    /// Runs parallel.
    /// </summary>
    Parallel
}
