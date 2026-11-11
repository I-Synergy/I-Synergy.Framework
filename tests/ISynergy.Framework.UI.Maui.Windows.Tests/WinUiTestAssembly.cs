namespace ISynergy.Framework.UI.Maui;

/// <summary>
/// Starts and stops the <see cref="WinUiTestHost"/> for the whole test session, so that MAUI
/// controls (pages, shells, …) can be constructed by any test of this assembly.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public static class WinUiTestAssembly
{
    [AssemblyInitialize]
    public static void Initialize(TestContext context) => WinUiTestHost.Start();

    [AssemblyCleanup]
    public static void Cleanup() => WinUiTestHost.Stop();
}
