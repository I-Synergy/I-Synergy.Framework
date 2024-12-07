using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.UI.Extensions.Tests;

[TestClass]
public class PreferencesExtensionsTests
{
    [TestMethod]
    public void SetObject_ShouldSerializeAndSetObjectInPreferences()
    {
        // Arrange
        var preferencesMock = new Mock<IPreferences>();
        var key = "testKey";
        var obj = new TestObject { Name = "Test", Age = 25 };

        // Act
        preferencesMock.Object.SetObject(key, obj);

        // Assert
        preferencesMock.Verify(p => p.Set<string>(key, It.IsAny<string>(), null), Times.Once);
    }

    [TestMethod]
    public void GetObject_ShouldDeserializeAndGetObjectFromPreferences()
    {
        // Arrange
        var preferencesMock = new Mock<IPreferences>();
        var key = "testKey";
        var defaultValue = new TestObject { Name = "Default", Age = 30 };
        var jsonValue = "{\"Name\":\"Test\",\"Age\":25}";

        preferencesMock.Setup(p => p.Get<string>(key, null, null)).Returns(jsonValue);

        // Act
        var result = preferencesMock.Object.GetObject<TestObject>(key, defaultValue);

        // Assert
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(25, result.Age);
    }

    [TestMethod]
    public void SetObjectShared_ShouldSerializeAndSetObjectInPreferences()
    {
        // Arrange
        var preferencesMock = new Mock<IPreferences>();
        var key = "testSharedKey";
        var obj = new TestObject { Name = "Test", Age = 25 };
        var sharedName = "sharedPreferences";

        // Act
        preferencesMock.Object.SetObject(key, obj, sharedName);

        // Assert
        preferencesMock.Verify(p => p.Set<string>(key, It.IsAny<string>(), sharedName), Times.Once);
    }

    [TestMethod]
    public void GetObjectShared_ShouldDeserializeAndGetObjectFromPreferences()
    {
        // Arrange
        var preferencesMock = new Mock<IPreferences>();
        var key = "testSharedKey";
        var defaultValue = new TestObject { Name = "Default", Age = 30 };
        var sharedName = "sharedPreferences";
        var jsonValue = "{\"Name\":\"Test\",\"Age\":25}";

        preferencesMock.Setup(p => p.Get<string>(key, null, sharedName)).Returns(jsonValue);

        // Act
        var result = preferencesMock.Object.GetObject<TestObject>(key, defaultValue, sharedName);

        // Assert
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(25, result.Age);
    }

    [TestMethod]
    public void SetObjectShared_ShouldFailInPreferences()
    {
        // Arrange
        var preferencesMock = new Mock<IPreferences>();
        var key = "testSharedKey";
        var obj = new TestObject { Name = "Test", Age = 25 };
        var sharedName = "sharedPreferences";

        // Act
        preferencesMock.Object.SetObject(key, obj, null);

        // Assert
        preferencesMock.Verify(p => p.Set<string>(key, It.IsAny<string>(), sharedName), Times.Never);
    }

    [TestMethod]
    public void GetObjectShared_ShouldFailFromPreferences()
    {
        // Arrange
        var preferencesMock = new Mock<IPreferences>();
        var key = "testSharedKey";
        var defaultValue = new TestObject { Name = "Default", Age = 30 };
        var sharedName = "sharedPreferences";
        var jsonValue = "{\"Name\":\"Test\",\"Age\":25}";

        preferencesMock.Setup(p => p.Get<string>(key, null, sharedName)).Returns(jsonValue);

        // Act
        var result = preferencesMock.Object.GetObject<TestObject>(key, defaultValue, null);

        // Assert
        Assert.AreNotEqual("Test", result.Name);
        Assert.AreNotEqual(25, result.Age);
        Assert.AreEqual("Default", result.Name);
        Assert.AreEqual(30, result.Age);
    }

    private class TestObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
