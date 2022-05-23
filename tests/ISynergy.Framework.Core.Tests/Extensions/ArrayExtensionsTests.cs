using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ArrayExtensionsTests.
    /// </summary>
    [TestClass]
    public class ArrayExtensionsTests
    {
        /// <summary>
        /// Defines the test method NullArrayNonFailableTest.
        /// </summary>
        [TestMethod]
        public void NullArrayNonFailableTest()
        {
            object[] list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Defines the test method NullArrayFailableTest.
        /// </summary>
        [TestMethod]
        public void NullArrayFailableTest()
        {
            Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
            {
                object[] list = null;

                foreach (var item in list)
                {
                }

                return Task.CompletedTask;
            });
        }
    }
}
