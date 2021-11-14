using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Utilities
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
