using System;
using ISynergy.Framework.Geography.Tests;
using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    public class GlobalMeshTests
    {
        [Fact]
        public void TestAllCenters()
        {
            var theMesh = new GlobalMesh(25000);
            for (long mesh = 0; mesh < theMesh.Count; mesh++)
            {
                var center = theMesh.CenterOf(mesh);
                var verify = theMesh.MeshNumber(center);
                Assert.Equal(mesh, verify);
            }
        }

        [Fact]
        public void TestBoundingBox()
        {
            var theMesh = new GlobalMesh(1000);
            var nr = theMesh.MeshNumber(Constants.MyHome);
            var ll = theMesh.LowerLeft(nr);
            var lr = theMesh.LowerRight(nr);
            var ul = theMesh.UpperLeft(nr);
            var ur = theMesh.UpperRight(nr);
            Assert.Equal(ll.X, ul.X);
            Assert.Equal(lr.X, ur.X);
            Assert.Equal(ll.Y, lr.Y);
            Assert.Equal(ul.Y, ur.Y);
        }

        [Fact]
        public void TestNeighborHood()
        {
            var theMesh = new GlobalMesh(1000);
            var nr = theMesh.MeshNumber(Constants.MyHome);
            var n0 = theMesh.Neighborhood(nr, 0);
            Assert.Single(n0);
            Assert.Equal(nr, n0[0]);
            var n1 = theMesh.Neighborhood(nr, 1);
            Assert.Equal(8, n1.Count);
            var n2 = theMesh.Neighborhood(nr, 2);
            Assert.Equal(16, n2.Count);
            var n3 = theMesh.Neighborhood(nr, 3);
            Assert.Equal(24, n3.Count);
        }

        [Fact]
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
                Assert.True(false);
            }

            try
            {
                new GlobalMesh(2);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.True(false);
            }
        }
    }
}
