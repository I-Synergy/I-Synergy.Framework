namespace ISynergy.Framework.Automations.Actions;

/// <summary>
/// Result from action.
/// </summary>
public class ActionResult
{
    /// <summary>
    /// Gets property if action is executed successfully.
    /// </summary>
    public bool Succeeded { get; private set; }
    /// <summary>
    /// Get placeholder for additional data as result.
    /// </summary>
    public object? Result { get; private set; }

    /// <summary>
    /// Default constructor.
    /// Result is false and result is null.
    /// </summary>
    public ActionResult()
        : this(false) { }

    /// <summary>
    /// Default constructor for setting result and if action is succeeded.
    /// </summary>
    /// <param name="succeeded"></param>
    /// <param name="result"></param>
    public ActionResult(bool succeeded, object? result = null)
    {
        Succeeded = succeeded;
        Result = result;
    }
}
