using ISynergy.Framework.UI.Abstractions;

namespace Sample.Wasm;

public class Program
{
    private static IBaseApplication _app;

    public static int Main(string[] args)
    {
        Microsoft.UI.Xaml.Application.Start(_ => _app = new AppHead());

        return 0;
    }
}
