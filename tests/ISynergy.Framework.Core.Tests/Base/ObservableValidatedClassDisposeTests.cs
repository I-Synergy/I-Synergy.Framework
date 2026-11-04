namespace ISynergy.Framework.Core.Base;

[TestClass]
public class ObservableValidatedClassDisposeTests
{
    private class TestObservableValidatedClass : ObservableValidatedClass
    {
        public string TestProperty
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }

    [TestMethod]
    public async Task Dispose_CalledConcurrentlyWithDisposeAsync_DoesNotThrow()
    {
        var obj = new TestObservableValidatedClass();

        await Task.WhenAll(
            Task.Run(() => obj.Dispose(), TestContext.CancellationToken),
            Task.Run(async () => await obj.DisposeAsync(), TestContext.CancellationToken)
        );

        Assert.IsTrue(obj.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_ErrorsChangedNotFiredAfterPropertiesCleared()
    {
        var obj = new TestObservableValidatedClass();
        var errorsChangedFired = false;
        obj.ErrorsChanged += (s, e) => errorsChangedFired = true;

        await obj.DisposeAsync();

        Assert.IsFalse(errorsChangedFired);
        Assert.IsEmpty(obj.Properties);
        Assert.AreEqual(0, obj.Errors?.Count);
    }

    public TestContext TestContext { get; set; }
}
