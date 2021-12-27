using ISynergy.Framework.Core.Extensions.Base;
using ISynergy.Framework.Core.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace ISynergy.Framework.Core.Extensions
{
#nullable disable
    /// <summary>
    /// Extensions for the <see cref="ICollection" /> and <see cref="Collection{T}" /> classes.
    /// </summary>
    public static class CollectionExtensions
    {
        //Linked list style enumerator
        /// <summary>
        /// Froms the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nextItem">The next item.</param>
        /// <param name="canContinue">The can continue.</param>
        /// <returns>IEnumerable&lt;TSource&gt;.</returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Froms the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nextItem">The next item.</param>
        /// <returns>IEnumerable&lt;TSource&gt;.</returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s is not null);
        }

        /// <summary>
        /// Determines whether the item can be moved up in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to check.</param>
        /// <returns><c>true</c> if the item can be moved up in the list; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        public static bool CanMoveItemUp(this IList list, object item)
        {
            Argument.IsNotNull(list);

            if (item is null)
            {
                return false;
            }

            if (list.Count <= 1)
            {
                return false;
            }

            if (list.IndexOf(item) <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves the specified item up in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to move up.</param>
        /// <returns><c>true</c> if the item has successfully been moved up; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="item" /> is <c>null</c>.</exception>
        public static bool MoveItemUp(this IList list, object item)
        {
            Argument.IsNotNull(list);
            Argument.IsNotNull(item);

            var currentIndex = list.IndexOf(item);

            if (currentIndex == -1)
            {
                return false;
            }

            return MoveItemUpByIndex(list, currentIndex);
        }

        /// <summary>
        /// Determines whether the item can be moved down in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to check.</param>
        /// <returns><c>true</c> if the item can be moved down in the list; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        public static bool CanMoveItemDown(this IList list, object item)
        {
            Argument.IsNotNull(list);

            if (item is null)
            {
                return false;
            }

            if (list.Count <= 1)
            {
                return false;
            }

            var index = list.IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            if (index == list.Count - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves the item at the specified index up in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index of the item to move up.</param>
        /// <returns><c>true</c> if the item has successfully been moved up; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is smaller than 0 or larger than the list count.</exception>
        public static bool MoveItemUpByIndex(this IList list, int index)
        {
            Argument.IsNotNull(list);
            Argument.IsNotOutOfRange(index, 0, list.Count - 1);

            if (list.Count < index - 1)
            {
                return false;
            }

            if (index == 0)
            {
                return true;
            }

            var item = list[index];
            list.RemoveAt(index);
            list.Insert(index - 1, item);

            return true;
        }

        /// <summary>
        /// Moves the specified item down in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to move down.</param>
        /// <returns><c>true</c> if the item has successfully been moved down; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="item" /> is <c>null</c>.</exception>
        public static bool MoveItemDown(this IList list, object item)
        {
            Argument.IsNotNull(list);
            Argument.IsNotNull(item);

            var currentIndex = list.IndexOf(item);

            if (currentIndex == -1)
            {
                return false;
            }

            return MoveItemDownByIndex(list, currentIndex);
        }

        /// <summary>
        /// Moves the item at the specified index down in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index of the item to move down.</param>
        /// <returns><c>true</c> if the item has successfully been moved down; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is smaller than 0 or larger than the list count.</exception>
        public static bool MoveItemDownByIndex(this IList list, int index)
        {
            Argument.IsNotNull(list);
            Argument.IsNotOutOfRange(index, 0, list.Count - 1);

            if (list.Count < index - 1)
            {
                return false;
            }

            if (index == list.Count - 1)
            {
                return true;
            }

            var item = list[index];
            list.RemoveAt(index);
            list.Insert(index + 1, item);

            return true;
        }

        /// <summary>
        /// Add an range of items to the specified <see cref="ICollection{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of items within the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="range">An range of items.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="range" /> is <c>null</c>.</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            collection.EnsureNotNull();

            foreach (var curItem in range.EnsureNotNull())
            {
                collection.Add(curItem);
            }
        }

        /// <summary>
        /// Add an new range of items to the specified <see cref="ICollection{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of items within the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="range">An range of items.</param>
        public static void AddNewRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            collection.EnsureNotNull();
            collection.Clear();
            collection.AddRange(range);
        }

        /// <summary>
        /// Replaces the whole range of the specified <paramref name="collection" />. This is done by internally
        /// calling <see cref="ICollection{T}.Clear" /> and finally <c>AddRange{T}</c>.
        /// </summary>
        /// <typeparam name="T">Type of items within the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="range">The range of items to add to the observable collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="range" /> is <c>null</c>.</exception>
        public static void ReplaceRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            Argument.IsNotNull(collection);
            Argument.IsNotNull(range);

            collection.Clear();

            AddRange(collection, range);
        }

        /// <summary>
        /// Removes the first entry from the list.
        /// <para />
        /// When there are no items in the list, this method will silently exit.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        public static void RemoveFirst(this IList list)
        {
            Argument.IsNotNull(list);

            if (list.Count == 0)
            {
                return;
            }

            list.RemoveAt(0);
        }

        /// <summary>
        /// Removes the last entry from the list.
        /// <para />
        /// When there are no items in the list, this method will silently exit.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list" /> is <c>null</c>.</exception>
        public static void RemoveLast(this IList list)
        {
            Argument.IsNotNull(list);

            if (list.Count == 0)
            {
                return;
            }

            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// The for each extension on <see cref="IEnumerable{TItem}" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items is null) return;
            foreach (var item in items.EnsureNotNull()) action(item);
        }

        /// <summary>
        /// Converts the dictionary to a readonly collection.
        /// </summary>
        /// <typeparam name="T">Type of the items inside the collection.</typeparam>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>The readonly version of the collection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <c>null</c>.</exception>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> collection)
        {
            Argument.IsNotNull(collection);

            return new ReadOnlyCollection<T>(collection);
        }

        /// <summary>
        /// Converts the collection to an array.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>Array.</returns>
        public static Array ToArray(this IEnumerable collection, Type elementType)
        {
            Argument.IsNotNull(elementType);

            var internalList = new List<object>(collection is not null ? collection.Cast<object>() : Array.Empty<object>());
            var array = Array.CreateInstance(elementType, internalList.Count);

            var index = 0;

            foreach (var item in internalList.EnsureNotNull())
            {
                array.SetValue(item, index++);
            }

            return array;
        }

        /// <summary>
        /// Synchronizes the collection by adding / removing items that are in the new set.
        /// </summary>
        /// <typeparam name="T">The type of the collection item.</typeparam>
        /// <param name="existingSet">The existing set.</param>
        /// <param name="newSet">The new set.</param>
        /// <param name="updateExistingSet">if set to <c>true</c>, the existing set will be updated, otherwise a new collection will be created and the existing set will remain unchanged.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> SynchronizeCollection<T>(this IList<T> existingSet, IEnumerable<T> newSet, bool updateExistingSet = true)
        {
            var finalSet = updateExistingSet ? existingSet : new List<T>(existingSet);
            var itemsToRemove = new List<T>(existingSet);
            var itemsToAdd = new List<T>();

            foreach (var newItem in newSet.EnsureNotNull())
            {
                if (itemsToRemove.Contains(newItem))
                {
                    itemsToRemove.Remove(newItem);
                }
                else
                {
                    itemsToAdd.Add(newItem);
                }
            }

            foreach (var itemToRemove in itemsToRemove.EnsureNotNull())
            {
                finalSet.Remove(itemToRemove);
            }

            foreach (var itemToAdd in itemsToAdd.EnsureNotNull())
            {
                finalSet.Add(itemToAdd);
            }

            return finalSet;
        }

        /// <summary>
        /// Sorts the specified existing set.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="existingSet">The existing set.</param>
        /// <param name="comparer">The comparer.</param>
        public static void Sort<T>(this IList<T> existingSet, Func<T, T, int> comparer = null)
        {
            Argument.IsNotNull(existingSet);

            for (var i = existingSet.Count - 1; i >= 0; i--)
            {
                for (var j = 1; j <= i; j++)
                {
                    var o1 = existingSet[j - 1];
                    var o2 = existingSet[j];

                    bool reshuffle;

                    if (comparer is null)
                    {
                        reshuffle = ((IComparable)o1).CompareTo(o2) > 0;
                    }
                    else
                    {
                        reshuffle = comparer(o1, o2) > 0;
                    }

                    if (reshuffle)
                    {
                        existingSet.Remove(o1);
                        existingSet.Insert(j, o1);
                    }
                }
            }
        }

        /// <summary>
        /// Converts to datatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTable<T>(this ICollection<T> collection, string tableName) =>
            collection.ToDataTableBase<T>(tableName);

        /// <summary>
        /// Converts to datatable.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="type">The type.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTable(this ICollection collection, Type type, string tableName) =>
            collection.ToDataTableBase(tableName, type);
    }
}
