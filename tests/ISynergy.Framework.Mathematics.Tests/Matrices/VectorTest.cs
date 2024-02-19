
using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests;
[TestClass]
public partial class VectorTest
{

    [TestMethod]
    public void ExpandTest()
    {
        int[] v = Vector.Zeros<int>(5);
        double[] u = Vector.Create(5, 1.0);
        int[] w = Vector.Create(1, 2, 3);

        Assert.IsTrue(v.IsEqual(new[] { 0, 0, 0, 0, 0 }));
        Assert.IsTrue(u.IsEqual(new[] { 1, 1, 1, 1, 1 }));
        Assert.IsTrue(w.IsEqual(new[] { 1, 2, 3 }));
    }

    [TestMethod]
    public void GetIndicesTest()
    {
        double[] v = Vector.Ones(5);
        int[] idx = v.GetIndices();
        Assert.IsTrue(idx.IsEqual(new[] { 0, 1, 2, 3, 4 }));
    }

    [TestMethod]
    public void sample_test()
    {
        int[] r = Vector.Range(10000);
        int[] s = Vector.Sample(r, 10);
        Assert.IsTrue(s.Any(x => x > 10));
    }

}
