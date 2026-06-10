using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Globalization.Converters;

[TestClass]
public class TimeOnlyConverterTests
{
    private readonly JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.General)
    {
        Converters =
            {
                new JsonStringEnumConverter(),
                new DateTimeConverter(),
                new DateTimeOffsetConverter(),
                new TimeOnlyConverter()
            },
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.Preserve
    };

    [TestMethod]
    public void ReadValidStringReturnsValidResult()
    {
        TimeOnly expected = TimeOnly.Parse("12:23:20.010");
        const string json = "{\"dt\":\"12:23:20.010\"}";
        var actual = JsonSerializer.Deserialize<Dictionary<string, TimeOnly>>(json, options);
        Assert.AreEqual(expected, actual!["dt"]);
    }

    [TestMethod]
    public void ReadNonStringReturnsDefault()
    {
        const string json = "{\"dt\":null}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, TimeOnly>>(json, options);
        Assert.Throws<FormatException>(act);
    }

    [TestMethod]
    public void ReadBadStringThrows()
    {
        const string json = "{\"dt\":\"not-time\"}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, TimeOnly>>(json, options);
        Assert.Throws<FormatException>(act);
    }
}