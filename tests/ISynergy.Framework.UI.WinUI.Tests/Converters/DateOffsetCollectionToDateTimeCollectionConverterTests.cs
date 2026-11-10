using ISynergy.Framework.UI.Converters;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.UI.WinUI.Converters;

/// <summary>
/// Tests for <see cref="DateOffsetCollectionToDateTimeCollectionConverter"/>, the remaining
/// date/time adapter in the WinUI converter head.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DateOffsetCollectionToDateTimeCollectionConverterTests
{
    [TestMethod]
    public void Convert_WithDateTimeOffsetCollection_ReturnsLocalDateTimes()
    {
        var converter = new DateOffsetCollectionToDateTimeCollectionConverter();
        var source = new ObservableCollection<DateTimeOffset>
        {
            new(2024, 5, 17, 13, 45, 30, TimeSpan.Zero),
            new(2024, 5, 18, 8, 0, 0, TimeSpan.Zero)
        };

        var result = converter.Convert(source, typeof(ObservableCollection<DateTime>), null!, null!);

        var dates = result as ObservableCollection<DateTime>;
        Assert.IsNotNull(dates);
        CollectionAssert.AreEqual(
            source.Select(item => item.ToLocalTime().DateTime).ToList(),
            dates);
    }

    [TestMethod]
    public void Convert_WithUnsupportedValue_ReturnsEmptyCollection()
    {
        var converter = new DateOffsetCollectionToDateTimeCollectionConverter();

        var result = converter.Convert("not a collection", typeof(ObservableCollection<DateTime>), null!, null!);

        var dates = result as ObservableCollection<DateTime>;
        Assert.IsNotNull(dates);
        Assert.IsEmpty(dates);
    }

    [TestMethod]
    public void ConvertBack_WithDateTimeCollection_ReturnsLocalDateTimeOffsets()
    {
        var converter = new DateOffsetCollectionToDateTimeCollectionConverter();
        var localDate = new DateTime(2024, 5, 17, 13, 45, 30, DateTimeKind.Local);
        var source = new ObservableCollection<DateTime> { localDate };

        var result = converter.ConvertBack(source, typeof(ObservableCollection<DateTimeOffset>), null!, null!);

        var offsets = result as ObservableCollection<DateTimeOffset>;
        Assert.IsNotNull(offsets);
        Assert.HasCount(1, offsets);
        Assert.AreEqual(new DateTimeOffset(DateTime.SpecifyKind(localDate, DateTimeKind.Local)), offsets[0]);
    }

    [TestMethod]
    public void ConvertBack_WithStringCollection_ParsesInvariantCulture()
    {
        var converter = new DateOffsetCollectionToDateTimeCollectionConverter();
        var source = new ObservableCollection<string> { "2024-05-17T13:45:30" };

        var result = converter.ConvertBack(source, typeof(ObservableCollection<DateTimeOffset>), null!, null!);

        var offsets = result as ObservableCollection<DateTimeOffset>;
        Assert.IsNotNull(offsets);
        Assert.HasCount(1, offsets);
        Assert.AreEqual(2024, offsets[0].Year);
        Assert.AreEqual(13, offsets[0].Hour);
    }

    [TestMethod]
    public void ConvertBack_WithUnsupportedValue_ReturnsEmptyCollection()
    {
        var converter = new DateOffsetCollectionToDateTimeCollectionConverter();

        var result = converter.ConvertBack(42, typeof(ObservableCollection<DateTimeOffset>), null!, null!);

        var offsets = result as ObservableCollection<DateTimeOffset>;
        Assert.IsNotNull(offsets);
        Assert.IsEmpty(offsets);
    }
}
