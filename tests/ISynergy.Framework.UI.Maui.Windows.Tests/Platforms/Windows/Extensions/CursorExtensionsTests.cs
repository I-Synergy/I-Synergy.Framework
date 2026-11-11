using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Maui.Controls;
using System.Reflection;
using Windows.UI.Core;

namespace ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions;

/// <summary>
/// Tests for the Windows cursor helpers in
/// <see cref="ISynergy.Framework.UI.Extensions.CursorExtensions"/>: the null-context
/// guard clauses on <c>SetCustomCursor</c>/<c>RemoveCustomCursor</c> (which must be
/// no-ops and never touch the element) and the private <c>GetCursor</c> mapping from
/// the framework's <see cref="CursorIcons"/> to <see cref="CoreCursorType"/>.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class CursorExtensionsTests
{
    private static readonly MethodInfo s_getCursor =
        typeof(ISynergy.Framework.UI.Extensions.CursorExtensions)
            .GetMethod("GetCursor", BindingFlags.NonPublic | BindingFlags.Static)!;

    private static CoreCursorType InvokeGetCursor(CursorIcons cursor) =>
        (CoreCursorType)s_getCursor.Invoke(null, new object?[] { cursor })!;

    [TestMethod]
    public void GetCursor_ExistsAsPrivateStatic()
    {
        Assert.IsNotNull(s_getCursor, "GetCursor should exist");
        Assert.IsTrue(s_getCursor.IsPrivate);
        Assert.IsTrue(s_getCursor.IsStatic);
        Assert.AreEqual(typeof(CoreCursorType), s_getCursor.ReturnType);
        Assert.HasCount(1, s_getCursor.GetParameters());
        Assert.AreEqual(typeof(CursorIcons), s_getCursor.GetParameters()[0].ParameterType);
    }

    [TestMethod]
    [DataRow(CursorIcons.Hand, CoreCursorType.Hand)]
    [DataRow(CursorIcons.IBeam, CoreCursorType.IBeam)]
    [DataRow(CursorIcons.Cross, CoreCursorType.Cross)]
    [DataRow(CursorIcons.Arrow, CoreCursorType.Arrow)]
    [DataRow(CursorIcons.SizeAll, CoreCursorType.SizeAll)]
    [DataRow(CursorIcons.Wait, CoreCursorType.Wait)]
    public void GetCursor_MapsEachIcon(CursorIcons icon, CoreCursorType expected)
    {
        Assert.AreEqual(expected, InvokeGetCursor(icon));
    }

    [TestMethod]
    public void GetCursor_UnknownValue_FallsBackToArrow()
    {
        Assert.AreEqual(CoreCursorType.Arrow, InvokeGetCursor((CursorIcons)999));
    }

    // ── Null-context guards ─────────────────────────────────────────────────

    [TestMethod]
    public void SetCustomCursor_WithNullContext_DoesNotThrow()
    {
        // The null mauiContext guard returns before the visual element is dereferenced,
        // so a null element is safe and proves the guard runs first.
        Exception? observed = null;
        try
        {
            ISynergy.Framework.UI.Extensions.CursorExtensions.SetCustomCursor(
                null!, CursorIcons.Hand, null);
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void RemoveCustomCursor_WithNullContext_DoesNotThrow()
    {
        Exception? observed = null;
        try
        {
            ISynergy.Framework.UI.Extensions.CursorExtensions.RemoveCustomCursor(null!, null);
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }
}
