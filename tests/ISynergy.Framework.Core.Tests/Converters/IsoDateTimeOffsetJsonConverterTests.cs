using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Converters.Tests;

[TestClass()]
public class IsoDateTimeOffsetJsonConverterTests
{
    private readonly DateTimeOffset _dateUtc;
    private readonly DateTimeOffset _dateLocal;

    private readonly string _jsonUtc;
    private readonly string _jsonLocal;

    private readonly JsonSerializerOptions _serializerOptions;

    public IsoDateTimeOffsetJsonConverterTests()
    {
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.Converters.Add(new IsoDateTimeOffsetJsonConverter());

        _dateUtc = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.Zero);
        _dateLocal = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.FromHours(2));

        _jsonUtc = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000Z", _serializerOptions));
        _jsonLocal = Regex.Unescape(JsonSerializer.Serialize("1975-10-29T15:00:00.0000000+02:00", _serializerOptions));
    }


    [TestMethod()]
    public void ReadTestUtc()
    {
        var date = JsonSerializer.Deserialize<DateTimeOffset>(_jsonUtc, _serializerOptions);
        Assert.AreEqual(_dateUtc, date.ToUniversalTime());
    }


    [TestMethod()]
    public void WriteTestUtc()
    {
        var json = JsonSerializer.Serialize(_dateUtc, _serializerOptions);
        Assert.AreEqual(_jsonUtc, Regex.Unescape(json));
    }

    [TestMethod()]
    public void ReadTestLocal()
    {
        var date = JsonSerializer.Deserialize<DateTimeOffset>(_jsonLocal, _serializerOptions);
        Assert.AreEqual(_dateLocal, date);
    }


    [TestMethod()]
    public void WriteTestLocal()
    {
        var json = JsonSerializer.Serialize(_dateLocal, _serializerOptions);
        Assert.AreEqual(_jsonLocal, Regex.Unescape(json));
    }
}