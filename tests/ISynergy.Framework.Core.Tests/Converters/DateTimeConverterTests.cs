using ISynergy.Framework.Core.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Converters.Tests;

[TestClass()]
public class DateTimeConverterTests
{
    private readonly DateTime _dateUnspecified;
    private readonly DateTime _dateUtc;
    private readonly DateTime _dateLocal;

    private readonly string _jsonUnspecified;
    private readonly string _jsonUtc;
    private readonly string _jsonLocal;


    private readonly JsonSerializerOptions _serializerOptions;

    public DateTimeConverterTests()
    {
        _serializerOptions = DefaultJsonSerializers.Default;

        _dateUnspecified = new DateTime(1975, 10, 29, 15, 0, 0, DateTimeKind.Unspecified);
        _dateUtc = new DateTime(1975, 10, 29, 15, 0, 0, DateTimeKind.Utc);
        _dateLocal = new DateTime(1975, 10, 29, 15, 0, 0, DateTimeKind.Local);

        _jsonUnspecified = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000", _serializerOptions));
        _jsonUtc = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000Z", _serializerOptions));
        _jsonLocal = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000+01:00", _serializerOptions));
    }


    [TestMethod()]
    public void ReadTestUtc()
    {
        DateTime date = JsonSerializer.Deserialize<DateTime>(_jsonUtc, _serializerOptions);
        Assert.AreEqual(_dateUtc, date.ToUniversalTime());
    }


    [TestMethod()]
    public void WriteTestUtc()
    {
        string json = JsonSerializer.Serialize(_dateUtc, _serializerOptions);
        Assert.AreEqual(_jsonUtc, Regex.Unescape(json));
    }


    [TestMethod()]
    public void ReadTestUnspecified()
    {
        DateTime date = JsonSerializer.Deserialize<DateTime>(_jsonUnspecified, _serializerOptions);
        Assert.AreEqual(_dateUnspecified, date);
    }


    [TestMethod()]
    public void WriteTestUnspecified()
    {
        string json = JsonSerializer.Serialize(_dateUnspecified, _serializerOptions);
        Assert.AreEqual(_jsonUnspecified, Regex.Unescape(json));
    }

    //[TestMethod()]
    //public void ReadTestLocal()
    //{
    //    var date = JsonSerializer.Deserialize<DateTime>(_jsonLocal, _serializerOptions);
    //    Assert.AreEqual(_dateLocal, date);
    //}


    //[TestMethod()]
    //public void WriteTestLocal()
    //{
    //    var json = JsonSerializer.Serialize(_dateLocal, _serializerOptions);
    //    Assert.AreEqual(_jsonLocal, Regex.Unescape(json));
    //}

    [TestMethod]
    public void ReadValidStringReturnsValidResult()
    {
        DateTime expected = DateTime.Parse("2023-12-23T12:23:20.0000010Z");
        const string json = "{\"dt\":\"2023-12-23T12:23:20.0000010Z\"}";
        Dictionary<string, DateTime> actual = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(json, _serializerOptions);
        Assert.AreEqual(expected, actual["dt"]);
    }

    [TestMethod]
    public void ReadNonStringReturnsDefault()
    {
        const string json = "{\"dt\":null}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(json, _serializerOptions);
        Assert.ThrowsException<ArgumentNullException>(act);
    }

    [TestMethod]
    public void ReadBadStringThrows()
    {
        const string json = "{\"dt\":\"not-date\"}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(json, _serializerOptions);
        Assert.ThrowsException<FormatException>(act);
    }
}