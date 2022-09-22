using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Tests.Data;
using ISynergy.Framework.Core.Tests.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Core.Collections.Tests
{
    [TestClass]
    public class TreeTests
    {
        private readonly Space propertySpace = new() { Name = "Industrial Property", SquareFeet = 150000 };
        private readonly Space buildingSpace = new() { Type = SpaceTypes.Building, Name = "Science Building", SquareFeet = 30000 };
        private readonly Space storageSpace = new() { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 1200 };
        private readonly Space binSpace = new() { Type = SpaceTypes.Inventory, Name = "Bin", SquareFeet = 4 };
        private readonly Space kitchenSpace = new() { Type = SpaceTypes.Room, Name = "Kitchen", SquareFeet = 1800 };
        private readonly Space buildingASpace = new() { Name = "Building A", SquareFeet = 900 };
        private readonly Space laundryRoomSpace = new() { Type = SpaceTypes.Room, Name = "Laundry Room", SquareFeet = 300 };
        private readonly Space bathroomASpace = new() { Name = "Bathroom", SquareFeet = 150 };
        private readonly Space storageASpace = new() { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 450 };
        private readonly Space buildingBSpace = new() { Name = "Building B", SquareFeet = 50000 };
        private readonly Space bathroomBSpace = new() { Type = SpaceTypes.Room, Name = "Bathroom", SquareFeet = 50000 };
        private readonly Space storageBSpace = new() { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 500 };
        private readonly Space meetingRoomSpace = new() { Type = SpaceTypes.Room, Name = "Meeting Room", SquareFeet = 1600 };
        private readonly Space meetingRoomClosetSpace = new() { Type = SpaceTypes.Inventory, Name = "Meeting Room Closet", SquareFeet = 150 };

        [TestMethod]
        public void SingleNode()
        {
            using var building = new Tree<Guid, Space>(buildingSpace.Id, buildingSpace);
            //var x = building.Flatten();

            Assert.AreEqual(0, building.Children.Count);
            Assert.IsNull(building.Parent);
        }

        [TestMethod]
        public void AddChildNodes()
        {
            using var building = new Tree<Guid, Space>(buildingSpace.Id, buildingSpace);
            using var storage = building.AddChild(new TreeNode<Guid, Space>(storageSpace.Id, storageSpace));
            using var bin = storage.AddChild(new TreeNode<Guid, Space>(binSpace.Id, binSpace));

            //var x = building.Flatten();
            //var y = building.FlattenList();
            //var z = building.FlattenValuesList();
            //var aa = y.ToTree();
            //var bb = building.Equals(aa);

            //try
            //{
            //    var json = JsonSerializer.Serialize(building, new JsonSerializerOptions
            //    {
            //        WriteIndented = true,
            //        PropertyNameCaseInsensitive = true,
            //        ReferenceHandler = ReferenceHandler.IgnoreCycles
            //    });

            //    var test = JsonSerializer.Deserialize<Tree<Guid, Space>>(json);
            //    var same = test.Equals(building);

            //}
            //catch (System.Exception ex)
            //{
            //    throw ex;
            //}

            Assert.AreEqual(1, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(1, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.AreSame(bin.Parent, storage);
        }

        [TestMethod]
        public void RemoveNodeBySettingParentToNull()
        {
            using var building = new Tree<Guid, Space>(buildingSpace.Id, buildingSpace);
            using var storage = building.AddChild(new TreeNode<Guid, Space>(storageSpace.Id, storageSpace));
            using var bin = storage.AddChild(new TreeNode<Guid, Space>(binSpace.Id, binSpace));

            bin.SetParent(null);

            Assert.AreEqual(1, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(0, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.IsNull(bin.Parent);
        }

        [TestMethod]
        public void SwapNodeParent()
        {
            using var building = new Tree<Guid, Space>(buildingSpace.Id, buildingSpace);
            using var storage = building.AddChild(new TreeNode<Guid, Space>(storageSpace.Id, storageSpace));
            using var bin = storage.AddChild(new TreeNode<Guid, Space>(binSpace.Id, binSpace));
            using var kitchen = building.AddChild(new TreeNode<Guid, Space>(kitchenSpace.Id, kitchenSpace));

            bin.SetParent(kitchen);

            Assert.AreEqual(2, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(0, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(1, kitchen.Children.Count);
            Assert.AreSame(kitchen.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.AreSame(bin.Parent, kitchen);
        }

        [TestMethod]
        public void RemoveNodeByCallingRemove()
        {
            using var building = new Tree<Guid, Space>(buildingSpace.Id, buildingSpace);
            using var storage = building.AddChild(new TreeNode<Guid, Space>(storageSpace.Id, storageSpace));
            using var bin = storage.AddChild(new TreeNode<Guid, Space>(binSpace.Id, binSpace));
            
            storage.RemoveChild(bin);

            Assert.AreEqual(1, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(0, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.IsNull(bin.Parent);
        }

        [TestMethod]
        public void ComplexInitialState()
        {
            var property = new Tree<Guid, Space>(propertySpace.Id, propertySpace);
            var buildingA = property.AddChild(new TreeNode<Guid, Space>(buildingASpace.Id, buildingASpace));
            var laundryRoom = buildingA.AddChild(new TreeNode<Guid, Space>(laundryRoomSpace.Id, laundryRoomSpace));
            var bathroomA = buildingA.AddChild(new TreeNode<Guid, Space>(bathroomASpace.Id, bathroomASpace));
            var storageA = buildingA.AddChild(new TreeNode<Guid, Space>(storageASpace.Id, storageASpace));
            var buildingB = property.AddChild(new TreeNode<Guid, Space>(buildingBSpace.Id, buildingBSpace));
            var bathroomB = buildingB.AddChild(new TreeNode<Guid, Space>(bathroomBSpace.Id, bathroomBSpace));
            var storageB = buildingB.AddChild(new TreeNode<Guid, Space>(storageBSpace.Id, storageBSpace));
            var meetingRoom = buildingB.AddChild(new TreeNode<Guid, Space>(meetingRoomSpace.Id, meetingRoomSpace));
            var meetingRoomCloset = meetingRoom.AddChild(new TreeNode<Guid, Space>(meetingRoomClosetSpace.Id, meetingRoomClosetSpace));
            
            var x = property.Flatten();
            var y = property.FlattenList();
            var z = property.FlattenDataList();

            Assert.AreEqual(2, property.Children.Count);
            Assert.IsNull(property.Parent);

            Assert.AreEqual(3, buildingA.Children.Count);
            Assert.AreSame(property, buildingA.Parent);

            Assert.AreEqual(0, laundryRoom.Children.Count);
            Assert.AreSame(buildingA, laundryRoom.Parent);

            Assert.AreEqual(0, bathroomA.Children.Count);
            Assert.AreSame(buildingA, bathroomA.Parent);

            Assert.AreEqual(0, storageA.Children.Count);
            Assert.AreSame(buildingA, storageA.Parent);

            Assert.AreEqual(3, buildingB.Children.Count);
            Assert.AreSame(property, buildingB.Parent);

            Assert.AreEqual(0, bathroomB.Children.Count);
            Assert.AreSame(buildingB, bathroomB.Parent);

            Assert.AreEqual(0, storageB.Children.Count);
            Assert.AreSame(buildingB, storageB.Parent);

            Assert.AreEqual(1, meetingRoom.Children.Count);
            Assert.AreSame(buildingB, meetingRoom.Parent);

            Assert.AreEqual(0, meetingRoomCloset.Children.Count);
            Assert.AreSame(meetingRoom, meetingRoomCloset.Parent);
        }

        [TestMethod]
        public void IncrementalChanges()
        {
            var property = new Tree<Guid, Space>(propertySpace.Id, propertySpace);
            var buildingA = property.AddChild(new TreeNode<Guid, Space>(buildingASpace.Id, buildingASpace));

            Assert.AreEqual(1, property.Children.Count);
            Assert.AreEqual(0, buildingA.Children.Count);

            var parentOfBuildingA = buildingA.Parent;
            Assert.IsNotNull(parentOfBuildingA);
            Assert.AreEqual("Industrial Property", parentOfBuildingA.Data.Name);

            property.Children.Remove(buildingA);

            Assert.AreEqual(0, property.Children.Count);
            Assert.AreEqual(0, buildingA.Children.Count);
        }
    }
}
