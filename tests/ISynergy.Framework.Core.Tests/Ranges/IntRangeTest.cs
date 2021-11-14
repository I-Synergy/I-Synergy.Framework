using ISynergy.Framework.Core.Ranges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Ranges
{
    [TestClass]
    public class IntRangeTest
    {
        [DataTestMethod]
        [DataRow(0, 1, 1, 2, true)]
        [DataRow(0, 1, 2, 3, false)]
        [DataRow(0, 10, 2, 4, true)]
        [DataRow(0, 10, 5, 15, true)]
        [DataRow(0, 10, -5, 5, true)]
        [DataRow(2, 4, 0, 10, true)]
        [DataRow(5, 15, 0, 10, true)]
        [DataRow(-5, 5, 0, 10, true)]
        public void IsOverlappingTest(int min1, int max1, int min2, int max2, bool expectedResult)
        {
            NumericRange range1 = new NumericRange(min1, max1);
            NumericRange range2 = new NumericRange(min2, max2);

            Assert.AreEqual(expectedResult, range1.IsOverlapping(range2));
        }

        [DataTestMethod]
        [DataRow(0, 1, 0, 1)]
        [DataRow(-1, 0, -1, 0)]
        public void ToRangeTest(int iMin, int iMax, float fMin, float fMax)
        {
            NumericRange iRange = new NumericRange(iMin, iMax);
            NumericRange range = iRange;

            Assert.AreEqual(fMin, range.Min);
            Assert.AreEqual(fMax, range.Max);
        }

        [DataTestMethod]
        [DataRow(1, 2, 1, 2, true)]
        [DataRow(-2, -1, -2, -1, true)]
        [DataRow(1, 2, 2, 3, false)]
        [DataRow(1, 2, 1, 4, false)]
        [DataRow(1, 2, 3, 4, false)]
        public void EqualityOperatorTest(int min1, int max1, int min2, int max2, bool areEqual)
        {
            NumericRange range1 = new NumericRange(min1, max1);
            NumericRange range2 = new NumericRange(min2, max2);

            Assert.AreEqual(range1.Equals(range2), areEqual);
            Assert.AreEqual(range1 == range2, areEqual);
            Assert.AreEqual(range1 != range2, !areEqual);
        }
    }
}
