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

            Guid NewG = Guid.NewGuid();
            Guid EnryptedGuid = number.ToGuid();
            uint result = EnryptedGuid.ToUInt();

            Assert.Equal(number, result);
        }
    }
}
