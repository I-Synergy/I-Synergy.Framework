using Xunit;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    /// <summary>
    /// Class GuidToIntTest.
    /// </summary>
    public class GuidToIntTest
    {
        /// <summary>
        /// Defines the test method ConvertTest.
        /// </summary>
        [Fact]
        public void ConvertTest()
        {
            int number = 1975;
            var EnryptedGuid = number.ToGuid();
            var result = EnryptedGuid.ToInt();

            Assert.Equal(number, result);
        }
    }
}
