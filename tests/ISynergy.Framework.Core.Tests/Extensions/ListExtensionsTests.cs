using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Extensions
{
    /// <summary>
    /// Class ListExtensionsTests.
    /// </summary>
    [TestClass]
    public class ListExtensionsTests
    {
        /// <summary>
        /// Defines the test method NullListNonFailableTest.
        /// </summary>
        [TestMethod]
        public void NullListNonFailableTest()
        {
            List<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Defines the test method NullListFailableTest.
        /// </summary>
        [TestMethod]
        public void NullListFailableTest()
        {
            Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
            {
                List<object> list = null;

                foreach (var item in list)
                {
                }

                return Task.CompletedTask;
            });
        }
    }
}
