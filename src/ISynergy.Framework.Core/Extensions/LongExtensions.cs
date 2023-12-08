namespace ISynergy.Framework.Core.Extensions;

public static class LongExtensions
{
    /// <summary>
    /// Converts long to ulong.
    /// </summary>
    /// <param name="_self">The value.</param>
    /// <returns>ulong</returns>
    public static ulong ToUlong(this long _self) =>
        unchecked((ulong)(_self - long.MinValue));

    /// <summary>
    /// Converts ulong to long.
    /// </summary>
    /// <param name="_self"></param>
    /// <returns>long</returns>
    public static long ToLong(this ulong _self) =>
        unchecked((long)_self + long.MinValue);
}
