using ISynergy.Framework.Mathematics.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests
{


    [TestClass]
    public class Point3Test
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod]
        public void CollinearTest()
        {
            {
                Point3 p1 = new(0, 0, 0);
                Point3 p2 = new(0, 0, 1);
                Point3 p3 = new(0, 0, 2);

                bool expected = true;
                bool actual = Point3.Collinear(p1, p2, p3);

                Assert.AreEqual(expected, actual);
            }

            {
                Point3 p1 = new(1, 0, 0);
                Point3 p2 = new(0, 2, 1);
                Point3 p3 = new(0, 0, 2);

                bool expected = false;
                bool actual = Point3.Collinear(p1, p2, p3);

                Assert.AreEqual(expected, actual);
            }

            {
                Point3 p1 = new(134, 268, 402);
                Point3 p2 = new(329, 658, 98);
                Point3 p3 = new(125, 250, 375);

                bool expected = false;
                bool actual = Point3.Collinear(p1, p2, p3);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
