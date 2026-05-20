namespace ISynergy.Framework.Core.Base;

[TestClass]
public class ObservableClassConcurrentDisposeTests
{
    private class TestObservableClass : ObservableClass
    {
        public string TestProperty
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }

    [TestMethod]
    public async Task Dispose_WhilePropertyChangedFiringConcurrently_DoesNotThrow()
    {
        var obj = new TestObservableClass();
        obj.PropertyChanged += (s, e) => { };

        Exception? capturedException = null;

        var disposeTask = Task.Run(() =>
        {
            try
            {
                obj.Dispose();
            }
            catch (Exception ex)
            {
                Interlocked.CompareExchange(ref capturedException, ex, null);
            }
        });

        var setValueTask = Task.Run(() =>
        {
            try
            {
                for (int i = 0; i < 50; i++)
                {
                    try
                    {
                        obj.TestProperty = $"value{i}";
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Interlocked.CompareExchange(ref capturedException, ex, null);
            }
        });

        await Task.WhenAll(disposeTask, setValueTask);
        Assert.IsNull(capturedException);
    }
}
