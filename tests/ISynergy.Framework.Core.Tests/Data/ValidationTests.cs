using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Data.Tests;

/// <summary>
/// Class ValidationTests.
/// </summary>
[TestClass]
public class ValidationTests
{
    private const string _valueIsNull = "Value is null";

    /// <summary>
    /// Defines the test method ValidateIsNullTest.
    /// </summary>
    [TestMethod]
    public void ValidateIsNullTest()
    {
        using ModelFixture<string> model = new()
        {
            Value = null!,
            Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (string.IsNullOrEmpty(i!.Value))
                {
                    i.AddValidationError(nameof(i.Value), _valueIsNull);
                }
            })
        };
        Assert.IsFalse(model.Validate());
    }

    /// <summary>
    /// Defines the test method ValidateIsNotNullTest.
    /// </summary>
    [TestMethod]
    public void ValidateIsNotNullTest()
    {
        using ModelFixture<string> model = new()
        {
            Value = "192.168.1.0",
            Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (string.IsNullOrEmpty(i!.Value))
                {
                    i.AddValidationError(nameof(i.Value), _valueIsNull);
                }
            })
        };
        Assert.IsTrue(model.Validate());
    }


    private const string _portNumberNotInRange = "Portnumber is not in range [1,100]";

    /// <summary>
    /// Defines the test method ValidateIsNotInRangeTest.
    /// </summary>
    [TestMethod]
    public void ValidateIsNotInRangeTest()
    {
        using ModelFixture<int> model = new()
        {
            Value = 9999,
            Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<int>;

                if (!Enumerable.Range(1, 100).Contains(i!.Value))
                {
                    i.AddValidationError(nameof(i.Value), _portNumberNotInRange);
                }
            })
        };
        Assert.IsFalse(model.Validate());
    }

    /// <summary>
    /// Defines the test method ValidateIsInRangeTest.
    /// </summary>
    [TestMethod]
    public void ValidateIsInRangeTest()
    {
        using ModelFixture<int> model = new()
        {
            Value = 80,
            Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<int>;

                if (!Enumerable.Range(1, 500).Contains(i!.Value))
                {
                    i.AddValidationError(nameof(i.Value), _portNumberNotInRange);
                }
            })
        };
        Assert.IsTrue(model.Validate());
    }

    private const string _stringLengthNotInRange = "String should have length of 1-35 characters";

    /// <summary>
    /// Defines the test method ValidateStringLengthIsNotInRangeTest.
    /// </summary>
    [TestMethod]
    public void ValidateStringLengthIsNotInRangeTest()
    {
        using ModelFixture<string> model = new()
        {
            Value = "",
            Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (!Enumerable.Range(1, 35).Contains(i!.Value.Length))
                {
                    i.AddValidationError(nameof(i.Value), _stringLengthNotInRange);
                }
            })
        };
        Assert.IsFalse(model.Validate());
    }

    /// <summary>
    /// Defines the test method ValidateStringLengthIsInRangeTest.
    /// </summary>
    [TestMethod]
    public void ValidateStringLengthIsInRangeTest()
    {
        using ModelFixture<string> model = new()
        {
            Value = "192.168.1.0",
            Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (!Enumerable.Range(1, 35).Contains(i!.Value.Length))
                {
                    i.AddValidationError(nameof(i.Value), _stringLengthNotInRange);
                }
            })
        };
        Assert.IsTrue(model.Validate());
    }

    private const string _selectedItemNull = "SelectedItem cannot be null.";

    [TestMethod]
    public void ViewModelProductIsNullTest()
    {
        TestViewModel fixture = new TestViewModel();

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(1, fixture.Errors.Count);
        Assert.AreEqual(_selectedItemNull, fixture.Errors.Single().Value);
    }

    [TestMethod]
    public void ViewModelProductIsNotNullTest()
    {
        TestViewModel fixture = new TestViewModel
        {
            SelectedItem = new Product()
        };

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(2, fixture.Errors.Count);

        Assert.IsTrue(fixture.Validate(false));
        Assert.AreEqual(0, fixture.Errors.Count);
    }

    [TestMethod]
    public void ViewModelProductIsNotNullAndQuantityIsOneTest()
    {
        TestViewModel fixture = new TestViewModel
        {
            SelectedItem = new Product() { Quantity = 1 }
        };

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(1, fixture.Errors.Count);

        Assert.IsTrue(fixture.Validate(false));
        Assert.AreEqual(0, fixture.Errors.Count);
    }

    [TestMethod]
    public void ViewModelProductIsNotNullAndQuantityIsOneChangedValidationTest()
    {
        TestViewModel fixture = new TestViewModel();

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(1, fixture.Errors.Count);

        fixture.SelectedItem = new Product()
        {
            Quantity = 0
        };

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(2, fixture.Errors.Count);

        Assert.IsTrue(fixture.Validate(false));
        Assert.AreEqual(0, fixture.Errors.Count);
    }

    [TestMethod]
    public void ViewModelProductIsNotNullAndQuantityIsOnePropertiesValidationTest()
    {
        TestViewModel fixture = new TestViewModel();

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(1, fixture.Errors.Count);

        fixture.SelectedItem = new Product()
        {
            Quantity = 0
        };

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(2, fixture.Errors.Count);

        fixture.SelectedItem.Quantity = 1;

        Assert.IsFalse(fixture.Validate());
        Assert.AreEqual(1, fixture.Errors.Count);
    }
}
