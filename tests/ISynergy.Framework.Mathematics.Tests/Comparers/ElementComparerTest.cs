using ISynergy.Framework.Mathematics.Comparers;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Comparers;
[TestClass]
public class ElementComparerTest
{

    [TestMethod]
    public void ElementComparerConstructorTest()
    {
        double[][] values =
        [ //                 v
            [0, 3, 0],
            [0, 4, 1],
            [-1, 1, 1],
            [-1, 5, 4],
            [-2, 2, 6]
        ];

        // Sort the array considering only the second column
        Array.Sort(values, new ElementComparer() { Index = 1 });

        double[][] expected =
        [
            [-1, 1, 1],
            [-2, 2, 6],
            [0, 3, 0],
            [0, 4, 1],
            [-1, 5, 4]
        ];

        Assert.IsTrue(values.IsEqual(expected));
    }
}
