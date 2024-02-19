using ISynergy.Framework.Geography.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Projection.Tests;

/// <summary>
/// Class MercatorProjectionTests.
/// </summary>
[TestClass]
public class MercatorProjectionTests
{
    /// <summary>
    /// Defines the test method TestLoxodromeToOffice.
    /// </summary>
    [TestMethod]
    public void TestLoxodromeToOffice()
    {
        EllipticalMercatorProjection proj = new();
        _ = proj.CalculatePath(
            Constants.MyHome,
            Constants.MyOffice,
            out double mercatorRhumDistance,
            out Common.Angle bearing, 3);

        // The reference values are computed with 
        // http://onboardintelligence.com/RL_Lat1Long1Lat2Long2.aspx
        // (Set the high precision options in the dialog!)
        Assert.AreEqual(43233, Math.Round(mercatorRhumDistance));
        Assert.AreEqual(342.2, Math.Round(bearing.Degrees * 10) / 10);
    }
}
