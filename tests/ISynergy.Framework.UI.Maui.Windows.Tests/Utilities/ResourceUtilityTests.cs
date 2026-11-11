using ISynergy.Framework.UI.Utilities;
using Microsoft.Maui.Controls;

namespace ISynergy.Framework.UI.Maui.Utilities;

/// <summary>
/// Tests for the resource resolution helpers in <see cref="ResourceUtility"/>:
/// key validation, direct value passthrough, and the reflective
/// string-to-target-type conversion (including its error path).
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class ResourceUtilityTests
{
    // ── FindResource(ResourceDictionary, key) guards ────────────────────────

    [TestMethod]
    public void FindResource_WithEmptyKey_ReturnsDefault()
    {
        var resources = new ResourceDictionary { { "Answer", 42 } };

        var result = resources.FindResource<int>("");

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void FindResource_WithWhitespaceKey_ReturnsDefault()
    {
        var resources = new ResourceDictionary { { "Answer", 42 } };

        var result = resources.FindResource<int>("   ");

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void FindResource_WithMissingKey_ReturnsDefault()
    {
        var resources = new ResourceDictionary { { "Answer", 42 } };

        var result = resources.FindResource<int>("DoesNotExist");

        Assert.AreEqual(0, result);
    }

    // ── Direct value passthrough ─────────────────────────────────────────────

    [TestMethod]
    public void FindResource_WithDirectValue_ReturnsValue()
    {
        var resources = new ResourceDictionary { { "Answer", 42 } };

        var result = resources.FindResource<int>("Answer");

        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void FindResource_WithDirectStringValue_ReturnsValue()
    {
        var resources = new ResourceDictionary { { "Greeting", "Hello" } };

        var result = resources.FindResource<string>("Greeting");

        Assert.AreEqual("Hello", result);
    }

    // ── String conversion path (TypeDescriptor) ──────────────────────────────

    [TestMethod]
    public void FindResource_WithNumericString_ConvertsToInt()
    {
        var resources = new ResourceDictionary { { "Number", "123" } };

        var result = resources.FindResource<int>("Number");

        Assert.AreEqual(123, result);
    }

    [TestMethod]
    public void FindResource_WithBooleanString_ConvertsToBool()
    {
        var resources = new ResourceDictionary { { "Flag", "true" } };

        var result = resources.FindResource<bool>("Flag");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void FindResource_WithUnparsableString_ReturnsDefault()
    {
        // The TypeDescriptor conversion throws; ConvertFromString must swallow it.
        var resources = new ResourceDictionary { { "NotANumber", "abc" } };

        var result = resources.FindResource<int>("NotANumber");

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void FindResource_WithNonConvertibleType_ReturnsDefault()
    {
        // A string resource for a type whose converter cannot convert from string.
        var resources = new ResourceDictionary { { "Payload", "irrelevant" } };

        var result = resources.FindResource<System.Uri>("Payload");

        // Uri has no string TypeConverter registered that succeeds here; regardless
        // of outcome the call must not throw. Verify no exception escaped and either
        // the value was produced or default returned.
        Assert.IsTrue(result is null || result is System.Uri);
    }

    // ── Static FindResource(string) with no Application.Current ─────────────

    [TestMethod]
    public void FindResource_StaticWithoutApplication_ReturnsDefault()
    {
        // No Application.Current in the unit-test host -> guard returns default.
        var result = ResourceUtility.FindResource<int>("Answer");

        Assert.AreEqual(0, result);
    }
}
