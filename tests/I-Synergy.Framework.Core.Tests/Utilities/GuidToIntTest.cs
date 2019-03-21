using ISynergy.Framework.Tests.Base;
using System;
using Xunit;

namespace ISynergy.Utilities
{
    public class GuidToIntTest : UnitTest
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
