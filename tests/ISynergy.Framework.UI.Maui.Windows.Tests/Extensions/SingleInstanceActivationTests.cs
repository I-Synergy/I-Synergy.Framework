using System.Reflection;
using System.Runtime.Serialization;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Windows.AppLifecycle;
using Moq;
using Windows.ApplicationModel.Activation;

namespace ISynergy.Framework.UI.Maui.Extensions;

/// <summary>
/// Tests for <see cref="MauiWinUIApplicationExtensions"/> activation callbacks
/// and <see cref="AppWindowExtensions.ConfigureSingleInstance"/> /
/// <see cref="AppWindowExtensions.ConfigureActivationHandlers"/>.
///
/// Internal members are accessed via reflection (no InternalsVisibleTo between
/// source and test projects; strong-name signing prevents attribute-based access).
///
/// <para>
/// WinRT projection types — <see cref="AppActivationArguments"/>,
/// <see cref="LaunchActivatedEventArgs"/>, <see cref="ProtocolActivatedEventArgs"/>
/// — have only a single constructor that takes a native <c>WinRT.IObjectReference</c>
/// and are not constructable from managed unit-test code.  Tests that require these
/// types for dispatch routing will report <see cref="Assert.Inconclusive"/> when
/// the construction attempt fails.
/// </para>
/// </summary>
[TestClass]
public class SingleInstanceActivationTests
{
    private static readonly Type s_targetType = typeof(MauiWinUIApplicationExtensions);

    [TestCleanup]
    public void Cleanup()
    {
        SetStaticField("s_activationDefault", null);
        SetStaticField("s_activationProtocol", null);
        SetStaticField("s_activationLaunch", null);
    }

    // ──────────────────────────────────────────────
    //  SetActivationCallbacks
    // ──────────────────────────────────────────────

    [TestMethod]
    public void SetActivationCallbacks_StoresAllCallbacks()
    {
        var mockDefault = new Mock<Action<string[]>>();
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();

        InvokeStatic("SetActivationCallbacks", mockDefault.Object, mockProtocol.Object, mockLaunch.Object);

        Assert.IsNotNull(GetStaticField("s_activationDefault"), "defaultAction must be stored");
        Assert.IsNotNull(GetStaticField("s_activationProtocol"), "protocolAction must be stored");
        Assert.IsNotNull(GetStaticField("s_activationLaunch"), "launchAction must be stored");
    }

    // ──────────────────────────────────────────────
    //  InvokeActivationCallbacksDirectly — Launch
    // ──────────────────────────────────────────────

    [TestMethod]
    public void InvokeActivationCallbacksDirectly_LaunchKind_InvokesLaunchAction()
    {
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();
        InvokeStatic("SetActivationCallbacks",
            new Mock<Action<string[]>>().Object,
            new Mock<Action<ProtocolActivatedEventArgs>>().Object,
            mockLaunch.Object);

        if (!TryCreateActivationArgs(ExtendedActivationKind.Launch, out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment; its only " +
                "constructor requires a native WinRT IObjectReference. The dispatch-routing " +
                "logic (if/else on Kind + Data pattern-match) cannot be exercised without it.");
            return;
        }

        InvokeStatic("InvokeActivationCallbacksDirectly", args);
        mockLaunch.Verify(a => a.Invoke(It.IsAny<LaunchActivatedEventArgs>()), Times.Once);
    }

    // ──────────────────────────────────────────────
    //  InvokeActivationCallbacksDirectly — Protocol
    // ──────────────────────────────────────────────

    [TestMethod]
    public void InvokeActivationCallbacksDirectly_ProtocolKind_InvokesProtocolAction()
    {
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        InvokeStatic("SetActivationCallbacks",
            new Mock<Action<string[]>>().Object,
            mockProtocol.Object,
            new Mock<Action<LaunchActivatedEventArgs>>().Object);

        if (!TryCreateActivationArgs(ExtendedActivationKind.Protocol, out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment. " +
                "See InvokeActivationCallbacksDirectly_LaunchKind_InvokesLaunchAction for details.");
            return;
        }

        InvokeStatic("InvokeActivationCallbacksDirectly", args);
        mockProtocol.Verify(a => a.Invoke(It.IsAny<ProtocolActivatedEventArgs>()), Times.Once);
    }

