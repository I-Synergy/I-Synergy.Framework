using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Ranges.Tests
{
    [TestClass]
    public class RangeTest
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
        public void IsOverlappingTest(float min1, float max1, float min2, float max2, bool expectedResult)
        {
            NumericRange range1 = new NumericRange(min1, max1);
            NumericRange range2 = new NumericRange(min2, max2);

            Assert.AreEqual(expectedResult, range1.IsOverlapping(range2));
        }

        [DataTestMethod]
        [DataRow(0.4f, 7.3f, 1, 7, true)]
        [DataRow(-6.6f, -0.1f, -6, -1, true)]
        [DataRow(0.4f, 7.3f, 0, 8, false)]
        [DataRow(-6.6f, -0.1f, -7, 0, false)]
        public void ToRangeTest(float fMin, float fMax, int iMin, int iMax, bool innerRange)
        {
            NumericRange range = new NumericRange(fMin, fMax);
            NumericRange iRange = range.ToRange(innerRange);

            Assert.AreEqual(iMin, iRange.Min);
            Assert.AreEqual(iMax, iRange.Max);
        }

        [DataTestMethod]
        [DataRow(1.1f, 2.2f, 1.1f, 2.2f, true)]
        [DataRow(-2.2f, -1.1f, -2.2f, -1.1f, true)]
        [DataRow(1.1f, 2.2f, 2.2f, 3.3f, false)]
        [DataRow(1.1f, 2.2f, 1.1f, 4.4f, false)]
        [DataRow(1.1f, 2.2f, 3.3f, 4.4f, false)]
        public void EqualityOperatorTest(float min1, float max1, float min2, float max2, bool areEqual)
        {
            NumericRange range1 = new NumericRange(min1, max1);
            NumericRange range2 = new NumericRange(min2, max2);

            Assert.AreEqual(range1.Equals(range2), areEqual);
            Assert.AreEqual(range1 == range2, areEqual);
            Assert.AreEqual(range1 != range2, !areEqual);
        }
    }
}
