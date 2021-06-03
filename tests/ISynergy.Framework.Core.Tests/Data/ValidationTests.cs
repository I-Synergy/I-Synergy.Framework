using System;
using System.Linq;
using ISynergy.Framework.Core.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Data.Tests
{
    /// <summary>
    /// Class ValidationTests.
    /// </summary>
    [TestClass]
    public class ValidationTests
    {
        /// <summary>
        /// Defines the test method ValidateIsNullTest.
        /// </summary>
        [TestMethod]
        public void ValidateIsNullTest()
        {
            var model = new ModelFixture<string>
            {
                Value = null
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (string.IsNullOrEmpty(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Value is null");
                }
            });

            Assert.IsFalse(model.Validate());
        }

        /// <summary>
        /// Defines the test method ValidateIsNotNullTest.
        /// </summary>
        [TestMethod]
        public void ValidateIsNotNullTest()
        {
            var model = new ModelFixture<string>
            {
                Value = "192.168.1.0"
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (string.IsNullOrEmpty(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Value is null");
                }
            });

            Assert.IsTrue(model.Validate());
        }

        /// <summary>
        /// Defines the test method ValidateIsNotInRangeTest.
        /// </summary>
        [TestMethod]
        public void ValidateIsNotInRangeTest()
        {
            var model = new ModelFixture<int>
            {
                Value = 9999
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<int>;

                if (!Enumerable.Range(1, 100).Contains(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Portnumber is not in range [1,100]");
                }
            });

            Assert.IsFalse(model.Validate());
        }

        /// <summary>
        /// Defines the test method ValidateIsInRangeTest.
        /// </summary>
        [TestMethod]
        public void ValidateIsInRangeTest()
        {
            var model = new ModelFixture<int>
            {
                Value = 80
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<int>;

                if (!Enumerable.Range(1, 500).Contains(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Portnumber is not in range [1,100]");
                }
            });

            Assert.IsTrue(model.Validate());
        }

        /// <summary>
        /// Defines the test method ValidateStringLengthIsNotInRangeTest.
        /// </summary>
        [TestMethod]
        public void ValidateStringLengthIsNotInRangeTest()
        {
            var model = new ModelFixture<string>
            {
                Value = ""
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (!Enumerable.Range(1, 35).Contains(i.Value.Length))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("String should have length of 1-35 characters");
                }
            });

            Assert.IsFalse(model.Validate());
        }

        /// <summary>
        /// Defines the test method ValidateStringLengthIsInRangeTest.
        /// </summary>
        [TestMethod]
        public void ValidateStringLengthIsInRangeTest()
        {
            var model = new ModelFixture<string>
            {
                Value = "192.168.1.0"
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (!Enumerable.Range(1, 35).Contains(i.Value.Length))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("String should have length of 1-35 characters");
                }
            });

            Assert.IsTrue(model.Validate());
        }
    }
}
