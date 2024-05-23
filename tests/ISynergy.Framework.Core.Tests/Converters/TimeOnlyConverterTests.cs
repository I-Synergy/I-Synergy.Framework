using ISynergy.Framework.Core.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace ISynergy.Framework.Core.Tests.Converters;

[TestClass]
public class TimeOnlyConverterTests
{
    private readonly JsonSerializerOptions options = DefaultJsonSerializers.Default();

    [TestMethod]
    public void ReadValidStringReturnsValidResult()
    {
        TimeOnly expected = TimeOnly.Parse("12:23:20.010");
        const string json = "{\"dt\":\"12:23:20.010\"}";
        Dictionary<string, TimeOnly> actual = JsonSerializer.Deserialize<Dictionary<string, TimeOnly>>(json, options);
        Assert.AreEqual(expected, actual["dt"]);
    }

    [TestMethod]
    public void ReadNonStringReturnsDefault()
    {
        const string json = "{\"dt\":null}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, TimeOnly>>(json, options);
        Assert.ThrowsException<FormatException>(act);
    }

    [TestMethod]
    public void ReadBadStringThrows()
    {
        const string json = "{\"dt\":\"not-time\"}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, TimeOnly>>(json, options);
        Assert.ThrowsException<FormatException>(act);
    }
}