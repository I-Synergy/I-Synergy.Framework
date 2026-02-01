using ISynergy.Framework.Core.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Data.Tests;

/// <summary>
/// Class ModelTests.
/// </summary>
[TestClass]
public class ModelTests
{
    /// <summary>
    /// Defines the test method ViewModelFieldNotifyPropertyChangedNotifiesOnNewValueTest.
    /// </summary>
    [TestMethod]
    public void ViewModelFieldNotifyPropertyChangedNotifiesOnNewValueTest()
    {
        string originalValue = "original value";

        ModelFixture<string> instance = new(originalValue);

        Assert.IsInstanceOfType(instance, typeof(INotifyPropertyChanged));

        bool gotEvent = false;

        ((INotifyPropertyChanged)instance).PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
        {
            gotEvent = true;
            Assert.IsTrue(e.PropertyName!.Equals("Value") | e.PropertyName.Equals("IsValid"), "PropertyName was wrong.");
        };

        string newValue = "new value";
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
        string originalValue = "original value";

        ModelFixture<string> instance = new(originalValue);

        Assert.IsInstanceOfType(instance, typeof(INotifyPropertyChanged));

        bool gotEvent = false;

        ((INotifyPropertyChanged)instance).PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
        {
            gotEvent = true;
            Assert.IsTrue(gotEvent, "Should not get any PropertyChanged events.");
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
        ModelFixture<string> instance = new();

        Assert.IsTrue(instance.Value is null, "Value should be null.");
    }

    /// <summary>
    /// Defines the test method ViewModelFieldConstructorWithValueTest.
    /// </summary>
    [TestMethod]
    public void ViewModelFieldConstructorWithValueTest()
    {
        string expectedValue = "expected value";

        ModelFixture<string> instance = new(expectedValue);

        Assert.IsTrue(expectedValue.Equals(instance.Value), "Value was wrong.");
    }

    /// <summary>
    /// Defines the test method ViewModelFieldToStringReturnsEmptyStringWhenValueIsNullTest.
    /// </summary>
    [TestMethod]
    public void ViewModelFieldToStringReturnsEmptyStringWhenValueIsNullTest()
    {
        ModelFixture<string> instance = new();

        Assert.IsTrue(instance.Value is null, "Value should be null.");
        Assert.IsTrue(instance.ToString().Equals(string.Empty), "ToString() should return empty.");
    }

    /// <summary>
    /// Defines the test method ViewModelFieldToStringTest.
    /// </summary>
    [TestMethod]
    public void ViewModelFieldToStringTest()
    {
        string expectedValue = "expected value";

        ModelFixture<string> instance = new(expectedValue);

        Assert.IsTrue(expectedValue.Equals(instance.Value), "Value was wrong.");
        Assert.IsTrue(expectedValue.Equals(instance.ToString()), "ToString() was wrong.");
    }
}
