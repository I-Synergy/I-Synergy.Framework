namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class FloatExtensions.
/// </summary>
public static class FloatExtensions
{
    /// <summary>
    /// Ease out.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <returns>System.Single.</returns>
    public static float EaseOut(this float _self) => _self * _self * _self;



    /// <summary>
    /// Ease in.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <returns>System.Single.</returns>
    public static float EaseIn(this float _self) => (--_self) * _self * _self + 1;
}
