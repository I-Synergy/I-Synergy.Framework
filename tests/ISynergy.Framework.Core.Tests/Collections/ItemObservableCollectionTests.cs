using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Collections.Tests;

[TestClass]
public class ItemObservableCollectionTests
{
    private class TestItem : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [TestMethod]
    public void Constructor_CreatesEmptyCollection()
    {
        var collection = new ItemObservableCollection<TestItem>();
        Assert.AreEqual(0, collection.Count);
    }

    [TestMethod]
    public void Add_ItemTriggersCollectionChanged()
    {
        var collection = new ItemObservableCollection<TestItem>();
        var eventRaised = false;
        collection.CollectionChanged += (s, e) => eventRaised = true;

        collection.Add(new TestItem());

        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void ItemPropertyChanged_RaisesEvent()
    {
        var collection = new ItemObservableCollection<TestItem>();
        var item = new TestItem();
        var eventRaised = false;
        var changedItem = default(TestItem);
        var changedProperty = default(string);

        collection.Add(item);
        collection.ItemPropertyChanged += (s, e) =>
        {
            eventRaised = true;
            changedItem = e.Item;
            changedProperty = e.PropertyName;
        };

        item.Name = "Test";

        Assert.IsTrue(eventRaised);
        Assert.AreEqual(item, changedItem);
        Assert.AreEqual(nameof(TestItem.Name), changedProperty);
    }

    [TestMethod]
    public void Remove_ItemStopsNotifications()
    {
        var collection = new ItemObservableCollection<TestItem>();
        var item = new TestItem();
        var eventRaised = false;

        collection.Add(item);
        collection.Remove(item);
        collection.ItemPropertyChanged += (s, e) => eventRaised = true;

        item.Name = "Test";

        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void Clear_RemovesAllItemsAndStopsNotifications()
    {
        var collection = new ItemObservableCollection<TestItem>();
        var item1 = new TestItem();
        var item2 = new TestItem();
        var eventRaised = false;

        collection.Add(item1);
        collection.Add(item2);
        collection.Clear();
        collection.ItemPropertyChanged += (s, e) => eventRaised = true;

        item1.Name = "Test1";
        item2.Name = "Test2";

        Assert.IsFalse(eventRaised);
        Assert.AreEqual(0, collection.Count);
    }

    [TestMethod]
    public void Dispose_CleansUpEventSubscriptions()
    {
        var collection = new ItemObservableCollection<TestItem>();
        var item = new TestItem();
        var eventRaised = false;

        collection.Add(item);
        collection.ItemPropertyChanged += (s, e) => eventRaised = true;
        collection.Dispose();

        item.Name = "Test";

        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void SetItem_ReplacesItemAndHandlesNotifications()
    {
        var collection = new ItemObservableCollection<TestItem>();
        var item1 = new TestItem();
        var item2 = new TestItem();
        var eventRaised = false;
        var eventCount = 0;

        collection.Add(item1);

        collection.ItemPropertyChanged += (s, e) =>
        {
            eventRaised = true;
            eventCount++;
        };

        collection[0] = item2;

        item1.Name = "Test1"; // Should not trigger
        item2.Name = "Test2"; // Should trigger

        Assert.IsTrue(eventRaised);
        Assert.AreEqual(1, eventCount);
    }

    [TestMethod]
    public void Collection_HasNoMemoryLeaks()
    {
        WeakReference collectionRef;
        WeakReference itemRef;

        CreateCollectionAndItem(out collectionRef, out itemRef);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.IsFalse(collectionRef.IsAlive, "Collection should be garbage collected");
        Assert.IsFalse(itemRef.IsAlive, "Item should be garbage collected");
    }

    private void CreateCollectionAndItem(out WeakReference collectionRef, out WeakReference itemRef)
    {
        var collection = new ItemObservableCollection<TestItem>();
        var item = new TestItem { Name = "Test" };

        collection.Add(item);
        item.Name = "Updated";
        collection.Clear();
        collection.Dispose();

        collectionRef = new WeakReference(collection);
        itemRef = new WeakReference(item);
    }
}
