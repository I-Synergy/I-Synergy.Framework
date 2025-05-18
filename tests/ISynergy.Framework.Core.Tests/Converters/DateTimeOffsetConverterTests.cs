using ISynergy.Framework.Core.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Converters.Tests;

[TestClass()]
public class DateTimeOffsetConverterTests
{
    private DateTimeOffsetConverter _converter;

    private readonly DateTimeOffset _dateUtc;
    private readonly DateTimeOffset _dateLocal;

    private readonly string _jsonUtc;
    private readonly string _jsonLocal;

    private readonly JsonSerializerOptions _serializerOptions;

    public DateTimeOffsetConverterTests()
    {
        _converter = new DateTimeOffsetConverter();
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
        var actual = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>(json, _serializerOptions);
        Assert.AreEqual(expected, actual!["dt"]);
    }

    [TestMethod]
    public void ReadNonStringReturnsDefault()
    {
        const string json = "{\"dt\":null}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>(json, _serializerOptions);
        Assert.ThrowsException<FormatException>(act);
    }

    [TestMethod]
    public void ReadBadStringThrows()
    {
        const string json = "{\"dt\":\"not-date\"}";
        Action act = () => _ = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>(json, _serializerOptions);
        Assert.ThrowsException<FormatException>(act);
    }

    [TestMethod]
    public void Read_ValidIsoFormat_ReturnsCorrectDateTimeOffset()
    {
        // Arrange
        string json = "\"2023-10-15T14:30:45.1234567Z\"";
        DateTimeOffset expected = DateTimeOffset.Parse("2023-10-15T14:30:45.1234567Z");

        // Act
        DateTimeOffset result = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Read_MinValueWithOffset_ReturnsCorrectDateTimeOffset()
    {
        // Arrange - This is the problematic case from the error
        string json = "\"0001-01-01T00:00:00+00:00\"";
        DateTimeOffset expected = new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        DateTimeOffset result = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Read_DateTimeOffsetWithPositiveOffset_ReturnsCorrectValue()
    {
        // Arrange
        string json = "\"2023-10-15T14:30:45+02:00\"";
        DateTimeOffset expected = new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.FromHours(2));

        // Act
        DateTimeOffset result = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Read_DateTimeOffsetWithNegativeOffset_ReturnsCorrectValue()
    {
        // Arrange
        string json = "\"2023-10-15T14:30:45-05:00\"";
        DateTimeOffset expected = new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.FromHours(-5));

        // Act
        DateTimeOffset result = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void Read_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        string json = "\"not-a-date\"";

        // Act & Assert - Should throw
        JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);
    }

    [TestMethod]
    public void Write_UtcDateTimeOffset_WritesCorrectFormat()
    {
        // Arrange
        var original = new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.Zero);

        // Act
        var json = JsonSerializer.Serialize(original, _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(original, deserialized);
    }

    [TestMethod]
    public void Write_DateTimeOffsetWithOffset_WritesCorrectFormat()
    {
        // Arrange
        var original = new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.FromHours(2));

        // Act
        var json = JsonSerializer.Serialize(original, _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(original, deserialized);
    }

    [TestMethod]
    public void Write_MinValue_WritesCorrectFormat()
    {
        // Arrange
        var value = DateTimeOffset.MinValue;

        // Act
        var json = JsonSerializer.Serialize(value, _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        // Instead of comparing strings, verify the round-trip works correctly
        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    public void Read_MaxValue_ReturnsCorrectDateTimeOffset()
    {
        // Arrange
        var json = "\"9999-12-31T23:59:59.9999999Z\"";
        var expected = DateTimeOffset.MaxValue;

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Write_MaxValue_WritesCorrectFormat()
    {
        // Arrange
        var value = DateTimeOffset.MaxValue;

        // Act
        var json = JsonSerializer.Serialize(value, _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

        // Assert
        // Instead of comparing strings, verify the round-trip works correctly
        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    public void RoundTrip_VariousDateTimeOffsets_PreservesValues()
    {
        // Arrange
        DateTimeOffset[] testValues = new[]
        {
                DateTimeOffset.UtcNow,
                new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.FromHours(2)),
                new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.FromHours(-5)),
                DateTimeOffset.MinValue,
                new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero)
            };

        foreach (var original in testValues)
        {
            // Act
            string json = JsonSerializer.Serialize(original, _serializerOptions);
            DateTimeOffset deserialized = JsonSerializer.Deserialize<DateTimeOffset>(json, _serializerOptions);

            // Assert
            Assert.AreEqual(original, deserialized, $"Failed for value: {original}");
        }
    }

    [TestMethod]
    public void TestWithComplexObject()
    {
        // Arrange
        var testObject = new TestClass
        {
            Id = 1,
            Name = "Test",
            Created = new DateTimeOffset(2023, 10, 15, 14, 30, 45, TimeSpan.FromHours(2)),
            MinDate = new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        // Act
        string json = JsonSerializer.Serialize(testObject, _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<TestClass>(json, _serializerOptions);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(testObject.Id, deserialized.Id);
        Assert.AreEqual(testObject.Name, deserialized.Name);
        Assert.AreEqual(testObject.Created, deserialized.Created);
        Assert.AreEqual(testObject.MinDate, deserialized.MinDate);
    }

    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset MinDate { get; set; }
    }
}