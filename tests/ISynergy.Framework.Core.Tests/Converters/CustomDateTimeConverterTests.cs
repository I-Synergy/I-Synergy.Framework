﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Converters.Tests;

[TestClass()]
public class CustomDateTimeConverterTests
{
    private readonly DateTime _dateUnspecified;
    private readonly DateTime _dateUtc;
    private readonly DateTime _dateLocal;

    private readonly string _jsonUnspecified;
    private readonly string _jsonUtc;
    private readonly string _jsonLocal;


    private readonly JsonSerializerOptions _serializerOptions;

    public CustomDateTimeConverterTests()
    {
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.Converters.Add(new CustomDateTimeJsonConverter("yyyyMMddHHmmK"));

        _dateUnspecified = new DateTime(1975, 10, 29, 15, 0, 0, DateTimeKind.Unspecified);
        _dateUtc = new DateTime(1975, 10, 29, 15, 0, 0, DateTimeKind.Utc);
        _dateLocal = new DateTime(1975, 10, 29, 15, 0, 0, DateTimeKind.Local);

        _jsonUnspecified = Regex.Unescape(JsonSerializer.Serialize("197510291500", _serializerOptions));
        _jsonUtc = Regex.Unescape(JsonSerializer.Serialize("197510291500Z", _serializerOptions));
        _jsonLocal = Regex.Unescape(JsonSerializer.Serialize("197510291500+01:00", _serializerOptions));
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
}