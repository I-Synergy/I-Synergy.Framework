using System;
using Xunit;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities
{
    public class GuidToIntTest
    {
        [Fact]
        public void ConvertTest()
        {
            uint number = 1975;

            var NewG = Guid.NewGuid();
            var EnryptedGuid = number.ToGuid();
            var result = EnryptedGuid.ToUInt();

            Assert.Equal(number, result);
        }
    }
}
