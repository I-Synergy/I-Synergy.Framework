using ISynergy.Framework.UI.Enumerations;
using Microsoft.Maui.Controls;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions;

/// <summary>
/// Coverage for the defensive catch / best-effort paths of
/// <see cref="ISynergy.Framework.UI.Extensions.CursorExtensions"/> that can be exercised
/// without a live platform view or window. A real MAUI <see cref="VisualElement"/> cannot be
/// constructed in the headless host (its static initializer needs platform state), but both
/// <c>SetupCursor</c> and <c>RemoveCustomCursor</c> wrap their platform work in
/// try/catch — driving them with a null element reaches exactly those catch handlers and
/// proves the helpers never let a failure escape. The cursor <em>mapping</em> itself is
/// covered by <c>CursorExtensionsTests</c>.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class CursorExtensionsSetupTests
{
    private static MethodInfo SetupCursorMethod =>
        typeof(ISynergy.Framework.UI.Extensions.CursorExtensions)
            .GetMethod("SetupCursor", BindingFlags.NonPublic | BindingFlags.Static)!;

    [TestMethod]
    public void SetupCursor_ExistsAsPrivateStatic()
    {
        var method = SetupCursorMethod;
        Assert.IsNotNull(method, "SetupCursor should exist");
        Assert.IsTrue(method.IsPrivate);
        Assert.IsTrue(method.IsStatic);
        Assert.HasCount(3, method.GetParameters());
    }

    [TestMethod]
    public void SetupCursor_WithNullElement_SwallowsException()
    {
        var method = SetupCursorMethod;
        var context = new Mock<IMauiContext>();

        // visualElement.Handler is dereferenced inside the try; a null element throws there
        // and the catch handler must swallow it (no window required).
        Exception? observed = null;
        try
        {
            method.Invoke(null, new object?[] { null, CursorIcons.Hand, context.Object });
        }
        catch (TargetInvocationException tie)
        {
            observed = tie.InnerException;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void SetupCursor_WithNullElementAndNullContext_SwallowsException()
    {
        var method = SetupCursorMethod;

        Exception? observed = null;
        try
        {
            method.Invoke(null, new object?[] { null, CursorIcons.IBeam, null });
        }
        catch (TargetInvocationException tie)
        {
            observed = tie.InnerException;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void RemoveCustomCursor_WithNullElementAndContext_IsBestEffort()
    {
        var context = new Mock<IMauiContext>();

        // ToPlatform on a null element throws inside the try; the catch is a no-op.
        Exception? observed = null;
        try
        {
            ISynergy.Framework.UI.Extensions.CursorExtensions.RemoveCustomCursor(null!, context.Object);
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }
}
