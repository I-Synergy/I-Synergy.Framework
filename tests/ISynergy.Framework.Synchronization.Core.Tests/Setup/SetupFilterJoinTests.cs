using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Synchronization.Core.Tests.Setup
{
    [TestClass]
    public class SetupFilterJoinTests
    {
        [TestMethod]
        public void SetupFilterJoin_Compare_TwoSetupFilterJoin_ShouldBe_Equals()
        {
            var filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);
            var filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);

            Assert.AreEqual(filterJoin1, filterJoin2);
            Assert.IsTrue(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, "t1", null, null, null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, "t1", null, null, null, null);

            Assert.AreEqual(filterJoin1, filterJoin2);
            Assert.IsTrue(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, "t1", null, null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, "t1", null, null, null);

            Assert.AreEqual(filterJoin1, filterJoin2);
            Assert.IsTrue(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, "t1", null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, "t1", null, null);

            Assert.AreEqual(filterJoin1, filterJoin2);
            Assert.IsTrue(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, "t1", null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, "t1", null);

            Assert.AreEqual(filterJoin1, filterJoin2);
            Assert.IsTrue(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, "t1");
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, null, "t1");

            Assert.AreEqual(filterJoin1, filterJoin2);
            Assert.IsTrue(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);
        }

        [TestMethod]
        public void SetupFilterJoin_Compare_TwoSetupFilterJoin_ShouldBe_Different()
        {
            var filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);
            var filterJoin2 = new SetupFilterJoin(Join.Left, null, null, null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, "t1", null, null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, "t1", null, null, null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, "t2", null, null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, "t1", null, null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, "t2", null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, "t2", null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, "t1", null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, "t2", null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, "t1", null, null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, "t1", null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, "t2", null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, "t1", null);
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, "t1");
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, null, "t2");

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);

            filterJoin1 = new SetupFilterJoin(Join.Inner, null, null, null, null, "t1");
            filterJoin2 = new SetupFilterJoin(Join.Inner, null, null, null, null, null);

            Assert.AreNotEqual(filterJoin1, filterJoin2);
            Assert.IsFalse(filterJoin1.Equals(filterJoin2));
            Assert.IsFalse(filterJoin1 == filterJoin2);
        }

    }
}
