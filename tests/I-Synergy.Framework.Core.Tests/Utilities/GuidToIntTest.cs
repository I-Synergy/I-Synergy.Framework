using ISynergy.Library;
using ISynergy.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ISynergy.Utilities
{
    public class GuidToIntTest
    {
        [Fact]
        [Trait(nameof(CompareUtility.CompareObject), Test.Unit)]
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
