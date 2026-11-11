using ISynergy.Framework.Mathematics.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Abstractions;

[TestClass]
public class WrapperTests
{
    [TestMethod]
    public void Matrix_WrapsWithoutCopying()
    {
        var source = new double[,] { { 1, 2 }, { 3, 4 } };
        var matrix = new Matrix<double>(source);

        Assert.AreEqual(2, matrix.Rows);
        Assert.AreEqual(2, matrix.Columns);
        Assert.AreEqual(4, matrix.Length);
        Assert.AreEqual(3.0, matrix[1, 0]);

        matrix[0, 1] = 7;
        Assert.AreEqual(7.0, source[0, 1]);
        Assert.AreSame(source, matrix.Array);
    }

    [TestMethod]
    public void Jagged_WrapsWithoutCopying()
    {
        var source = new[] { new[] { 1, 2 }, new[] { 3 } };
        var jagged = new Jagged<int>(source);

        Assert.AreEqual(2, jagged.Rows);
        Assert.AreEqual(2, jagged.Columns(0));
        Assert.AreEqual(1, jagged.Columns(1));
        Assert.AreEqual(3, jagged[1][0]);

        jagged[0][0] = 9;
        Assert.AreEqual(9, source[0][0]);
    }

    [TestMethod]
    public void Vector_WrapsWithoutCopying()
    {
        var source = new[] { 1.0, 2.0, 3.0 };
        var vector = new Vector<double>(source);

        Assert.AreEqual(3, vector.Length);
        Assert.AreEqual(2.0, vector[1]);

        vector[1] = 5;
        Assert.AreEqual(5.0, source[1]);
        Assert.HasCount(3, vector.AsSpan());

        double[] unwrapped = vector;
        Assert.AreSame(source, unwrapped);
    }

    [TestMethod]
    public void Tensor3_WrapsWithoutCopying()
    {
        var source = new double[2, 3, 4];
        var tensor = new Tensor3<double>(source);

        Assert.AreEqual(2, tensor.GetLength(0));
        Assert.AreEqual(3, tensor.GetLength(1));
        Assert.AreEqual(4, tensor.GetLength(2));

        tensor[1, 2, 3] = 42;
        Assert.AreEqual(42.0, source[1, 2, 3]);
    }

    [TestMethod]
    public void Wrappers_RejectNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Vector<int>(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Matrix<int>(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Jagged<int>(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Tensor3<int>(null));
    }
}
