using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Tests.Data;
using ISynergy.Framework.Core.Tests.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ISynergy.Framework.Core.Collections.Tests
{
    [TestClass]
    public class TreeTests
    {
        [TestMethod]
        public void SingleNode()
        {
            var building = new Tree<Space>(new Space { Type = SpaceTypes.Building, Name = "Science Building", SquareFeet = 30000 });
            //var x = building.Flatten();

            Assert.AreEqual(0, building.Height);
            Assert.AreEqual(0, building.Depth);
            Assert.AreEqual(0, building.Ancestors.Count());
            Assert.AreEqual(0, building.Descendants.Count());
            Assert.AreEqual(0, building.ChildNodes.Count());
            Assert.IsNull(building.Parent);
        }

        [TestMethod]
        public void DescendantChangedEvent()
        {
            var eventList = new List<string>();

            var building = new Tree<Space>(new Space { Type = SpaceTypes.Building, Name = "Building", SquareFeet = 30000 });
            building.DescendantChanged += (changeType, node) => eventList.Add("Building: " + changeType + " " + ((Space)node.Data).Name);

            var storage = building.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 1200 }));
            storage.DescendantChanged += (changeType, node) => eventList.Add("Storage: " + changeType + " " + ((Space)node.Data).Name);

            var bin = storage.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Bin", SquareFeet = 4 }));
            bin.DescendantChanged += (changeType, node) => eventList.Add("Bin: " + changeType + " " + ((Space)node.Data).Name);

            var envelope = bin.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Envelope", SquareFeet = .001 }));
            envelope.DescendantChanged += (changeType, node) => eventList.Add("Envelope: " + changeType + " " + ((Space)node.Data).Name);

            var x = building.Flatten();

            var expected = new List<string>
                {
                    "Building: NodeAdded Storage",
                    "Storage: NodeAdded Bin",
                    "Building: NodeAdded Bin",
                    "Bin: NodeAdded Envelope",
                    "Storage: NodeAdded Envelope",
                    "Building: NodeAdded Envelope"
                };

            Assert.AreEqual(string.Join(",", expected), string.Join(",", eventList));
        }

        [TestMethod]
        public void AddChildNodes()
        {
            var building = new Tree<Space>(new Space { Name = "Science Building", SquareFeet = 30000 });
            var storage = building.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 1200 }));
            var bin = storage.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Bin", SquareFeet = 4 }));

            //var x = building.Flatten();
            //var y = building.FlattenList();
            //var z = building.FlattenValuesList();
            //var aa = y.ToTree();
            //var bb = building.Equals(aa);

            //try
            //{
            //    var memoryStream = new MemoryStream();
            //    //var formatter = new BinaryFormatter();
            //    //formatter.Serialize(memoryStream, building);
            //    //memoryStream.Seek(0, 0);



            //    var serializer = new XmlSerializer(typeof(Tree<Space>));

            //    using (var writer = new StreamWriter(memoryStream))
            //    {
            //        serializer.Serialize(writer, building);
            //    }

            //    var result = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            //}
            //catch (System.Exception ex)
            //{
            //    throw ex;
            //}

            //try
            //{
            //    var json = JsonSerializer.Serialize(building, new JsonSerializerOptions
            //    {
            //        WriteIndented = true,
            //        PropertyNameCaseInsensitive = true,
            //        ReferenceHandler = ReferenceHandler.IgnoreCycles
            //    });

            //    var test = JsonSerializer.Deserialize(json, typeof(object));
            //    var same = test.Equals(building);

            //}
            //catch (System.Exception ex)
            //{
            //    throw ex;
            //}

            Assert.AreEqual(0, building.Ancestors.Count());
            Assert.AreEqual(1, building.Children.Count);
            Assert.AreEqual(1, building.ChildNodes.Count());
            Assert.AreEqual(2, building.Descendants.Count());
            Assert.AreEqual(2, building.Height);
            Assert.AreEqual(0, building.Depth);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(1, storage.Ancestors.Count());
            Assert.AreEqual(1, storage.Children.Count);
            Assert.AreEqual(1, storage.ChildNodes.Count());
            Assert.AreEqual(1, storage.Descendants.Count());
            Assert.AreEqual(1, storage.Height);
            Assert.AreEqual(1, storage.Depth);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(2, bin.Ancestors.Count());
            Assert.AreEqual(0, bin.Children.Count);
            Assert.AreEqual(0, bin.ChildNodes.Count());
            Assert.AreEqual(0, bin.Descendants.Count());
            Assert.AreEqual(0, bin.Height);
            Assert.AreEqual(2, bin.Depth);
            Assert.AreSame(bin.Parent, storage);
        }

        [TestMethod]
        public void RemoveNodeBySettingParentToNull()
        {
            var building = new Tree<Space>(new Space { Name = "Science Building", SquareFeet = 30000 });
            var storage = building.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 1200 }));
            var bin= storage.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Bin", SquareFeet = 4 }));

            bin.Parent = null;

            Assert.AreEqual(0, building.Ancestors.Count());
            Assert.AreEqual(1, building.ChildNodes.Count());
            Assert.AreEqual(1, building.Descendants.Count());
            Assert.AreEqual(1, building.Height);
            Assert.AreEqual(0, building.Depth);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(1, storage.Ancestors.Count());
            Assert.AreEqual(0, storage.ChildNodes.Count());
            Assert.AreEqual(0, storage.Descendants.Count());
            Assert.AreEqual(0, storage.Height);
            Assert.AreEqual(1, storage.Depth);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Ancestors.Count());
            Assert.AreEqual(0, bin.ChildNodes.Count());
            Assert.AreEqual(0, bin.Descendants.Count());
            Assert.AreEqual(0, bin.Height);
            Assert.AreEqual(0, bin.Depth);
            Assert.IsNull(bin.Parent);
        }

        [TestMethod]
        public void SwapNodeParent()
        {
            var building = new Tree<Space>(new Space { Name = "Science Building", SquareFeet = 30000 });
            var storage = building.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 1200 }));
            var kitchen = building.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Kitchen", SquareFeet = 1800 }));
            var bin= storage.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Bin", SquareFeet = 4 }));

            bin.Parent = kitchen;

            Assert.AreEqual(0, building.Ancestors.Count());
            Assert.AreEqual(2, building.ChildNodes.Count());
            Assert.AreEqual(3, building.Descendants.Count());
            Assert.AreEqual(2, building.Height);
            Assert.AreEqual(0, building.Depth);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(1, storage.Ancestors.Count());
            Assert.AreEqual(0, storage.ChildNodes.Count());
            Assert.AreEqual(0, storage.Descendants.Count());
            Assert.AreEqual(0, storage.Height);
            Assert.AreEqual(1, storage.Depth);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(1, kitchen.Ancestors.Count());
            Assert.AreEqual(1, kitchen.ChildNodes.Count());
            Assert.AreEqual(1, kitchen.Descendants.Count());
            Assert.AreEqual(1, kitchen.Height);
            Assert.AreEqual(1, kitchen.Depth);
            Assert.AreSame(kitchen.Parent, building);

            Assert.AreEqual(2, bin.Ancestors.Count());
            Assert.AreEqual(0, bin.ChildNodes.Count());
            Assert.AreEqual(0, bin.Descendants.Count());
            Assert.AreEqual(0, bin.Height);
            Assert.AreEqual(2, bin.Depth);
            Assert.AreSame(bin.Parent, kitchen);
        }

        [TestMethod]
        public void RemoveNodeByCallingRemove()
        {
            var building = new Tree<Space>(new Space { Name = "Science Building", SquareFeet = 30000 });
            var storage = building.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 1200 }));
            var bin= storage.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Bin", SquareFeet = 4 }));

            storage.Children.Remove(bin);

            Assert.AreEqual(0, building.Ancestors.Count());
            Assert.AreEqual(1, building.ChildNodes.Count());
            Assert.AreEqual(1, building.Descendants.Count());
            Assert.AreEqual(1, building.Height);
            Assert.AreEqual(0, building.Depth);
            Assert.IsNull(building.Parent);

            Assert.AreEqual(1, storage.Ancestors.Count());
            Assert.AreEqual(0, storage.ChildNodes.Count());
            Assert.AreEqual(0, storage.Descendants.Count());
            Assert.AreEqual(0, storage.Height);
            Assert.AreEqual(1, storage.Depth);
            Assert.AreSame(storage.Parent, building);

            Assert.AreEqual(0, bin.Ancestors.Count());
            Assert.AreEqual(0, bin.ChildNodes.Count());
            Assert.AreEqual(0, bin.Descendants.Count());
            Assert.AreEqual(0, bin.Height);
            Assert.AreEqual(0, bin.Depth);
            Assert.IsNull(bin.Parent);
        }

        [TestMethod]
        public void ComplexInitialState()
        {
            var property = new Tree<Space>(new Space { Name = "Industrial Property", SquareFeet = 150000 });
            var buildingA = property.Children.Add(new TreeNode<Space>(new Space { Name = "Building A", SquareFeet = 50000 }));
            var laundryRoom = buildingA.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Laundry Room", SquareFeet = 300 }));
            var bathroomA = buildingA.Children.Add(new TreeNode<Space>(new Space { Name = "Bathroom", SquareFeet = 150 }));
            var storageA = buildingA.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 450 }));
            var buildingB = property.Children.Add(new TreeNode<Space>(new Space { Name = "Building B", SquareFeet = 50000 }));
            var bathroomB = buildingB.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Bathroom", SquareFeet = 50000 }));
            var storageB = buildingB.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Storage", SquareFeet = 500 }));
            var meetingRoom = buildingB.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Room, Name = "Meeting Room", SquareFeet = 1600 }));
            var meetingRoomCloset = meetingRoom.Children.Add(new TreeNode<Space>(new Space { Type = SpaceTypes.Inventory, Name = "Meeting Room Closet", SquareFeet = 150 }));

            Assert.AreEqual(0, property.Ancestors.Count());
            Assert.AreEqual(2, property.Children.Count);
            Assert.AreEqual(9, property.Descendants.Count());
            Assert.AreEqual(3, property.Height);
            Assert.AreEqual(0, property.Depth);
            Assert.IsNull(property.Parent);

            Assert.AreEqual(1, buildingA.Ancestors.Count());
            Assert.AreEqual(3, buildingA.Children.Count);
            Assert.AreEqual(3, buildingA.Descendants.Count());
            Assert.AreEqual(1, buildingA.Height);
            Assert.AreEqual(1, buildingA.Depth);
            Assert.AreSame(property, buildingA.Parent);

            Assert.AreEqual(2, laundryRoom.Ancestors.Count());
            Assert.AreEqual(0, laundryRoom.Children.Count);
            Assert.AreEqual(0, laundryRoom.Descendants.Count());
            Assert.AreEqual(0, laundryRoom.Height);
            Assert.AreEqual(2, laundryRoom.Depth);
            Assert.AreSame(buildingA, laundryRoom.Parent);

            Assert.AreEqual(2, bathroomA.Ancestors.Count());
            Assert.AreEqual(0, bathroomA.Children.Count);
            Assert.AreEqual(0, bathroomA.Descendants.Count());
            Assert.AreEqual(0, bathroomA.Height);
            Assert.AreEqual(2, bathroomA.Depth);
            Assert.AreSame(buildingA, bathroomA.Parent);

            Assert.AreEqual(2, storageA.Ancestors.Count());
            Assert.AreEqual(0, storageA.Children.Count);
            Assert.AreEqual(0, storageA.Descendants.Count());
            Assert.AreEqual(0, storageA.Height);
            Assert.AreEqual(2, storageA.Depth);
            Assert.AreSame(buildingA, storageA.Parent);

            Assert.AreEqual(1, buildingB.Ancestors.Count());
            Assert.AreEqual(3, buildingB.Children.Count);
            Assert.AreEqual(4, buildingB.Descendants.Count());
            Assert.AreEqual(2, buildingB.Height);
            Assert.AreEqual(1, buildingB.Depth);
            Assert.AreSame(property, buildingB.Parent);

            Assert.AreEqual(2, bathroomB.Ancestors.Count());
            Assert.AreEqual(0, bathroomB.Children.Count);
            Assert.AreEqual(0, bathroomB.Descendants.Count());
            Assert.AreEqual(0, bathroomB.Height);
            Assert.AreEqual(2, bathroomB.Depth);
            Assert.AreSame(buildingB, bathroomB.Parent);

            Assert.AreEqual(2, storageB.Ancestors.Count());
            Assert.AreEqual(0, storageB.Children.Count);
            Assert.AreEqual(0, storageB.Descendants.Count());
            Assert.AreEqual(0, storageB.Height);
            Assert.AreEqual(2, storageB.Depth);
            Assert.AreSame(buildingB, storageB.Parent);

            Assert.AreEqual(2, meetingRoom.Ancestors.Count());
            Assert.AreEqual(1, meetingRoom.Children.Count);
            Assert.AreEqual(1, meetingRoom.Descendants.Count());
            Assert.AreEqual(1, meetingRoom.Height);
            Assert.AreEqual(2, meetingRoom.Depth);
            Assert.AreSame(buildingB, meetingRoom.Parent);

            Assert.AreEqual(3, meetingRoomCloset.Ancestors.Count());
            Assert.AreEqual(0, meetingRoomCloset.Children.Count);
            Assert.AreEqual(0, meetingRoomCloset.Descendants.Count());
            Assert.AreEqual(0, meetingRoomCloset.Height);
            Assert.AreEqual(3, meetingRoomCloset.Depth);
            Assert.AreSame(meetingRoom, meetingRoomCloset.Parent);

            var ancestorList = new List<string>();
            foreach (var node in meetingRoomCloset.Ancestors)
                ancestorList.Add(node.Data.Name);

            Assert.AreEqual(3, ancestorList.Count);
            Assert.IsTrue(ancestorList.Contains(meetingRoom.Data.Name));
            Assert.IsTrue(ancestorList.Contains(buildingB.Data.Name));
            Assert.IsTrue(ancestorList.Contains(property.Data.Name));

            ancestorList = new List<string>();
            foreach (var node in laundryRoom.Ancestors)
                ancestorList.Add(node.Data.Name);

            Assert.AreEqual(2, ancestorList.Count);
            Assert.IsTrue(ancestorList.Contains(buildingA.Data.Name));
            Assert.IsTrue(ancestorList.Contains(property.Data.Name));

            var descendantList = new List<string>();
            foreach (var node in buildingA.Descendants)
                descendantList.Add(node.Data.Name);

            Assert.AreEqual(3, descendantList.Count);
            Assert.IsTrue(descendantList.Contains(laundryRoom.Data.Name));
            Assert.IsTrue(descendantList.Contains(bathroomA.Data.Name));
            Assert.IsTrue(descendantList.Contains(storageA.Data.Name));

            descendantList = new List<string>();
            foreach (var node in property.Descendants)
                descendantList.Add(((Space)node.Data).Name);

            Assert.AreEqual(9, descendantList.Count);
            Assert.IsTrue(descendantList.Contains(buildingA.Data.Name));
            Assert.IsTrue(descendantList.Contains(laundryRoom.Data.Name));
            Assert.IsTrue(descendantList.Contains(bathroomA.Data.Name));
            Assert.IsTrue(descendantList.Contains(storageA.Data.Name));
            Assert.IsTrue(descendantList.Contains(buildingB.Data.Name));
            Assert.IsTrue(descendantList.Contains(bathroomB.Data.Name));
            Assert.IsTrue(descendantList.Contains(storageB.Data.Name));
            Assert.IsTrue(descendantList.Contains(meetingRoom.Data.Name));
            Assert.IsTrue(descendantList.Contains(meetingRoomCloset.Data.Name));
        }

        [TestMethod]
        public void IncrementalChanges()
        {
            var property = new Tree<Space>(new Space
            {
                Name = "Industrial Property",
                SquareFeet = 150000
            });

            Assert.AreEqual(0, property.Depth);
            Assert.AreEqual(0, property.Height);

            var buildingA = property.Children.Add(new TreeNode<Space>(
                new Space
                {
                    Name = "Building A",
                    SquareFeet = 900
                }));

            Assert.AreEqual(0, property.Depth);
            Assert.AreEqual(1, property.Height);

            Assert.AreEqual(1, buildingA.Depth);
            Assert.AreEqual(0, buildingA.Height);

            var parentOfBuildingA = buildingA.Parent;
            Assert.IsNotNull(parentOfBuildingA);
            Assert.AreEqual("Industrial Property", parentOfBuildingA.Data.Name);

            property.Children.Remove(buildingA);

            Assert.AreEqual(0, property.Depth);
            Assert.AreEqual(0, property.Height);

            Assert.AreEqual(0, buildingA.Depth);
            Assert.AreEqual(0, buildingA.Height);
        }
    }
}
