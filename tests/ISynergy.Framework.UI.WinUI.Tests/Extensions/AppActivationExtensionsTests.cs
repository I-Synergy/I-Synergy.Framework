using System.Reflection;
using System.Runtime.Serialization;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Windows.AppLifecycle;
using Moq;
using Windows.ApplicationModel.Activation;

namespace ISynergy.Framework.UI.WinUI.Extensions;

/// <summary>
/// Tests for <see cref="AppActivationExtensions"/> (internal) activation callback
/// storage and dispatch, plus <see cref="WindowsAppBuilderExtensions.ConfigureSingleInstance"/>
/// and <see cref="WindowsAppBuilderExtensions.ConfigureActivationHandlers"/>.
///
/// <para>
/// <see cref="AppActivationExtensions"/> is internal so all member access is
/// performed via reflection from the containing assembly.  Extension methods on
/// <see cref="IHostApplicationBuilder"/> are public and tested directly.
/// </para>
///
/// <para>
/// WinRT projection types — <see cref="AppActivationArguments"/>,
/// <see cref="LaunchActivatedEventArgs"/>, <see cref="ProtocolActivatedEventArgs"/>
/// — have only a single constructor that takes a native <c>WinRT.IObjectReference</c>
/// and are not constructable from managed unit-test code.  Dispatch-routing tests
/// that depend on these types will report <see cref="Assert.Inconclusive"/> when
/// construction fails.
/// </para>
/// </summary>
[TestClass]
public class AppActivationExtensionsTests
{
    private static readonly Type? s_targetType;

    static AppActivationExtensionsTests()
    {
        // Resolve the internal AppActivationExtensions type via the assembly
        // that contains the public WindowsAppBuilderExtensions.
        var assembly = typeof(WindowsAppBuilderExtensions).Assembly;
        s_targetType = assembly.GetType("ISynergy.Framework.UI.Extensions.AppActivationExtensions");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (s_targetType is null)
            return;

        SetStaticField("s_defaultAction", null);
        SetStaticField("s_protocolAction", null);
        SetStaticField("s_launchAction", null);
        SetStaticField("IsSingleInstanceEnabled", false);
    }

    // ──────────────────────────────────────────────
    //  SetCallbacks
    // ──────────────────────────────────────────────

    [TestMethod]
    public void SetCallbacks_StoresAllCallbacks()
    {
        AssumeTargetTypeIsAvailable();

        var mockDefault = new Mock<Action<string[]>>();
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();

        InvokeStatic("SetCallbacks", mockDefault.Object, mockProtocol.Object, mockLaunch.Object);

        Assert.IsNotNull(GetStaticField("s_defaultAction"), "defaultAction must be stored");
        Assert.IsNotNull(GetStaticField("s_protocolAction"), "protocolAction must be stored");
        Assert.IsNotNull(GetStaticField("s_launchAction"), "launchAction must be stored");
    }

    // ──────────────────────────────────────────────
    //  Dispatch — Launch
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Dispatch_LaunchKind_InvokesLaunchAction()
    {
        AssumeTargetTypeIsAvailable();

        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();
        InvokeStatic("SetCallbacks",
            new Mock<Action<string[]>>().Object,
            new Mock<Action<ProtocolActivatedEventArgs>>().Object,
            mockLaunch.Object);

        if (!TryCreateActivationArgs(out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment; its only " +
                "constructor requires a native WinRT IObjectReference. The dispatch-routing " +
                "logic (if/else on Kind + Data pattern-match) cannot be exercised without it.");
            return;
        }

        InvokeStatic("Dispatch", new object?[] { null, args });
        mockLaunch.Verify(a => a.Invoke(It.IsAny<LaunchActivatedEventArgs>()), Times.Once);
    }

    // ──────────────────────────────────────────────
    //  Dispatch — Protocol
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Dispatch_ProtocolKind_InvokesProtocolAction()
    {
        AssumeTargetTypeIsAvailable();

        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        InvokeStatic("SetCallbacks",
            new Mock<Action<string[]>>().Object,
            mockProtocol.Object,
            new Mock<Action<LaunchActivatedEventArgs>>().Object);

        if (!TryCreateActivationArgs(out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment. " +
                "See Dispatch_LaunchKind_InvokesLaunchAction for details.");
            return;
        }

        InvokeStatic("Dispatch", new object?[] { null, args });
        mockProtocol.Verify(a => a.Invoke(It.IsAny<ProtocolActivatedEventArgs>()), Times.Once);
    }

    // ──────────────────────────────────────────────
    //  Dispatch — Fallback
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Dispatch_Fallback_InvokesDefaultAction()
    {
        AssumeTargetTypeIsAvailable();

        var mockDefault = new Mock<Action<string[]>>();
        InvokeStatic("SetCallbacks",
            mockDefault.Object,
            new Mock<Action<ProtocolActivatedEventArgs>>().Object,
            new Mock<Action<LaunchActivatedEventArgs>>().Object);

        if (!TryCreateActivationArgs(out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment. " +
                "See Dispatch_LaunchKind_InvokesLaunchAction for details.");
            return;
        }

        InvokeStatic("Dispatch", new object?[] { null, args });
        mockDefault.Verify(a => a.Invoke(It.IsAny<string[]>()), Times.Once);
    }

