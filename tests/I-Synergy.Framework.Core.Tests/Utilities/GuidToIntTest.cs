using Xunit;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities
{
    public class GuidToIntTest
    {
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
