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
            using var building = new Tree<Guid, Space>(buildingSpace);

            Assert.AreEqual(0, building.Children.Count);
            Assert.AreEqual(building.ParentKey, Guid.Empty);
        }

        [TestMethod]
        public void FindNodeTest()
        {
            using var building = new Tree<Guid, Space>(buildingSpace);
            using var storage = building.AddChild(storageSpace);
            using var bin = storage.AddChild(binSpace);

            var node = bin.FindNode(storage.Key);

            Assert.IsNotNull(node);
            Assert.AreEqual(storage, node);
        }

        [TestMethod]
        public void FindRootNodeTest()
        {
            using var building = new Tree<Guid, Space>(buildingSpace);
            using var storage = building.AddChild(storageSpace);
            using var bin = storage.AddChild(binSpace);

            var binRoot = bin.GetRootNode();

            Assert.IsNotNull(binRoot);
            Assert.AreEqual(building, binRoot);

            var storageRoot = bin.GetRootNode();

            Assert.IsNotNull(storageRoot);
            Assert.AreEqual(building, storageRoot);

            var buildingRoot = bin.GetRootNode();

            Assert.IsNotNull(buildingRoot);
            Assert.AreEqual(building, buildingRoot);
        }

        [TestMethod]
        public void AddChildNodes()
        {
            using var building = new Tree<Guid, Space>(buildingSpace);
            using var storage = building.AddChild(storageSpace);
            using var bin = storage.AddChild(binSpace);

            Assert.AreEqual(1, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(1, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);
            
            Assert.AreEqual(0, bin.Children.Count);
            Assert.AreSame(bin.Parent, storage);

            Assert.AreEqual(building.Key, storage.ParentKey);
            Assert.AreEqual(building.Data.Id, storage.Data.ParentId);

            Assert.AreEqual(storage.Key, bin.ParentKey);
            Assert.AreEqual(storage.Data.Id, bin.Data.ParentId);
        }

        [TestMethod]
        public void RemoveNodeBySettingParentToNull()
        {
            using var building = new Tree<Guid, Space>(buildingSpace);
            using var storage = building.AddChild(storageSpace);
            using var bin = storage.AddChild(binSpace);

            bin.SetParent(null);

            Assert.AreEqual(1, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(0, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.IsNull(bin.Parent);

            Assert.AreEqual(building.Key, storage.ParentKey);
            Assert.AreEqual(building.Data.Id, storage.Data.ParentId);

            Assert.AreEqual(Guid.Empty, bin.ParentKey);
            Assert.AreEqual(Guid.Empty, bin.Data.ParentId);
        }

        [TestMethod]
        public void SwapNodeParent()
        {
            using var building = new Tree<Guid, Space>(buildingSpace);
            using var storage = building.AddChild(storageSpace);
            using var bin = storage.AddChild(binSpace);
            using var kitchen = building.AddChild(kitchenSpace);

            bin.SetParent(kitchen);

            Assert.AreEqual(2, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(0, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(1, kitchen.Children.Count);
            Assert.AreSame(kitchen.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.AreSame(bin.Parent, kitchen);

            Assert.AreEqual(building.Key, storage.ParentKey);
            Assert.AreEqual(building.Data.Id, storage.Data.ParentId);

            Assert.AreNotEqual(storage.Key, bin.ParentKey);
            Assert.AreNotEqual(storage.Data.Id, bin.Data.ParentId);

            Assert.AreEqual(kitchen.Key, bin.ParentKey);
            Assert.AreEqual(kitchen.Data.Id, bin.Data.ParentId);
        }

        [TestMethod]
        public void RemoveNodeByCallingRemove()
        {
            using var building = new Tree<Guid, Space>(buildingSpace);
            using var storage = building.AddChild(storageSpace);
            using var bin = storage.AddChild(binSpace);
            
            storage.RemoveChild(bin);

            Assert.AreEqual(1, building.Children.Count);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(0, storage.Children.Count);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Children.Count);
            Assert.IsNull(bin.Parent);

            Assert.AreEqual(building.Key, storage.ParentKey);
            Assert.AreEqual(building.Data.Id, storage.Data.ParentId);

            Assert.AreEqual(Guid.Empty, bin.ParentKey);
            Assert.AreEqual(Guid.Empty, bin.Data.ParentId);
        }

        [TestMethod]
        public void ComplexInitialState()
        {
            var property = new Tree<Guid, Space>(propertySpace);
            var buildingA = property.AddChild(buildingASpace);
            var laundryRoom = buildingA.AddChild(laundryRoomSpace);
            var bathroomA = buildingA.AddChild(bathroomASpace);
            var storageA = buildingA.AddChild(storageASpace);
            var buildingB = property.AddChild(buildingBSpace);
            var bathroomB = buildingB.AddChild(bathroomBSpace);
            var storageB = buildingB.AddChild(storageBSpace);
            var meetingRoom = buildingB.AddChild(meetingRoomSpace);
            var meetingRoomCloset = meetingRoom.AddChild(meetingRoomClosetSpace);
            
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
            var property = new Tree<Guid, Space>(propertySpace);
            var buildingA = property.AddChild(buildingASpace);

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
