namespace ISynergy.Framework.Core.Base;

[TestClass]
public class ObservableClassHashCodeTests
{
    private class TestObservableClass : ObservableClass
    {
    }

    [TestMethod]
    public void GetHashCode_TwoDifferentInstances_ReturnDifferentValues()
    {
        var a = new TestObservableClass();
        var b = new TestObservableClass();
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_SameInstance_IsStable()
    {
        var a = new TestObservableClass();
        Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
    }
}