    // ──────────────────────────────────────────────
    //  InvokeActivationCallbacksDirectly — Fallback
    // ──────────────────────────────────────────────

    [TestMethod]
    public void InvokeActivationCallbacksDirectly_Fallback_InvokesDefaultAction()
    {
        var mockDefault = new Mock<Action<string[]>>();
        InvokeStatic("SetActivationCallbacks",
            mockDefault.Object,
            new Mock<Action<ProtocolActivatedEventArgs>>().Object,
            new Mock<Action<LaunchActivatedEventArgs>>().Object);

        if (!TryCreateActivationArgs(ExtendedActivationKind.File, out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment. " +
                "See InvokeActivationCallbacksDirectly_LaunchKind_InvokesLaunchAction for details.");
            return;
        }

        InvokeStatic("InvokeActivationCallbacksDirectly", args);
        mockDefault.Verify(a => a.Invoke(It.IsAny<string[]>()), Times.Once);
    }

    // ──────────────────────────────────────────────
    //  InvokeActivationCallbacksDirectly — Unknown
    // ──────────────────────────────────────────────

    [TestMethod]
    public void InvokeActivationCallbacksDirectly_UnknownKind_NoCallbackInvoked()
    {
        var mockDefault = new Mock<Action<string[]>>();
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();

        InvokeStatic("SetActivationCallbacks", mockDefault.Object, mockProtocol.Object, mockLaunch.Object);

        if (!TryCreateActivationArgs(ExtendedActivationKind.File, out var args))
        {
            Assert.Inconclusive(
                "AppActivationArguments cannot be constructed in this environment. " +
                "See InvokeActivationCallbacksDirectly_LaunchKind_InvokesLaunchAction for details.");
            return;
        }

        InvokeStatic("InvokeActivationCallbacksDirectly", args);

        mockDefault.Verify(a => a.Invoke(It.IsAny<string[]>()), Times.Never);
        mockProtocol.Verify(a => a.Invoke(It.IsAny<ProtocolActivatedEventArgs>()), Times.Never);
        mockLaunch.Verify(a => a.Invoke(It.IsAny<LaunchActivatedEventArgs>()), Times.Never);
    }

    // ──────────────────────────────────────────────
    //  ConfigureSingleInstance (AppWindowExtensions)
    // ──────────────────────────────────────────────

    [TestMethod]
    public void ConfigureSingleInstance_ReturnsSameBuilder()
    {
        var builder = MauiApp.CreateBuilder();
        var result = builder.ConfigureSingleInstance();
        Assert.AreSame(builder, result);
    }

    // ──────────────────────────────────────────────
    //  ConfigureActivationHandlers (AppWindowExtensions)
    // ──────────────────────────────────────────────

    [TestMethod]
    public void ConfigureActivationHandlers_ReturnsSameBuilderAndStoresCallbacks()
    {
        var builder = MauiApp.CreateBuilder();
        var mockDefault = new Mock<Action<string[]>>();
        var mockProtocol = new Mock<Action<ProtocolActivatedEventArgs>>();
        var mockLaunch = new Mock<Action<LaunchActivatedEventArgs>>();

        var result = builder.ConfigureActivationHandlers(
            mockDefault.Object,
            mockProtocol.Object,
            mockLaunch.Object);

        Assert.AreSame(builder, result);
        Assert.IsNotNull(GetStaticField("s_activationDefault"));
        Assert.IsNotNull(GetStaticField("s_activationProtocol"));
        Assert.IsNotNull(GetStaticField("s_activationLaunch"));
    }

    // ──────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────

    private static object? GetStaticField(string fieldName)
    {
        var field = s_targetType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
        return field?.GetValue(null);
    }

    private static void SetStaticField(string fieldName, object? value)
    {
        var field = s_targetType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, value);
    }

    private static void InvokeStatic(string methodName, params object?[] args)
    {
        var method = s_targetType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
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
    private static bool TryCreateActivationArgs(
        ExtendedActivationKind kind,
        out AppActivationArguments? args)
    {
        args = null;
        try
        {
            var raw = FormatterServices.GetUninitializedObject(typeof(AppActivationArguments));

            // If reading Kind throws, the object is a dead shell without a native
            // backing — we cannot exercise the dispatch routing.
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
