namespace ISynergy.Framework.Mathematics;

/// <summary>
///   Exact floating-point comparisons that keep the semantics of the <c>==</c> and <c>!=</c>
///   operators without comparing floating-point values for equality with those operators.
///   A value compares equal to itself unless it is <see cref="double.NaN"/>; positive and
///   negative zero compare equal, exactly as with the operators.
/// </summary>
internal static class ExactComparisons
{
    /// <summary>Determines whether the two values are exactly equal (NaN is not equal to anything).</summary>
    public static bool AreEqual(double a, double b) =>
        double.Equals(a, b) && !double.IsNaN(a);

    /// <summary>Determines whether the two values are exactly equal (NaN is not equal to anything).</summary>
    public static bool AreEqual(float a, float b) =>
        float.Equals(a, b) && !float.IsNaN(a);

    /// <summary>Determines whether the two values are not exactly equal (NaN is not equal to anything).</summary>
    public static bool AreNotEqual(double a, double b) =>
        !double.Equals(a, b) || double.IsNaN(a);

    /// <summary>Determines whether the two values are not exactly equal (NaN is not equal to anything).</summary>
    public static bool AreNotEqual(float a, float b) =>
        !float.Equals(a, b) || float.IsNaN(a);
}
