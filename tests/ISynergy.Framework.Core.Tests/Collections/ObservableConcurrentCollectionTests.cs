using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Collections.Tests
{
    public class ReadOnlyCollection : ObservableConcurrentCollection<int>
    {
        public ReadOnlyCollection()
        {
            IsReadOnly = true;
        }

        public ReadOnlyCollection(IEnumerable<int> initial)
            : base(initial)
        {
            IsReadOnly = true;
        }
    }

    [TestClass]
    public class ConcurrencyTest
    {
        [TestMethod]
        public async Task WithConcurrency1()
        {
            var raiseCount1 = 0;
            var col = new ObservableConcurrentCollection<int>();

            Assert.AreEqual(0, col.Count);

            col.CollectionChanged += (s, e) =>
            {
                Interlocked.Increment(ref raiseCount1);
            };

            await Task.WhenAll(
                Task.Run(delegate
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        col.Add(i);
                    }
                }),
                Task.Run(delegate
                {
                    for (int i = 10000; i < 20000; i++)
                    {
                        col.Add(i);
                    }
                }));

            Assert.AreEqual(20000, raiseCount1);
            Assert.AreEqual(20000, col.Count);

            for (int i = 0; i < 20000; i++)
            {
                Assert.IsTrue(col.Contains(i));
            }
        }

        [TestMethod]
        public async Task WithConcurrency2()
        {
            var raiseCount1 = 0;
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                Interlocked.Increment(ref raiseCount1);
            };

            await Task.WhenAll(
                Task.Run(delegate
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        col.AddRange(i, i + 10000, i + 20000);
                    }
                }),
                Task.Run(delegate
                {
                    for (int i = 30000; i < 40000; i++)
                    {
                        col.AddRange(new int[] { i, i + 10000, i + 20000 } as IEnumerable<int>);
                    }
                }));

            Assert.AreEqual(20000, raiseCount1);
            Assert.AreEqual(60000, col.Count);

            for (int i = 0; i < 60000; i++)
            {
                Assert.IsTrue(col.Contains(i));
            }
        }


        [TestMethod]
        public async Task WithConcurrency3()
        {
            var raiseCount1 = 0;
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                Interlocked.Increment(ref raiseCount1);
            };

            await Task.WhenAll(
                Task.Run(delegate
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        col.AddRange(i, i + 10000, i + 20000);
                    }
                }),
                Task.Run(delegate
                {
                    for (int i = 30000; i < 40000; i++)
                    {
                        col.AddRange(new int[] { i, i + 10000, i + 20000 } as IEnumerable<int>);
                    }
                }));

            for (int i = 0; i < 60000; i++)
            {
                Assert.IsTrue(col.Contains(i));
            }

            col.Clear();

            Assert.AreEqual(20001, raiseCount1);
            Assert.AreEqual(0, col.Count);
        }

        [TestMethod]
        public async Task WithConcurrency4()
        {
            var raiseCount1 = 0;
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                Interlocked.Increment(ref raiseCount1);
            };

            await Task.WhenAll(
                Task.Run(delegate
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        col.AddRange(i, i + 10000, i + 20000);
                    }
                }),
                Task.Run(delegate
                {
                    for (int i = 30000; i < 40000; i++)
                    {
                        col.AddRange(new int[] { i, i + 10000, i + 20000 } as IEnumerable<int>);
                    }
                }));

            Assert.AreEqual(20000, raiseCount1);
            Assert.AreEqual(60000, col.Count);

            for (int i = 0; i < 70000; i++)
            {
                if (i >= 60000)
                {
                    Assert.IsFalse(col.Contains(i));
                }
                else
                {
                    Assert.IsTrue(col.Contains(i));
                }
            }
        }
    }

    [TestClass]
    public class IndexerTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            Assert.AreEqual(1, col[0]);
            Assert.AreEqual(2, col[1]);
            Assert.AreEqual(3, col[2]);

            col[2] = 33;

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, raiseCol[0].Action);
            Assert.AreEqual(2, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(2, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[0]?.OldItems?[0]);
            Assert.AreEqual(33, raiseCol[0]?.NewItems?[0]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 33 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3 });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col[-1]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col[4]);

            var col2 = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col2[0] = 11);
        }
    }

    [TestClass]
    public class CountTest
    {
        [TestMethod]
        public void Normal()
        {
            var col = new ObservableConcurrentCollection<int>();

            Assert.AreEqual(0, col.Count);

            col.AddRange(1, 2, 3, 4, 5, 6);

            Assert.AreEqual(6, col.Count);
        }
    }

    [TestClass]
    public class IsReadOnlyTest
    {
        [TestMethod]
        public void Normal()
        {
            var listCols = new List<ObservableConcurrentCollection<int>>(
                new ObservableConcurrentCollection<int>[]
                {
                new ObservableConcurrentCollection<int>(),
                new ReadOnlyCollection(),
                new ObservableConcurrentCollection<int>(),
                new ReadOnlyCollection(),
                new ObservableConcurrentCollection<int>(),
                new ReadOnlyCollection(),
                });

            foreach (var col in listCols)
            {
                if (col.IsReadOnly)
                {
                    Assert.ThrowsException<NotSupportedException>(() => col.Add(1));
                }
                else
                {
                    col.Add(1);
                }
            }
        }
    }

    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        public void Parameterless()
        {
            var col = new ObservableConcurrentCollection<int>();

            Assert.AreEqual(0, col.Count);
        }

        [TestMethod]
        public void WithInitialItems()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            Assert.AreEqual(4, col.Count);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ObservableConcurrentCollection<int>(null));
        }
    }

    [TestClass]
    public class AddTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Add(1);
            col.Add(2);
            col.Add(3);

            Assert.AreEqual(3, col.Count);
            Assert.AreEqual(3, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(1, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(2, raiseCol[1]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[2].Action);
            Assert.AreEqual(2, raiseCol[2].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[2]?.NewItems?[0]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col.Add(1));
        }
    }

    [TestClass]
    public class AddRangeTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.AddRange(1, 2);
            col.AddRange(new int[] { 3, 4 } as IEnumerable<int>);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(2, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);
            Assert.AreEqual(2, raiseCol[0]?.NewItems?[1]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(2, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.NewItems?[0]);
            Assert.AreEqual(4, raiseCol[1]?.NewItems?[1]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>();
            Assert.ThrowsException<ArgumentNullException>(() => col.AddRange(null));
            Assert.ThrowsException<ArgumentNullException>(() => col.AddRange((IEnumerable<int>?)null));
            var col1 = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col1.AddRange(1));
        }
    }

    [TestClass]
    public class ClearTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4, 5, 6 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Clear();

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(1, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, raiseCol[0].Action);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col.Clear());
        }
    }

    [TestClass]
    public class ContainsTest
    {
        [TestMethod]
        public void Normal()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            Assert.IsTrue(col.Contains(1));
            Assert.IsTrue(col.Contains(2));
            Assert.IsTrue(col.Contains(3));
            Assert.IsTrue(col.Contains(4));
            Assert.IsFalse(col.Contains(5));
            Assert.IsFalse(col.Contains(6));
        }
    }

    [TestClass]
    public class CopyToTest
    {
        [TestMethod]
        public void Normal()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });
            int[] clone1 = new int[4];
            col.CopyTo(clone1);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, clone1);
        }

        [TestMethod]
        public void WithArrayIndex()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });
            int[] clone2 = new int[4];
            col.CopyTo(clone2, 0);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, clone2);
        }


        [TestMethod]
        public void ArrayWithArrayIndex()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });
            Array clone3 = new int[6];
            col.CopyTo(clone3, 2);

            Assert.AreEqual(0, clone3.Cast<int>().ElementAt(0));
            Assert.AreEqual(0, clone3.Cast<int>().ElementAt(1));
            Assert.AreEqual(1, clone3.Cast<int>().ElementAt(2));
            Assert.AreEqual(2, clone3.Cast<int>().ElementAt(3));
            Assert.AreEqual(3, clone3.Cast<int>().ElementAt(4));
            Assert.AreEqual(4, clone3.Cast<int>().ElementAt(5));
        }

        [TestMethod]
        public void WithArrayIndexAndSourceIndex()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });
            int[] clone4 = new int[6];
            col.CopyTo(2, clone4, 2, 2);

            CollectionAssert.AreEqual(new int[] { 0, 0, 3, 4, 0, 0 }, clone4);
        }
    }

    [TestClass]
    public class GetEnumeratorTest
    {
        [TestMethod]
        public void ForLoop()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            var enumerator = col.GetEnumerator();
            for (int i = 0; i < col.Count; i++)
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(i + 1, enumerator.Current);
            }

            Assert.IsFalse(enumerator.MoveNext());
        }
    }

    [TestClass]
    public class IndexOfTest
    {
        [TestMethod]
        public void Normal()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            Assert.AreEqual(0, col.IndexOf(1));
            Assert.AreEqual(1, col.IndexOf(2));
            Assert.AreEqual(2, col.IndexOf(3));
            Assert.AreEqual(3, col.IndexOf(4));
        }
    }

    [TestClass]
    public class InsertTest
    {
        [TestMethod]
        public void AtBottom()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Insert(0, 1);
            col.Insert(0, 2);
            col.Insert(0, 3);
            col.Insert(0, 4);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(4, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(0, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(2, raiseCol[1]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[2].Action);
            Assert.AreEqual(0, raiseCol[2].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[2]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[3].Action);
            Assert.AreEqual(0, raiseCol[3].NewStartingIndex);
            Assert.AreEqual(4, raiseCol[3]?.NewItems?[0]);

            CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, col);
        }
        [TestMethod]
        public void AtTop()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Insert(col.Count, 1);
            col.Insert(col.Count, 2);
            col.Insert(col.Count, 3);
            col.Insert(col.Count, 4);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(4, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(1, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(2, raiseCol[1]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[2].Action);
            Assert.AreEqual(2, raiseCol[2].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[2]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[3].Action);
            Assert.AreEqual(3, raiseCol[3].NewStartingIndex);
            Assert.AreEqual(4, raiseCol[3]?.NewItems?[0]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, col);
        }

        [TestMethod]
        public void AtAny()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 2, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Insert(0, 1);
            col.Insert(2, 3);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(2, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(2, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.NewItems?[0]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Insert(5, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Insert(-1, 1));

            var col1 = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col1.Insert(0, 1));
        }
    }

    [TestClass]
    public class InsertRange
    {
        [TestMethod]
        public void AtBottom()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.InsertRange(0, 1, 2);
            col.InsertRange(0, new int[] { 3, 4 } as IEnumerable<int>);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(2, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);
            Assert.AreEqual(2, raiseCol[0]?.NewItems?[1]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(0, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.NewItems?[0]);
            Assert.AreEqual(4, raiseCol[1]?.NewItems?[1]);

            CollectionAssert.AreEqual(new int[] { 3, 4, 1, 2 }, col);
        }
        [TestMethod]
        public void AtTop()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>();

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.InsertRange(col.Count, 1, 2);
            col.InsertRange(col.Count, new int[] { 3, 4 } as IEnumerable<int>);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(2, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.NewItems?[0]);
            Assert.AreEqual(2, raiseCol[0]?.NewItems?[1]);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, raiseCol[1].Action);
            Assert.AreEqual(2, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.NewItems?[0]);
            Assert.AreEqual(4, raiseCol[1]?.NewItems?[1]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });
            Assert.ThrowsException<ArgumentNullException>(() => col.InsertRange(4, null));
            Assert.ThrowsException<ArgumentNullException>(() => col.InsertRange(4, (IEnumerable<int>?)null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.InsertRange(5, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.InsertRange(-1, 1));

            var col1 = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col1.InsertRange(0, 1));
        }
    }

    [TestClass]
    public class MoveTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Move(3, 0);
            col.Move(3, 1);
            col.Move(3, 2);

            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(3, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Move, raiseCol[0].Action);
            Assert.AreEqual(3, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(0, raiseCol[0].NewStartingIndex);
            Assert.AreEqual(4, raiseCol[0]?.OldItems?[0]);
            Assert.AreEqual(4, raiseCol[0]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Move, raiseCol[1].Action);
            Assert.AreEqual(3, raiseCol[1].OldStartingIndex);
            Assert.AreEqual(1, raiseCol[1].NewStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.OldItems?[0]);
            Assert.AreEqual(3, raiseCol[1]?.NewItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Move, raiseCol[2].Action);
            Assert.AreEqual(3, raiseCol[2].OldStartingIndex);
            Assert.AreEqual(2, raiseCol[2].NewStartingIndex);
            Assert.AreEqual(2, raiseCol[2]?.OldItems?[0]);
            Assert.AreEqual(2, raiseCol[2]?.NewItems?[0]);

            CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, col);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Move(4, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Move(-1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Move(1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Move(1, 4));

            var col1 = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col1.Move(0, 1));
        }
    }

    [TestClass]
    public class RemoveTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Remove(1);
            col.Remove(3);

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(2, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[1].Action);
            Assert.AreEqual(1, raiseCol[1].OldStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.OldItems?[0]);

            CollectionAssert.AreEqual(new int[] { 2, 4 }, col);
        }

        [TestMethod]
        public void All()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.Remove(1);
            col.Remove(2);
            col.Remove(3);
            col.Remove(4);

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(4, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[1].Action);
            Assert.AreEqual(0, raiseCol[1].OldStartingIndex);
            Assert.AreEqual(2, raiseCol[1]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[2].Action);
            Assert.AreEqual(0, raiseCol[2].OldStartingIndex);
            Assert.AreEqual(3, raiseCol[2]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[3].Action);
            Assert.AreEqual(0, raiseCol[3].OldStartingIndex);
            Assert.AreEqual(4, raiseCol[3]?.OldItems?[0]);
        }

        [TestMethod]
        public void Throws()
        {
            var col1 = new ReadOnlyCollection();

            Assert.ThrowsException<NotSupportedException>(() => col1.Remove(1));
        }
    }

    [TestClass]
    public class RemoveAtTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.RemoveAt(0);
            col.RemoveAt(1);

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(2, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[1].Action);
            Assert.AreEqual(1, raiseCol[1].OldStartingIndex);
            Assert.AreEqual(3, raiseCol[1]?.OldItems?[0]);

            CollectionAssert.AreEqual(new int[] { 2, 4 }, col);
        }

        [TestMethod]
        public void All()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.RemoveAt(0);
            col.RemoveAt(0);
            col.RemoveAt(0);
            col.RemoveAt(0);

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(4, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[1].Action);
            Assert.AreEqual(0, raiseCol[1].OldStartingIndex);
            Assert.AreEqual(2, raiseCol[1]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[2].Action);
            Assert.AreEqual(0, raiseCol[2].OldStartingIndex);
            Assert.AreEqual(3, raiseCol[2]?.OldItems?[0]);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[3].Action);
            Assert.AreEqual(0, raiseCol[3].OldStartingIndex);
            Assert.AreEqual(4, raiseCol[3]?.OldItems?[0]);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.RemoveAt(0);
            col.RemoveAt(0);
            col.RemoveAt(0);
            col.RemoveAt(0);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.RemoveAt(0));

            var col1 = new ReadOnlyCollection(new int[] { 1, 2, 3, 4 });

            Assert.ThrowsException<NotSupportedException>(() => col1.RemoveAt(0));
        }
    }

    [TestClass]
    public class RemoveRangeTest
    {
        [TestMethod]
        public void Normal()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.RemoveRange(2, 2);

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(1, raiseCol.Count);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[0].Action);
            Assert.AreEqual(2, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(3, raiseCol[0]?.OldItems?[0]);
            Assert.AreEqual(4, raiseCol[0]?.OldItems?[1]);

            CollectionAssert.AreEqual(new int[] { 1, 2 }, col);
        }

        [TestMethod]
        public void All()
        {
            var raiseCol = new List<NotifyCollectionChangedEventArgs>();
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            col.CollectionChanged += (s, e) =>
            {
                raiseCol.Add(e);
            };

            col.RemoveRange(0, 4);

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, raiseCol[0].Action);
            Assert.AreEqual(0, raiseCol[0].OldStartingIndex);
            Assert.AreEqual(1, raiseCol[0]?.OldItems?[0]);
            Assert.AreEqual(2, raiseCol[0]?.OldItems?[1]);
            Assert.AreEqual(3, raiseCol[0]?.OldItems?[2]);
            Assert.AreEqual(4, raiseCol[0]?.OldItems?[3]);

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(1, raiseCol.Count);
        }

        [TestMethod]
        public void Throws()
        {
            var col = new ObservableConcurrentCollection<int>(new int[] { 1, 2, 3, 4 });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.RemoveRange(-1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.RemoveRange(0, -1));
            Assert.ThrowsException<ArgumentException>(() => col.RemoveRange(0, 5));
            Assert.ThrowsException<ArgumentException>(() => col.RemoveRange(1, 4));
            Assert.ThrowsException<ArgumentException>(() => col.RemoveRange(4, 1));

            col.RemoveAt(0);
            col.RemoveAt(0);
            col.RemoveAt(0);
            col.RemoveAt(0);

            Assert.ThrowsException<ArgumentException>(() => col.RemoveRange(0, 1));

            var col1 = new ReadOnlyCollection(new int[] { 1, 2, 3, 4 });

            Assert.ThrowsException<NotSupportedException>(() => col1.RemoveRange(0, 1));
        }
    }
}
