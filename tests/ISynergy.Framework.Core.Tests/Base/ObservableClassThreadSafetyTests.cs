namespace ISynergy.Framework.Core.Base;

[TestClass]
public class ObservableClassThreadSafetyTests
{
    private class TestObservableClass : ObservableClass
    {
        public void SetValuePublic<T>(T value, string propertyName) => SetValue(value, propertyName);
        public T GetValuePublic<T>(string propertyName) => GetValue<T>(propertyName);
    }

    [TestMethod]
    public async Task SetValue_ConcurrentFirstWrite_DoesNotThrowOrCorrupt()
    {
        var obj = new TestObservableClass();
        var tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => obj.SetValuePublic($"value{index}", $"Property{index}")));
        }

        await Task.WhenAll(tasks);
        Assert.AreEqual(10, obj.Properties.Count);
    }

    [TestMethod]
    public async Task GetValue_ConcurrentRead_DoesNotThrow()
    {
        var obj = new TestObservableClass();
        obj.SetValuePublic("test", "SomeProperty");

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => obj.GetValuePublic<string>("SomeProperty")))
            .ToList();

        await Task.WhenAll(tasks);
    }
}
