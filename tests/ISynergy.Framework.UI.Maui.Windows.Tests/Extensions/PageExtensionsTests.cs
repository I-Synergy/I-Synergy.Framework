using ISynergy.Framework.UI.Extensions;
using Microsoft.Maui.Controls;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Extensions;

/// <summary>
/// Tests for <see cref="PageExtensions"/>. The public <c>ApplyWindowFlowDirection</c>
/// helper applies the MAUI-level flow direction and returns the page for chaining;
/// the head-specific work is delegated to the private <c>ApplyPlatformFlowDirection</c>.
///
/// A real <see cref="ContentPage"/> cannot always be constructed in a headless unit-test
/// host, so the behavioural test reports <see cref="Assert.Inconclusive"/> when
/// construction fails (matching the approach used by <c>SingleInstanceActivationTests</c>);
/// the structural tests always run.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class PageExtensionsTests
{
    private static readonly Type s_type = typeof(PageExtensions);

    [TestMethod]
    public void ApplyWindowFlowDirection_IsPublicGenericStaticWithExpectedShape()
    {
        var method = s_type.GetMethod(
            "ApplyWindowFlowDirection",
            BindingFlags.Public | BindingFlags.Static);

        Assert.IsNotNull(method, "ApplyWindowFlowDirection should exist");
        Assert.IsTrue(method!.IsGenericMethodDefinition);
        Assert.AreEqual(typeof(Page), method.GetGenericArguments()[0].GetGenericParameterConstraints()[0]);

        var parameters = method.GetParameters();
        Assert.HasCount(2, parameters);
        Assert.AreEqual(typeof(string), parameters[1].ParameterType);
    }

    [TestMethod]
    public void ApplyPlatformFlowDirection_IsPrivateStatic()
    {
        var method = s_type.GetMethod(
            "ApplyPlatformFlowDirection",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "ApplyPlatformFlowDirection should exist");
        Assert.IsTrue(method!.IsPrivate);
        Assert.IsTrue(method.IsGenericMethodDefinition);
    }

    [TestMethod]
    public void ApplyWindowFlowDirection_ReturnsSamePageInstance()
    {
        ContentPage page;
        try
        {
            page = new ContentPage();
        }
        catch (Exception ex)
        {
            Assert.Inconclusive(
                "ContentPage cannot be constructed in this host without MAUI platform " +
                $"initialization: {ex.GetType().Name}.");
            return;
        }

        var result = page.ApplyWindowFlowDirection("ar");

        Assert.AreSame(page, result, "The helper is documented to return the page for chaining");
    }
}
