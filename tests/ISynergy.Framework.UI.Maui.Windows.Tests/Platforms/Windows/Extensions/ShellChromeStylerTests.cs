using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions;

/// <summary>
/// Tests for <see cref="ShellChromeStyler"/>: the public style-id constants that
/// shells reference from XAML, the null-element guard in <c>ApplyHandCursor</c>, and
/// the registration flag. <c>Register()</c> touches the WinUI handler mappers whose
/// static constructors require a running WinUI process, so that path reports
/// <see cref="Assert.Inconclusive"/> when the platform is unavailable.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class ShellChromeStylerTests
{
    private static readonly Type s_type = typeof(ShellChromeStyler);

    [TestMethod]
    public void HeaderIconButtonStyleId_HasExpectedValue()
    {
        Assert.AreEqual("HeaderIconButton", ShellChromeStyler.HeaderIconButtonStyleId);
    }

    [TestMethod]
    public void SidebarRowStyleId_HasExpectedValue()
    {
        Assert.AreEqual("SidebarRow", ShellChromeStyler.SidebarRowStyleId);
    }

    [TestMethod]
    public void LinkLabelStyleId_HasExpectedValue()
    {
        Assert.AreEqual("LinkLabel", ShellChromeStyler.LinkLabelStyleId);
    }

    [TestMethod]
    public void ApplyHandCursor_WithNullElement_ReturnsWithoutThrowing()
    {
        var method = s_type.GetMethod(
            "ApplyHandCursor", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method, "ApplyHandCursor should exist");

        Exception? observed = null;
        try
        {
            method!.Invoke(null, new object?[] { null });
        }
        catch (TargetInvocationException tie)
        {
            observed = tie.InnerException;
        }

        // The null-element guard must short-circuit before touching WinUI types.
        Assert.IsNull(observed);
    }

    [TestMethod]
    public void Register_SetsRegisteredFlagOrSkipsWhenPlatformUnavailable()
    {
        var flagField = s_type.GetField("_isRegistered", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(flagField, "_isRegistered flag should exist");

        Exception? hostException = null;
        try
        {
            ShellChromeStyler.Register();
            // Idempotent: a second call must short-circuit rather than double-register.
            ShellChromeStyler.Register();
        }
        catch (TypeInitializationException ex)
        {
            hostException = ex;
        }

        if (hostException is not null)
        {
            Assert.Inconclusive(
                "WinUI handler mappers require a running WinUI process; Register() cannot be " +
                $"exercised headless: {hostException.InnerException?.GetType().Name}.");
        }

        Assert.IsTrue((bool)flagField!.GetValue(null)!, "_isRegistered should be set after Register()");
    }
}
