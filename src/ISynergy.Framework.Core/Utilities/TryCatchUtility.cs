namespace ISynergy.Framework.Core.Utilities;

public static class TryCatchUtility
{
    /// <summary>
    /// Runs an operation and ignores any Exceptions that occur.
    /// Returns true or falls depending on whether catch was
    /// triggered
    /// </summary>
    /// <param name="operation">lambda that performs an operation that might throw</param>
    /// <returns></returns>
    public static bool IgnoreAllErrors(Action operation)
    {
        if (operation == null)
            return false;

        try
        {
            operation.Invoke();
        }
        catch
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Runs an function that returns a value and ignores any Exceptions that occur.
    /// Returns true or falls depending on whether catch was
    /// triggered
    /// </summary>
    /// <param name="operation">parameterless lamda that returns a value of T</param>
    /// <param name="defaultValue">Default value returned if operation fails</param>
    public static T? IgnoreAllErrors<T>(Func<T> operation, T? defaultValue = default(T))
    {
        if (operation == null)
            return defaultValue;

        T? result;

        try
        {
            result = operation.Invoke();
        }
        catch
        {
            result = defaultValue;
        }

        return result;
    }

    /// <summary>
    /// Runs an operation and ignores any Exceptions that occur.
    /// Returns true or falls depending on whether catch was
    /// triggered
    /// </summary>
    /// <param name="operation">lambda that performs an operation that might throw</param>
    /// <returns></returns>
    public static bool IgnoreErrors<TException>(Action operation)
        where TException : Exception
    {
        if (operation == null)
            return false;

        try
        {
            operation.Invoke();
        }
        catch (TException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Runs an function that returns a value and ignores any Exceptions that occur.
    /// Returns true or falls depending on whether catch was
    /// triggered
    /// </summary>
    /// <param name="operation">parameterless lamda that returns a value of T</param>
    /// <param name="defaultValue">Default value returned if operation fails</param>
    public static T? IgnoreErrors<T, TException>(Func<T> operation, T? defaultValue = default(T))
        where TException : Exception
    {
        if (operation == null)
            return defaultValue;

        T? result;

        try
        {
            result = operation.Invoke();
        }
        catch (TException)
        {
            result = defaultValue;
        }

        return result;
    }
}
