using System;
using ISynergy.Framework.Geography.Tests;
using Xunit;

namespace ISynergy.Framework.Geography.Projection.Tests
{
    public class MercatorProjectionTests
    {
        [Fact]
        public void TestLoxodromeToOffice()
        {
            var proj = new EllipticalMercatorProjection();
            _ = proj.CalculatePath(
                Constants.MyHome,
                Constants.MyOffice,
                out var mercatorRhumDistance,
                out var bearing, 3);

            // The reference values are computed with 
            // http://onboardintelligence.com/RL_Lat1Long1Lat2Long2.aspx
            // (Set the high precision options in the dialog!)
            Assert.Equal(43233, Math.Round(mercatorRhumDistance));
            Assert.Equal(342.2, Math.Round(bearing.Degrees * 10) / 10);
        }
    }
}
