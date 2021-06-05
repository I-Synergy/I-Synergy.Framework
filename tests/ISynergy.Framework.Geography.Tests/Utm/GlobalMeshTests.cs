using System;
using ISynergy.Framework.Geography.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    /// <summary>
    /// Class GlobalMeshTests.
    /// </summary>
    [TestClass]
    public class GlobalMeshTests
    {
        /// <summary>
        /// Defines the test method TestAllCenters.
        /// </summary>
        [TestMethod]
        public void TestAllCenters()
        {
            var theMesh = new GlobalMesh(25000);
            for (long mesh = 0; mesh < theMesh.Count; mesh++)
            {
                var center = theMesh.CenterOf(mesh);
                var verify = theMesh.MeshNumber(center);
                Assert.AreEqual(mesh, verify);
            }
        }

        /// <summary>
        /// Defines the test method TestBoundingBox.
        /// </summary>
        [TestMethod]
        public void TestBoundingBox()
        {
            var theMesh = new GlobalMesh(1000);
            var nr = theMesh.MeshNumber(Constants.MyHome);
            var ll = theMesh.LowerLeft(nr);
            var lr = theMesh.LowerRight(nr);
            var ul = theMesh.UpperLeft(nr);
            var ur = theMesh.UpperRight(nr);
            Assert.AreEqual(ll.X, ul.X);
            Assert.AreEqual(lr.X, ur.X);
            Assert.AreEqual(ll.Y, lr.Y);
            Assert.AreEqual(ul.Y, ur.Y);
        }

        /// <summary>
        /// Defines the test method TestNeighborHood.
        /// </summary>
        [TestMethod]
        public void TestNeighborHood()
        {
            var theMesh = new GlobalMesh(1000);
            var nr = theMesh.MeshNumber(Constants.MyHome);
            var n0 = theMesh.Neighborhood(nr, 0);
            Assert.IsTrue(n0.Count == 1);
            Assert.AreEqual(nr, n0[0]);
            var n1 = theMesh.Neighborhood(nr, 1);
            Assert.AreEqual(8, n1.Count);
            var n2 = theMesh.Neighborhood(nr, 2);
            Assert.AreEqual(16, n2.Count);
            var n3 = theMesh.Neighborhood(nr, 3);
            Assert.AreEqual(24, n3.Count);
        }

        /// <summary>
        /// Defines the test method TestMeshSizeInMetersValidation.
        /// </summary>
        [TestMethod]
        public void TestMeshSizeInMetersValidation()
        {
            try
            {
                new GlobalMesh(0);
            }
            catch (ArgumentOutOfRangeException)
            { }

            try
            {
                new GlobalMesh(1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.IsTrue(false);
            }

            try
            {
                new GlobalMesh(2);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.IsTrue(false);
            }
        }
    }
}
