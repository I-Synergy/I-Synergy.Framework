using System.ComponentModel;
using ISynergy.Framework.Core.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Data.Tests
{
    /// <summary>
    /// Class ModelFieldTests.
    /// </summary>
    [TestClass]
    public class ModelFieldTests
    {
        /// <summary>
        /// Defines the test method ViewModelFieldNotifyPropertyChangedNotifiesOnNewValueTest.
        /// </summary>
        [TestMethod]
        public void ViewModelFieldNotifyPropertyChangedNotifiesOnNewValueTest()
        {
            var originalValue = "original value";

            var instance = new ModelFixture<string>(originalValue);

            Assert.IsInstanceOfType(instance, typeof(INotifyPropertyChanged));

            var gotEvent = false;

            ((INotifyPropertyChanged)instance).PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                gotEvent = true;
                Assert.IsTrue(e.PropertyName.Equals("Value") | e.PropertyName.Equals("IsValid"), "PropertyName was wrong.");
            };

            var newValue = "new value";
            instance.Value = newValue;

            Assert.IsTrue(newValue.Equals(instance.Value), "Value didn't change.");
            Assert.IsTrue(gotEvent, "Didn't get the PropertyChanged event.");
        }

        /// <summary>
        /// Defines the test method ViewModelFieldNotifyPropertyChangedDoesNotNotifyOnSameValueTest.
        /// </summary>
        [TestMethod]
        public void ViewModelFieldNotifyPropertyChangedDoesNotNotifyOnSameValueTest()
        {
            var originalValue = "original value";

            var instance = new ModelFixture<string>(originalValue);

            Assert.IsInstanceOfType(instance, typeof(INotifyPropertyChanged));

            var gotEvent = false;

            ((INotifyPropertyChanged)instance).PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                gotEvent = true;
                Assert.IsFalse(gotEvent, "Should not get any PropertyChanged events.");
            };

            instance.Value = originalValue;

            Assert.IsFalse(gotEvent, "Should not have gotten the PropertyChanged event.");
        }

        /// <summary>
        /// Defines the test method ViewModelFieldConstructorEmptyTest.
        /// </summary>
        [TestMethod]
        public void ViewModelFieldConstructorEmptyTest()
        {
            var instance = new ModelFixture<string>();
            Assert.IsTrue(instance.Value is null, "Value should be null.");
        }

        /// <summary>
        /// Defines the test method ViewModelFieldConstructorWithValueTest.
        /// </summary>
        [TestMethod]
        public void ViewModelFieldConstructorWithValueTest()
        {
            var expectedValue = "expected value";

            var instance = new ModelFixture<string>(expectedValue);

            Assert.IsTrue(expectedValue.Equals(instance.Value), "Value was wrong.");
        }

        /// <summary>
        /// Defines the test method ViewModelFieldToStringReturnsEmptyStringWhenValueIsNullTest.
        /// </summary>
        [TestMethod]
        public void ViewModelFieldToStringReturnsEmptyStringWhenValueIsNullTest()
        {
            var instance = new ModelFixture<string>();

            Assert.IsTrue(instance.Value is null, "Value should be null.");
            Assert.IsTrue(instance.ToString() == string.Empty, "ToString() should return empty.");
        }

        /// <summary>
        /// Defines the test method ViewModelFieldToStringTest.
        /// </summary>
        [TestMethod]
        public void ViewModelFieldToStringTest()
        {
            var expectedValue = "expected value";

            var instance = new ModelFixture<string>(expectedValue);

            Assert.IsTrue(expectedValue.Equals(instance.Value), "Value was wrong.");
            Assert.IsTrue(expectedValue.Equals(instance.ToString()), "ToString() was wrong.");
        }
    }
}
