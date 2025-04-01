using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Validation.Tests;

/// <summary>
/// Class ArgumentTests.
/// </summary>
[TestClass]
public class ArgumentTests
{
    /// <summary>
    /// Defines the test method IsNotNullTest.
    /// </summary>
    [TestMethod]
    public void IsNotNullTest()
    {
        Product? test = null;
        Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNull(test));
    }

    /// <summary>
    /// Defines the test method IsNotNullOrEmptyTest.
    /// </summary>
    [TestMethod]
    public void IsNotNullOrEmptyTest()
    {
        string test = string.Empty;
        Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(test));
    }

    /// <summary>
    /// Defines the test method GuidIsNotEmptyTest.
    /// </summary>
    [TestMethod]
    public void GuidIsNotEmptyTest()
    {
        Guid test = Guid.Empty;
        Assert.ThrowsException<ArgumentException>(() => Argument.IsNotEmpty(test));
    }

    /// <summary>
    /// Defines the test method GuidIsNotNullOrEmptyTest.
    /// </summary>
    [TestMethod]
    public void GuidIsNotNullOrEmptyTest()
    {
        Guid test = default;
        Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(test));
    }

    /// <summary>
    /// Defines the test method IsNotNullOrWhitespaceTest.
    /// </summary>
    [TestMethod]
    public void IsNotNullOrWhitespaceTest()
    {
        string test = " ";
        Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrWhitespace(test));
    }

    /// <summary>
    /// Defines the test method IsNotNullOrEmptyArrayTest.
    /// </summary>
    [TestMethod]
    public void IsNotNullOrEmptyArrayTest()
    {
        Array test = Array.Empty<object>();
        Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmptyArray(test));
    }

    /// <summary>
    /// Defines the test method IsNotNullOrEmptyListTTest.
    /// </summary>
    [TestMethod]
    public void IsNotNullOrEmptyListTTest()
    {
        List<object> test = [];
        Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmptyList(test));
    }

    /// <summary>
    /// Defines the test method IsNotEnumTTest.
    /// </summary>
    [TestMethod]
    public void IsNotEnumTTest()
    {
        object test = new();
        Assert.ThrowsException<ArgumentException>(() => Argument.IsNotEnum(test));
    }

    /// <summary>
    /// Defines the test method HasNoNullsTTest.
    /// </summary>
    [TestMethod]
    public void HasNoNullsTTest()
    {
        List<Product> test = [new Product(), null];

        Assert.ThrowsException<ArgumentNullException>(() => Argument.HasNoNulls(test));
    }

    /// <summary>
    /// Defines the test method IsNotOutOfRangeTTest.
    /// </summary>
    [TestMethod]
    public void IsNotOutOfRangeTTest()
    {
        int test = 1975;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange(test, 2000, 2021));
    }

    /// <summary>
    /// Defines the test method IsMinimalTTest.
    /// </summary>
    [TestMethod]
    public void IsMinimalTTest()
    {
        int test = 1975;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsMinimal(test, 2000));
    }

    /// <summary>
    /// Defines the test method IsMaximumTTest.
    /// </summary>
    [TestMethod]
    public void IsMaximumTTest()
    {
        int test = 1975;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsMaximum(test, 1970));
    }

    /// <summary>
    /// Tests the Condition method with a failing condition.
    /// </summary>
    [TestMethod]
    public void ConditionFailTest()
    {
        int test = 10;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Argument.Condition(test, x => x < 5));
    }

    /// <summary>
    /// Tests the Condition method with a passing condition.
    /// </summary>
    [TestMethod]
    public void ConditionPassTest()
    {
        int test = 3;
        var result = Argument.Condition(test, x => x < 5);
        Assert.AreEqual(test, result);
    }

    /// <summary>
    /// Tests the Equals method with equal values.
    /// </summary>
    [TestMethod]
    public void EqualsFailTest()
    {
        int test = 5;
        int compareValue = 5;
        Assert.ThrowsException<ArgumentException>(() =>
            Argument.Equals(test, compareValue));
    }

    /// <summary>
    /// Tests the Equals method with different values.
    /// </summary>
    [TestMethod]
    public void EqualsPassTest()
    {
        int test = 5;
        int compareValue = 10;
        var result = Argument.Equals(test, compareValue);
        Assert.AreEqual(test, result);
    }

    /// <summary>
    /// Tests the IsNotOutOfRange method with custom validation function (failing case).
    /// </summary>
    [TestMethod]
    public void IsNotOutOfRangeWithCustomValidationFailTest()
    {
        int test = 15;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Argument.IsNotOutOfRange(test, 0, 10,
                (value, min, max) => value % 2 == 0 && value >= min && value <= max));
    }

    /// <summary>
    /// Tests the IsNotOutOfRange method with custom validation function (passing case).
    /// </summary>
    [TestMethod]
    public void IsNotOutOfRangeWithCustomValidationPassTest()
    {
        int test = 8;
        Argument.IsNotOutOfRange(test, 0, 10,
            (value, min, max) => value % 2 == 0 && value >= min && value <= max);
        // No exception should be thrown
    }

    /// <summary>
    /// Tests the IsMinimal method with custom validation function (failing case).
    /// </summary>
    [TestMethod]
    public void IsMinimalWithCustomValidationFailTest()
    {
        int test = 5;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Argument.IsMinimal(test, 10, (value, min) => value > min));
    }

    /// <summary>
    /// Tests the IsMinimal method with custom validation function (passing case).
    /// </summary>
    [TestMethod]
    public void IsMinimalWithCustomValidationPassTest()
    {
        int test = 15;
        Argument.IsMinimal(test, 10, (value, min) => value > min);
        // No exception should be thrown
    }

    /// <summary>
    /// Tests the IsMaximum method with custom validation function (failing case).
    /// </summary>
    [TestMethod]
    public void IsMaximumWithCustomValidationFailTest()
    {
        int test = 15;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Argument.IsMaximum(test, 10, (value, max) => value < max));
    }

    /// <summary>
    /// Tests the IsMaximum method with custom validation function (passing case).
    /// </summary>
    [TestMethod]
    public void IsMaximumWithCustomValidationPassTest()
    {
        int test = 5;
        Argument.IsMaximum(test, 10, (value, max) => value < max);
        // No exception should be thrown
    }

    /// <summary>
    /// Tests the positive case for IsNotNull.
    /// </summary>
    [TestMethod]
    public void IsNotNullPassTest()
    {
        Product test = new Product();
        bool result = Argument.IsNotNull(test);
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests the positive case for IsNotNullOrEmpty with string.
    /// </summary>
    [TestMethod]
    public void IsNotNullOrEmptyPassTest()
    {
        string test = "Test String";
        bool result = Argument.IsNotNullOrEmpty(test);
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests the positive case for IsNotEmpty with Guid.
    /// </summary>
    [TestMethod]
    public void GuidIsNotEmptyPassTest()
    {
        Guid test = Guid.NewGuid();
        bool result = Argument.IsNotEmpty(test);
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests the positive case for IsNotOutOfRange.
    /// </summary>
    [TestMethod]
    public void IsNotOutOfRangePassTest()
    {
        int test = 2010;
        Argument.IsNotOutOfRange(test, 2000, 2021);
        // No exception should be thrown
    }
}
