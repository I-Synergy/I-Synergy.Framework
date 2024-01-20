using ISynergy.Framework.Mathematics.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Core;

[TestClass]
public class ToolsTest
{
    [DataTestMethod]
    [DataRow(0, false)]
    [DataRow(1, true)]
    [DataRow(2, true)]
    [DataRow(3, false)]
    [DataRow(4, true)]
    [DataRow(8, true)]
    [DataRow(0x80, true)]
    [DataRow(0x81, false)]
    [DataRow(0x8000, true)]
    [DataRow(0x8001, false)]
    [DataRow(0x40000000, true)]
    [DataRow(0x3FFFFFFF, false)]
    [DataRow(-1, false)]
    [DataRow(-8, false)]
    [DataRow(int.MinValue, false)]
    public void IsPowerOf2Test(int valueToTest, bool expectedResult)
    {
        Assert.AreEqual(expectedResult, Tools.IsPowerOf2(valueToTest));
    }
}
