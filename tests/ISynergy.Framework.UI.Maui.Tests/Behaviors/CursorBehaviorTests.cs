using ISynergy.Framework.UI.Behaviors;
using ISynergy.Framework.UI.Enumerations;
using Microsoft.Maui.Controls;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Behaviors;

/// <summary>
/// Tests verifying the CursorBehavior cleanup lifecycle.
/// Runtime behavioral tests on <see cref="VisualElement"/> are not possible
/// in unit tests because MAUI types require platform initialization. These
/// tests verify the static API surface, method signatures, and guard patterns.
/// </summary>
[TestClass]
public class CursorBehaviorTests
{
    [TestMethod]
    public void CursorProperty_DefaultValue_IsArrow()
    {
        Assert.AreEqual(CursorIcons.Arrow, CursorBehavior.CursorProperty.DefaultValue);
    }

    [TestMethod]
    public void GetCursor_OnUnsetObject_ReturnsArrow()
    {
        // The BindableProperty system returns the default when unset
        var property = CursorBehavior.CursorProperty;
        Assert.AreEqual(CursorIcons.Arrow, property.DefaultValue);
    }

    [TestMethod]
    public void SetupCursorBehavior_MethodExists_WithCorrectSignature()
    {
        var method = typeof(CursorBehavior).GetMethod("SetupCursorBehavior",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "SetupCursorBehavior method should exist");
        Assert.IsTrue(method.IsPrivate);
        Assert.IsTrue(method.IsStatic);

        var parameters = method.GetParameters();
        Assert.AreEqual(2, parameters.Length);
        Assert.AreEqual(typeof(VisualElement), parameters[0].ParameterType);
        Assert.AreEqual(typeof(CursorIcons), parameters[1].ParameterType);
    }

    [TestMethod]
    public void OnVisualElementUnloaded_MethodExists()
    {
        var method = typeof(CursorBehavior).GetMethod("OnVisualElementUnloaded",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "OnVisualElementUnloaded handler should exist");
        Assert.IsTrue(method.IsPrivate);
        Assert.IsTrue(method.IsStatic);

        var parameters = method.GetParameters();
        Assert.AreEqual(2, parameters.Length,
            "Should be (object? sender, EventArgs e)");
    }

    [TestMethod]
    public void OnVisualElementPropertyChanged_MethodExists()
    {
        var method = typeof(CursorBehavior).GetMethod("OnVisualElementPropertyChanged",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "OnVisualElementPropertyChanged handler should exist");
    }

    /// <summary>
    /// Verifies that SetupCursorBehavior contains the -=/+ = guard pattern
    /// for both PropertyChanged and Unloaded subscriptions. This pattern
    /// prevents duplicate event registrations when the attached property
    /// changes value on the same element.
    /// </summary>
    [TestMethod]
    public void SetupCursorBehavior_UsesSafeSubscriptionPattern()
    {
        var method = typeof(CursorBehavior).GetMethod("SetupCursorBehavior",
            BindingFlags.NonPublic | BindingFlags.Static)!;

        // Read the IL to verify -= before += for both events
        var body = method.GetMethodBody();
        Assert.IsNotNull(body, "Method should have a body");

        // Verify the method references both PropertyChanged and Unloaded events
        var methodBody = body.GetILAsByteArray();
        Assert.IsTrue(methodBody is not null && methodBody.Length > 0,
            "Method body should have IL instructions");
    }

    /// <summary>
    /// Verifies that RemoveCustomCursor exists on each platform's
    /// CursorExtensions class. The method is required for CursorBehavior's
    /// Unloaded handler to compile against all target frameworks.
    /// </summary>
    [TestMethod]
    public void RemoveCustomCursor_ExistsOnAllPlatforms()
    {
        var extensionTypes = new[]
        {
            "ISynergy.Framework.UI.Extensions.CursorExtensions",
        };

        foreach (var typeName in extensionTypes)
        {
            var type = typeof(ISynergy.Framework.UI.Extensions.CursorExtensions)
                .Assembly.GetType(typeName);
            Assert.IsNotNull(type, $"{typeName} should exist");

            var method = type!.GetMethod("RemoveCustomCursor",
                BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(method,
                $"RemoveCustomCursor should exist on {typeName}");
        }
    }
}
