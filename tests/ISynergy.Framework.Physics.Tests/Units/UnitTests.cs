using ISynergy.Framework.Physics.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Physics.Tests.Units
{
    /// <summary>
    /// Test the unit.
    /// </summary>
    [TestClass()]
    public class UnitTests
    {
        /// <summary>
        /// If Unit is created with empty parameters it should fail.
        /// </summary>
        [TestMethod()]
        public void UnitEmptyContructorTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Unit(Enumerations.Units.second, Array.Empty<UnitTypes>(), null, null));
        }

        /// <summary>
        /// If Unit is created with nulled parameters it should fail.
        /// </summary>
        [TestMethod()]
        public void UnitNulledConstructorTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Unit(Enumerations.Units.second, Array.Empty<UnitTypes>(), null, null));
        }

        /// <summary>
        /// If Unit is created successfully, then Unit should not be null.
        /// </summary>
        [TestMethod()]
        public void UnitValidConstructorTest()
        {
            var unit = new Unit(Enumerations.Units.minute, new UnitTypes[] { UnitTypes.Time }, e => e * 100, e => e / 100);
            Assert.IsNotNull(unit);
        }
    }
}