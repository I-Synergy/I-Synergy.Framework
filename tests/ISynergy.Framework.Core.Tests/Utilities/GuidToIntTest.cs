using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    /// <summary>
    /// Class GuidToIntTest.
    /// </summary>
    [TestClass]
    public class GuidToIntTest
    {
        /// <summary>
        /// Defines the test method ConvertTest.
        /// </summary>
        [TestMethod]
        public void ConvertTest()
        {
            int number = 1975;
            var EnryptedGuid = number.ToGuid();
            var result = EnryptedGuid.ToInt();

            Assert.AreEqual(number, result);
        }
    }
}
