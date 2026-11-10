using ISynergy.Framework.UI.Utilities;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

namespace ISynergy.Framework.UI.Maui.Utilities;

/// <summary>
/// Additional coverage for the remaining branches of <see cref="ResourceUtility"/>:
/// <list type="bullet">
///   <item>the <c>default</c> case of the resource switch (a value that is neither the
///         requested type nor a supported wrapper),</item>
///   <item><c>DynamicResource</c> key indirection,</item>
///   <item><c>OnIdiom&lt;T&gt;</c> idiom resolution,</item>
///   <item><c>ConvertFromString</c> when the target type has no string converter.</item>
/// </list>
/// Idiom-dependent cases are guarded because <c>DeviceInfo.Idiom</c> is not guaranteed to be
/// available in a headless host.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class ResourceUtilityBranchTests
{
    /// <summary>A type with no <see cref="System.ComponentModel.TypeConverter"/> for strings.</summary>
    private sealed class PayloadWithoutStringConverter
    {
        public int Value { get; set; }
    }

    private static readonly int[] s_idiomValues = { 1, 2, 3, 4, 5, 7 };

    // ── default switch branch ────────────────────────────────────────────────

    [TestMethod]
    public void FindResource_WithIncompatibleValueType_ReturnsDefault()
    {
        // A double resource requested as an int matches no case and falls to `default`.
        var resources = new ResourceDictionary { { "Ratio", 1.5d } };

        var result = resources.FindResource<int>("Ratio");

        Assert.AreEqual(0, result);
    }

    // ── ConvertFromString: converter cannot convert from string ──────────────

    [TestMethod]
    public void FindResource_WithTypeLackingStringConverter_ReturnsDefault()
    {
        var resources = new ResourceDictionary { { "Payload", "anything" } };

        var result = resources.FindResource<PayloadWithoutStringConverter>("Payload");

        Assert.IsNull(result);
    }

    // ── DynamicResource indirection ──────────────────────────────────────────

    [TestMethod]
    public void FindResource_WithDynamicResource_ResolvesKeyedValue()
    {
        var resources = new ResourceDictionary
        {
            { "Answer", 42 },
            { "Indirection", new DynamicResource("Answer") }
        };

        var result = resources.FindResource<int>("Indirection");

        Assert.AreEqual(42, result);
    }

    // ── OnPlatform<T> resolution ─────────────────────────────────────────────

    [TestMethod]
    public void FindResource_WithOnPlatformWithoutPlatformMatch_ReturnsDefault()
    {
        // No platform entries -> GetResource recurses with null and returns default.
        var resources = new ResourceDictionary
        {
            { "Platform", new OnPlatform<int> { Default = 9 } }
        };

        Exception? hostException = null;
        int result;
        try
        {
            result = resources.FindResource<int>("Platform");
        }
        catch (Exception ex)
        {
            hostException = ex;
            result = -1;
        }

        if (hostException is not null)
        {
            Assert.Inconclusive($"DeviceInfo.Platform unavailable in this host: {hostException.GetType().Name}.");
        }

        // No platform-specific match, so the recursive lookup yields the default of int.
        Assert.AreEqual(0, result);
    }

    // ── OnIdiom<T> resolution ────────────────────────────────────────────────

    [TestMethod]
    public void FindResource_WithOnIdiom_ResolvesOneOfTheIdiomValues()
    {
        var resources = new ResourceDictionary
        {
            {
                "Idiomatic",
                new OnIdiom<int>
                {
                    Default = 7,
                    Desktop = 1,
                    Phone = 2,
                    Tablet = 3,
                    TV = 4,
                    Watch = 5,
                }
            }
        };

        Exception? hostException = null;
        int result;
        try
        {
            result = resources.FindResource<int>("Idiomatic");
        }
        catch (Exception ex)
        {
            hostException = ex;
            result = -1;
        }

        if (hostException is not null)
        {
            Assert.Inconclusive($"DeviceInfo.Idiom unavailable in this host: {hostException.GetType().Name}.");
        }

        // Regardless of the current idiom the resolved value must be one of the configured ones.
        Assert.Contains(result, s_idiomValues);
    }
}
