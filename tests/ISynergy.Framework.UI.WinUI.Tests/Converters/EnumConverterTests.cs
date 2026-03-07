using ISynergy.Framework.UI.Converters;
using ISynergy.Framework.UI.WinUI.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.UI.WinUI.Converters;

[TestClass]
public class EnumConverterTests
{
    [DataTestMethod]
    [DataRow(TestEnum.Value1, 1)]
    [DataRow(TestEnum.Value2, 2)]
    [DataRow(TestEnum.Value3, 3)]
    public void EnumToIntegerConverter_Convert_ReturnsIntValue(TestEnum enumeration, int id)
    {
        EnumToIntegerConverter converter = new EnumToIntegerConverter();
        var result = converter.Convert(enumeration, typeof(int), null!, null!);
        Assert.AreEqual(id, result);
    }

    [DataTestMethod]
    [DataRow(1, TestEnum.Value1)]
    [DataRow(2, TestEnum.Value2)]
    [DataRow(3, TestEnum.Value3)]
    public void EnumToIntegerConverter_ConvertBack_ReturnsEnumValue(int id, TestEnum enumeration)
    {
        EnumToIntegerConverter converter = new EnumToIntegerConverter();
        var result = converter.ConvertBack(id, typeof(TestEnum), null!, null!);
        Assert.AreEqual(enumeration, result);
    }
}
