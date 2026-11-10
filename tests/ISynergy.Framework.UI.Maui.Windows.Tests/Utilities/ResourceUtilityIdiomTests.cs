using ISynergy.Framework.UI.Utilities;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Devices;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Utilities;

/// <summary>
/// Exercises every branch of <c>ResourceUtility.ResolveOnIdiom</c> and both idiom resource shapes
/// (<see cref="OnIdiom{T}"/> and <see cref="OnIdiomExtension"/>) by substituting the
/// <see cref="DeviceInfo.Idiom"/> the utility reads.
/// The MAUI <see cref="DeviceInfo"/> implementation is swapped via its internal
/// <c>SetCurrent</c> hook and restored in <see cref="Cleanup"/>.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class ResourceUtilityIdiomTests
{
    private static readonly MethodInfo? s_setCurrent =
        typeof(DeviceInfo).GetMethod("SetCurrent", BindingFlags.NonPublic | BindingFlags.Static);

    private static ResourceDictionary CreateOnIdiomResources() => new()
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

    /// <summary><see cref="DeviceIdiom"/> is a struct with well-known values, not an enum.</summary>
    private static DeviceIdiom ToIdiom(string name) => name switch
    {
        "Desktop" => DeviceIdiom.Desktop,
        "Phone" => DeviceIdiom.Phone,
        "Tablet" => DeviceIdiom.Tablet,
        "TV" => DeviceIdiom.TV,
        "Watch" => DeviceIdiom.Watch,
        _ => DeviceIdiom.Unknown,
    };

    private static void SetIdiom(DeviceIdiom idiom)
    {
        Assert.IsNotNull(s_setCurrent, "DeviceInfo.SetCurrent was not found; cannot simulate an idiom.");

        var deviceInfo = new Mock<IDeviceInfo>();
        deviceInfo.SetupGet(d => d.Idiom).Returns(idiom);

        s_setCurrent!.Invoke(null, [deviceInfo.Object]);
    }

    [TestCleanup]
    public void Cleanup() => s_setCurrent?.Invoke(null, [null]);

    [TestMethod]
    [DataRow("Desktop", 1)]
    [DataRow("Phone", 2)]
    [DataRow("Tablet", 3)]
    [DataRow("TV", 4)]
    [DataRow("Watch", 5)]
    [DataRow("Unknown", 7)]
    public void ResolveOnIdiom_OnIdiomOfT_ReturnsValueOfCurrentIdiom(string idiomName, int expected)
    {
        // Guard: the internal DeviceInfo hook must be reachable for this simulation to be meaningful.
        if (s_setCurrent is null)
            Assert.Inconclusive("DeviceInfo.SetCurrent is not available in this host.");

        SetIdiom(ToIdiom(idiomName));

        var result = CreateOnIdiomResources().FindResource<int>("Idiomatic");

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("Desktop", 1)]
    [DataRow("Phone", 2)]
    [DataRow("Tablet", 3)]
    [DataRow("TV", 4)]
    [DataRow("Watch", 5)]
    [DataRow("Unknown", 7)]
    public void ResolveOnIdiom_OnIdiomExtension_ReturnsValueOfCurrentIdiom(string idiomName, int expected)
    {
        if (s_setCurrent is null)
            Assert.Inconclusive("DeviceInfo.SetCurrent is not available in this host.");

        SetIdiom(ToIdiom(idiomName));

        var resources = new ResourceDictionary
        {
            {
                "Idiomatic",
                new OnIdiomExtension
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

        var result = resources.FindResource<int>("Idiomatic");

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ResolveOnIdiom_UnknownIdiom_FallsBackToDefaultValue()
    {
        if (s_setCurrent is null)
            Assert.Inconclusive("DeviceInfo.SetCurrent is not available in this host.");

        SetIdiom(DeviceIdiom.Unknown);

        var resources = new ResourceDictionary
        {
            { "NoDefaultForUnknown", new OnIdiom<int> { Phone = 2 } }
        };

        // DeviceIdiom.Unknown matches none of the known idioms, so the default (0) is returned.
        Assert.AreEqual(0, resources.FindResource<int>("NoDefaultForUnknown"));
    }
}
