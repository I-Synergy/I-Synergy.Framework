using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Physics.Tests.Services
{
    /// <summary>
    /// Test class for the UnitConversionService
    /// </summary>
    [TestClass()]
    public class UnitConversionServiceTests
    {
        private IUnitConversionService _unitConversionService;

        /// <summary>
        /// Initializes tests
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            var languageServiceMock = Mock.Of<ILanguageService>();
            _unitConversionService = new UnitConversionService(languageServiceMock);
        }

        /// <summary>
        /// Tests if UnitConversionService is constructed correctly.
        /// </summary>
        [TestMethod()]
        public void UnitConversionServiceTest()
        {
            Assert.IsNotNull(_unitConversionService);
            Assert.IsNotNull(_unitConversionService.Units);
            Assert.IsTrue(_unitConversionService.Units.Count > 0);
        }

        /// <summary>
        /// Tests if UnitConversionService is constructed correctly.
        /// </summary>
        [TestMethod()]
        public void UnitConversionServiceGetSIUnitsTest()
        {
            Assert.IsTrue(_unitConversionService.Units.Where(q => q is SIUnit).Count() == 15);
        }

        /// <summary>
        /// Tests for checking correct results from converter.
        /// </summary>
        /// <param name="sourceSymbol"></param>
        /// <param name="value"></param>
        /// <param name="targetSymbol"></param>
        /// <param name="result"></param>
        [DataTestMethod()]
        [DataRow("nm", 1000000000, "m", 1)]
        [DataRow("μm", 1000000, "m", 1)]
        [DataRow("mm", 1000, "m", 1)]
        [DataRow("cm", 100, "m", 1)]
        [DataRow("dm", 10, "m", 1)]
        [DataRow("m", 1, "mm", 1000)]
        [DataRow("cm", 1, "mm", 10)]
        [DataRow("m", 1, "m", 1)]
        [DataRow("mm", 10, "mm", 10)]
        [DataRow("in", 1, "cm", 2.54000508001016)]
        [DataRow("ft", 1, "m", 0.30480060960121924)]
        [DataRow("yd", 1, "m", 0.9144018288036577)]
        [DataRow("min", 1, "s", 60)]
        [DataRow("h", 1, "min", 60)]
        [DataRow("d", 1, "h", 24)]
        [DataRow("t", 1, "kg", 1000)]
        [DataRow("C", 1, "K", 274.15)]
        [DataRow("K", 0, "C", -273.15)]
        [DataRow("F", 100, "K", 310.9277777777778)]
        [DataRow("K", 300, "F", 80.32999999999998)]
        [DataRow("C", 35, "F", 94.99999999999994)]
        [DataRow("F", 86, "C", 30.000000000000057)]
        [DataRow("kgf", 1, "N", 9.806650)]
        [DataRow("gf", 1000, "N", 9.806650)]
        [DataRow("au", 1, "m", 149597870700)]
        [DataRow("L", 1000, "m3", 1)]
        public void UnitConvertTest(string sourceSymbol, double value, string targetSymbol, double result)
        {
            var source = _unitConversionService.Units.Where(q => q.Symbol.Equals(sourceSymbol)).Single();
            var target = _unitConversionService.Units.Where(q => q.Symbol.Equals(targetSymbol)).Single();

            Assert.AreEqual(result, _unitConversionService.Convert(source, value, target));
        }

        /// <summary>
        /// Tests for checking correct results from converter.
        /// </summary>
        /// <param name="sourceSymbol"></param>
        /// <param name="value"></param>
        /// <param name="targetSymbol"></param>
        /// <param name="result"></param>
        [DataTestMethod()]
        [DataRow("nm", 1000000000, "m", 1)]
        [DataRow("μm", 1000000, "m", 1)]
        [DataRow("mm", 1000, "m", 1)]
        [DataRow("cm", 100, "m", 1)]
        [DataRow("dm", 10, "m", 1)]
        [DataRow("m", 1, "mm", 1000)]
        [DataRow("cm", 1, "mm", 10)]
        [DataRow("m", 1, "m", 1)]
        [DataRow("mm", 10, "mm", 10)]
        [DataRow("in", 1, "cm", 2.54000508001016)]
        [DataRow("ft", 1, "m", 0.30480060960121924)]
        [DataRow("yd", 1, "m", 0.9144018288036577)]
        [DataRow("min", 1, "s", 60)]
        [DataRow("h", 1, "min", 60)]
        [DataRow("d", 1, "h", 24)]
        [DataRow("t", 1, "kg", 1000)]
        [DataRow("C", 1, "K", 274.15)]
        [DataRow("K", 0, "C", -273.15)]
        [DataRow("F", 100, "K", 310.9277777777778)]
        [DataRow("K", 300, "F", 80.32999999999998)]
        [DataRow("C", 35, "F", 94.99999999999994)]
        [DataRow("F", 86, "C", 30.000000000000057)]
        [DataRow("kgf", 1, "N", 9.806650)]
        [DataRow("gf", 1000, "N", 9.806650)]
        [DataRow("au", 1, "m", 149597870700)]
        [DataRow("L", 1000, "m3", 1)]
        public void SymbolConvertTest(string sourceSymbol, double value, string targetSymbol, double result)
        {
            Assert.AreEqual(result, _unitConversionService.Convert(sourceSymbol, value, targetSymbol));
        }

        /// <summary>
        /// Cleanup after tests
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            _unitConversionService = null;
        }
    }
}