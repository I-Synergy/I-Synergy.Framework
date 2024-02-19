namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class DecimalExtensions.
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// Determines whether the specified self is negative.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns><c>true</c> if the specified self is negative; otherwise, <c>false</c>.</returns>
    public static bool IsNegative(this decimal self)
    {
        if (self < 0)
        {
            return true;
        }

        return false;
    }
}
