using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class MetricTests
    {
        [Fact]
        [Trait(nameof(Metric.Surface), Test.Unit)]
        public void Surface100Test()
        {
            decimal result = Metric.Surface(10, 10);
            Assert.Equal(100, result);
        }

        [Fact]
        [Trait(nameof(Metric.Surface), Test.Unit)]
        public void SurfaceDecimalTest()
        {
            decimal result = Metric.Surface(2.5m, 7.3m);
            Assert.Equal(18.25m, result);
        }

        [Fact]
        [Trait(nameof(Metric.Volume), Test.Unit)]
        public void Volume1000Test()
        {
            decimal result = Metric.Volume(10, 10, 10);
            Assert.Equal(1000, result);
        }

        [Fact]
        [Trait(nameof(Metric.Volume), Test.Unit)]
        public void VolumeDecimalTest()
        {
            decimal result = Metric.Volume(2.5m, 7.3m, 5.7m);
            Assert.Equal(104.025m, result);
        }

        [Fact]
        [Trait(nameof(Metric.Density), Test.Unit)]
        public void DensityTest()
        {
            decimal result = Metric.Density(1000m, 100m);
            Assert.Equal(0.1m, result);
        }
    }
}