using ISynergy.Framework.Core.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Converters.Tests;

[TestClass()]
public class DateTimeOffsetConverterTests
{
    private readonly DateTimeOffset _dateUtc;
    private readonly DateTimeOffset _dateLocal;

    private readonly string _jsonUtc;
    private readonly string _jsonLocal;

    private readonly JsonSerializerOptions _serializerOptions;

    public DateTimeOffsetConverterTests()
    {
        _serializerOptions = DefaultJsonSerializers.Default;

        _dateUtc = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.Zero);
        _dateLocal = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.FromHours(2));

        _jsonUtc = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000Z", _serializerOptions));
        _jsonLocal = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000+02:00", _serializerOptions));
    }


    [TestMethod()]
    public void ReadTestUtc()
    {
        DateTimeOffset date = JsonSerializer.Deserialize<DateTimeOffset>(_jsonUtc, _serializerOptions);
        Assert.AreEqual(_dateUtc, date.ToUniversalTime());
    }


    [TestMethod()]
    public void WriteTestUtc()
    {
        string json = JsonSerializer.Serialize(_dateUtc, _serializerOptions);
        Assert.AreEqual(_jsonUtc, Regex.Unescape(json));
    }

    [TestMethod()]
    public void ReadTestLocal()
    {
        DateTimeOffset date = JsonSerializer.Deserialize<DateTimeOffset>(_jsonLocal, _serializerOptions);
        Assert.AreEqual(_dateLocal, date);
    }


    [TestMethod()]
    public void WriteTestLocal()
    {
        string json = JsonSerializer.Serialize(_dateLocal, _serializerOptions);
        Assert.AreEqual(_jsonLocal, Regex.Unescape(json));
    }

    [TestMethod]
    public void ReadValidStringReturnsValidResult()
    {
        DateTimeOffset expected = DateTimeOffset.Parse("2023-12-23T12:23:20.0000010Z");
        const string json = "{\"dt\":\"2023-12-23T12:23:20.0000010Z\"}";
        Dictionary<string, DateTimeOffset> actual = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>(json, _serializerOptions);
        Assert.AreEqual(expected, actual["dt"]);
    }

    [TestMethod]
    public void ReadNonStringReturnsDefault()
    {
        const string json = "{\"dt\":null}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>(json, _serializerOptions);
        Assert.ThrowsException<ArgumentNullException>(act);
    }

    [TestMethod]
    public void ReadBadStringThrows()
    {
        const string json = "{\"dt\":\"not-date\"}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>(json, _serializerOptions);
        Assert.ThrowsException<FormatException>(act);
    }
}