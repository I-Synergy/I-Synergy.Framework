using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests;
public partial class VectorTest
{
    private static readonly double[] expected = new double[] { 2.0, 2.25, 2.5, 2.75, 3.0 };
    private static readonly double[] expectedArray = new double[] { 2.0, 2.2, 2.4, 2.6, 2.8 };

    [TestMethod]
    public void linspace_comparison_tests()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        double[] vector = Vector.Interval(-12d, 12d, 1d);
#pragma warning restore CS0618 // Type or member is obsolete

        Assert.AreSequenceEqual(
            new double[] { -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, vector);

        // https://docs.scipy.org/doc/numpy/reference/generated/numpy.linspace.html
        Assert.AreSequenceEqual(expected, Vector.Interval(2.0, 3.0, steps: 5));
        Assert.AreSequenceEqual(expectedArray, Vector.Interval(2.0, 3.0, steps: 5, includeLast: false));


        Assert.AreSequenceEqual(new double[] { 1 }, Vector.Interval(1, 3, 1));
        Assert.AreSequenceEqual(new double[] { 3 }, Vector.Interval(3, 1, 1));
        Assert.AreSequenceEqual(new double[] { 1, 1.5, 2, 2.5, 3 }, Vector.Interval(1, 3, 5));
        Assert.AreSequenceEqual(new double[] { 1, 1.5, 2, 2.5, 3 }, Vector.Interval(1.0, 3.0, 5));
        Assert.AreSequenceEqual(new double[] { 3 }, Vector.Interval(3, 1, 1));

        Assert.Throws<ArgumentOutOfRangeException>(() => Vector.Interval(1, 3, -1));
    }

}