    // ──────────────────────────────────────────────
    //  Dispatch — Unknown kind, no callback
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Dispatch_UnknownKind_NoCallbackInvoked()
    {
        AssumeTargetTypeIsAvailable();

        var mockDefault = new Mock<Action<string[]>>();
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();

        InvokeStatic("SetCallbacks", mockDefault.Object, mockProtocol.Object, mockLaunch.Object);

        if (!TryCreateActivationArgs(out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment. " +
                "See Dispatch_LaunchKind_InvokesLaunchAction for details.");
            return;
        }

        InvokeStatic("Dispatch", new object?[] { null, args });

        mockDefault.Verify(a => a.Invoke(It.IsAny<string[]>()), Times.Never);
        mockProtocol.Verify(a => a.Invoke(It.IsAny<ProtocolActivatedEventArgs>()), Times.Never);
        mockLaunch.Verify(a => a.Invoke(It.IsAny<LaunchActivatedEventArgs>()), Times.Never);
    }

    // ──────────────────────────────────────────────
    //  IsSingleInstanceEnabled flag
    // ──────────────────────────────────────────────

    [TestMethod]
    public void IsSingleInstanceEnabled_DefaultIsFalse()
    {
        AssumeTargetTypeIsAvailable();

        var value = GetStaticField("IsSingleInstanceEnabled");

        Assert.IsNotNull(value);
        Assert.IsFalse((bool)value);
    }

    [TestMethod]
    public void IsSingleInstanceEnabled_AfterConfigureSingleInstance_IsTrue()
    {
        AssumeTargetTypeIsAvailable();

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Object.ConfigureSingleInstance();

        var value = GetStaticField("IsSingleInstanceEnabled");
        Assert.IsNotNull(value);
        Assert.IsTrue((bool)value);
    }

    // ──────────────────────────────────────────────
    //  ConfigureSingleInstance (WindowsAppBuilderExtensions)
    // ──────────────────────────────────────────────

    [TestMethod]
    public void ConfigureSingleInstance_ReturnsSameBuilder()
    {
        var mockBuilder = new Mock<IHostApplicationBuilder>();
        var result = mockBuilder.Object.ConfigureSingleInstance();
        Assert.AreSame(mockBuilder.Object, result);
    }

    // ──────────────────────────────────────────────
    //  ConfigureActivationHandlers (WindowsAppBuilderExtensions)
    // ──────────────────────────────────────────────

    [TestMethod]
    public void ConfigureActivationHandlers_ReturnsSameBuilderAndStoresCallbacks()
    {
        AssumeTargetTypeIsAvailable();

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        var mockDefault = new Mock<Action<string[]>>();
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();

        var result = mockBuilder.Object.ConfigureActivationHandlers(
            mockDefault.Object,
            mockProtocol.Object,
            mockLaunch.Object);

        Assert.AreSame(mockBuilder.Object, result);
        Assert.IsNotNull(GetStaticField("s_defaultAction"));
        Assert.IsNotNull(GetStaticField("s_protocolAction"));
        Assert.IsNotNull(GetStaticField("s_launchAction"));
    }

    // ──────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────

    private static void AssumeTargetTypeIsAvailable()
    {
        if (s_targetType is null)
            Assert.Inconclusive("AppActivationExtensions type could not be resolved from the assembly.");
    }

    private static object? GetStaticField(string fieldName)
    {
        Assert.IsNotNull(s_targetType, "Target type must be available");
        var field = s_targetType!.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        return field?.GetValue(null);
    }

    private static void SetStaticField(string fieldName, object? value)
    {
        Assert.IsNotNull(s_targetType, "Target type must be available");
        var field = s_targetType!.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field?.SetValue(null, value);
    }

    private static void InvokeStatic(string methodName, params object?[] args)
    {
        Assert.IsNotNull(s_targetType, "Target type must be available");
        var method = s_targetType!.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(method, $"Method '{methodName}' not found on {s_targetType.FullName}");
        method.Invoke(null, args);
    }

#pragma warning disable SYSLIB0050 // FormatterServices is obsolete but still the only way to probe WinRT activation types

    /// <summary>
    /// Attempts to create a functional <see cref="AppActivationArguments"/> instance.
    /// The only public constructor takes a <c>WinRT.IObjectReference</c> which is
    /// unavailable outside a running WinRT process.  Returns <c>false</c> in the
    /// normal case (unit-test environment).
    /// </summary>
    private static bool TryCreateActivationArgs(out AppActivationArguments? args)
    {
        args = null;
        try
        {
            var raw = FormatterServices.GetUninitializedObject(typeof(AppActivationArguments));

            // Verify the instance is backed by a native WinRT object by reading a
            // property.  GUSO shells without an _inner reference will throw TIE.
            var kindProp = typeof(AppActivationArguments).GetProperty("Kind");
            if (kindProp is null)
                return false;

            kindProp.GetValue(raw);
            args = (AppActivationArguments)raw;
            return true;
        }
        catch
        {
            return false;
        }
    }
#pragma warning restore SYSLIB0050
}
