using System.Collections;

namespace ISynergy.Framework.Core.Collections.Base
{
    /// <summary>
    /// Provides a thread-safe observable collection used for data binding.
    /// </summary>
    /// <typeparam name="T">
    /// Specifies the type of the items in this collection.
    /// </typeparam>
    public abstract class ObservableConcurrentCollectionBase<T> : ObservableCollectionSyncContext, IReadOnlyList<T>, ICollection
    {
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </exception>
        public T this[int index] => ReadWriteLock.LockRead(() =>
                                                 {
                                                     if (index < 0 || index >= Items.Count)
                                                     {
                                                         throw new ArgumentOutOfRangeException(nameof(index));
                                                     }
                                                     return Items[index];
                                                 });

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableConcurrentCollectionBase{T}"/> collection.
        /// </summary>
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ObservableConcurrentCollectionBase{T}"/> is read-only.
        /// </summary>
        public bool IsReadOnly { get; protected set; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> wrapper around the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </summary>
        protected virtual List<T> Items { get; set; }

        // This must agree with Binding.IndexerName. It is declared separately
        // here so as to avoid a dependency on PresentationFramework.dll.
        private protected const string IndexerName = "Item[]";

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentCollectionBase{T}"/> class that contains empty elements and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        public ObservableConcurrentCollectionBase()
            : this(instance => new List<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentCollectionBase{T}"/> class.
        /// </summary>
        /// <param name="collectionWrapperFactory">
        /// The function used to create the <see cref="ObservableConcurrentCollectionBase{T}.Items"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collectionWrapperFactory"/> is a null reference.
        /// </exception>
        public ObservableConcurrentCollectionBase(Func<ObservableConcurrentCollectionBase<T>, List<T>> collectionWrapperFactory)
        {
            if (collectionWrapperFactory == null)
            {
                throw new ArgumentNullException(nameof(collectionWrapperFactory));
            }

            Items = collectionWrapperFactory.Invoke(this);
        }

        /// <summary>
        /// Determines whether the <see cref="ObservableConcurrentCollectionBase{T}"/> contains a specific <paramref name="item"/>.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if item is found in the <see cref="ObservableConcurrentCollectionBase{T}"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            return ReadWriteLock.LockRead(() => Items.Contains(item));
        }

        /// <summary>
        /// Copies the entire <see cref="ObservableConcurrentCollectionBase{T}"/> to a compatible one-dimensional <paramref name="array"/>, starting at the beginning of the specified target <paramref name="array"/>.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableConcurrentCollectionBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ObservableConcurrentCollectionBase{T}"/> is greater than the number of elements that the destination <paramref name="array"/> can contain.
        /// </exception>
        public void CopyTo(T[] array)
        {
            ReadWriteLock.LockRead(() => Items.CopyTo(array));
        }

        /// <summary>
        /// Copies the entire <see cref="ObservableConcurrentCollectionBase{T}"/> to a compatible one-dimensional <paramref name="array"/>, starting at the specified <paramref name="arrayIndex"/> of the target <paramref name="array"/>.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableConcurrentCollectionBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ObservableConcurrentCollectionBase{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ReadWriteLock.LockRead(() => Items.CopyTo(array, arrayIndex));
        }

        /// <summary>
        /// Copies the entire <see cref="ObservableConcurrentCollectionBase{T}"/> to a compatible one-dimensional <paramref name="array"/>, starting at the specified <paramref name="arrayIndex"/> of the target <paramref name="array"/>.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableConcurrentCollectionBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ObservableConcurrentCollectionBase{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(Array array, int arrayIndex)
        {
            ReadWriteLock.LockRead(() => (Items as ICollection).CopyTo(array, arrayIndex));
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="ObservableConcurrentCollectionBase{T}"/> to a compatible one-dimensional <paramref name="array"/>, starting at the specified <paramref name="arrayIndex"/> of the target <paramref name="array"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index in the source <see cref="ObservableConcurrentCollectionBase{T}"/> at which copying begins.
        /// </param>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableConcurrentCollectionBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <param name="count">
        /// The number of elements to copy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0. -or- <paramref name="arrayIndex"/> is less than 0. -or- <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> is equal to or greater than the <see cref="ObservableConcurrentCollectionBase{T}.Count"/> of the source <see cref="ObservableConcurrentCollectionBase{T}"/>. -or- The number of elements from <paramref name="index"/> to the end of the source <see cref="ObservableConcurrentCollectionBase{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            ReadWriteLock.LockRead(() => Items.CopyTo(index, array, arrayIndex, count));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return ReadWriteLock.LockRead(Items.GetEnumerator);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </summary>
        /// <param name="value">
        /// The object to locate in the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T value)
        {
            return ReadWriteLock.LockRead(() => Items.IndexOf(value));
        }

        /// <summary>
        /// Removes all elements from the <see cref="ObservableConcurrentCollectionBase{T}"/> and notify the observers.
        /// </summary>
        /// <param name="oldItems">
        /// The removed items of the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected bool ClearItems(out IEnumerable<T>? oldItems)
        {
            if (ClearItemsOperationInvoke(out oldItems))
            {
                ClearItemsObservableInvoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all elements from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </summary>
        /// <param name="oldItems">
        /// The removed items of the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected bool ClearItemsOperationInvoke(out IEnumerable<T>? oldItems)
        {
            IEnumerable<T>? proxy = default;

            var ret = ReadWriteLock.LockWrite(() =>
            {
                if (InternalClearItems(out proxy))
                {
                    return true;
                }

                return false;
            });

            oldItems = proxy;
            return ret;
        }

        /// <summary>
        /// Notify the observers with clear items operation.
        /// </summary>
        protected void ClearItemsObservableInvoke()
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(IndexerName);
                OnCollectionReset();
            });
        }

        /// <summary>
        /// Inserts an element into the <see cref="ObservableConcurrentCollectionBase{T}"/> at the specified <paramref name="index"/> and notify the observers.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">
        /// The element to insert. The value can be null for reference types.
        /// </param>
        /// <param name="lastCount">
        /// The last <see cref="Count"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> before the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- index is greater than <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool InsertItem(int index, T item, out int lastCount)
        {
            if (InsertItemOperationInvoke(index, item, out lastCount))
            {
                InsertItemObservableInvoke(index, item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts an element into the <see cref="ObservableConcurrentCollectionBase{T}"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">
        /// The element to insert. The value can be null for reference types.
        /// </param>
        /// <param name="lastCount">
        /// The last <see cref="Count"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> before the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- index is greater than <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool InsertItemOperationInvoke(int index, T item, out int lastCount)
        {
            int proxy = default;
            var ret = ReadWriteLock.LockUpgradeableRead(() =>
            {
                if (index < 0 || index > Items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return ReadWriteLock.LockWrite(() =>
                {
                    if (InternalInsertItems(index, new T[] { item }, out proxy))
                    {
                        return true;
                    }

                    return false;
                });
            });
            lastCount = proxy;
            return ret;
        }

        /// <summary>
        /// Notify the observers with insert item operation.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">
        /// The element to insert. The value can be null for reference types.
        /// </param>
        protected void InsertItemObservableInvoke(int index, T item)
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(IndexerName);
                OnCollectionAdd(item, index);
            });
        }

        /// <summary>
        /// Inserts an elements into the <see cref="ObservableConcurrentCollectionBase{T}"/> at the specified <paramref name="index"/> and notify the observers.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="items">
        /// The elements to insert. The value can be null for reference types.
        /// </param>
        /// <param name="lastCount">
        /// The last <see cref="Count"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> before the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- index is greater than <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool InsertItems(int index, IEnumerable<T> items, out int lastCount)
        {
            if (InsertItemsOperationInvoke(index, items, out lastCount))
            {
                InsertItemsObservableInvoke(index, items);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inserts an elements into the <see cref="ObservableConcurrentCollectionBase{T}"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="items">
        /// The elements to insert. The value can be null for reference types.
        /// </param>
        /// <param name="lastCount">
        /// The last <see cref="Count"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> before the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- index is greater than <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool InsertItemsOperationInvoke(int index, IEnumerable<T> items, out int lastCount)
        {
            int proxy = default;
            var ret = ReadWriteLock.LockUpgradeableRead(() =>
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                if (index < 0 || index > Items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return ReadWriteLock.LockWrite(() =>
                {
                    if (InternalInsertItems(index, items, out proxy))
                    {
                        return true;
                    }

                    return false;
                });
            });
            lastCount = proxy;
            return ret;
        }

        /// <summary>
        /// Notify the observers with insert items operation.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="items">
        /// The elements to insert. The value can be null for reference types.
        /// </param>
        protected void InsertItemsObservableInvoke(int index, IEnumerable<T>? items)
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(IndexerName);
                if (items is IList list)
                {
                    OnCollectionAdd(list, index);
                }
                else
                {
                    OnCollectionAdd(items?.ToList(), index);
                }
            });
        }

        /// <summary>
        /// Moves an element at the specified <paramref name="oldIndex"/> to the specified <paramref name="newIndex"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> and notify the observers.
        /// </summary>
        /// <param name="oldIndex">
        /// The index of the element to be moved.
        /// </param>
        /// <param name="newIndex">
        /// The new index of the element to move to.
        /// </param>
        /// <param name="movedItem">
        /// The moved element at the specified <paramref name="oldIndex"/> to the specified <paramref name="newIndex"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="oldIndex"/> or <paramref name="newIndex"/> is less than zero. -or- is greater than or equal to <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool MoveItem(int oldIndex, int newIndex, out T? movedItem)
        {
            if (MoveItemOperationInvoke(oldIndex, newIndex, out movedItem))
            {
                MoveItemObservableInvoke(oldIndex, newIndex, movedItem);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves an element at the specified <paramref name="oldIndex"/> to the specified <paramref name="newIndex"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </summary>
        /// <param name="oldIndex">
        /// The index of the element to be moved.
        /// </param>
        /// <param name="newIndex">
        /// The new index of the element to move to.
        /// </param>
        /// <param name="movedItem">
        /// The moved element at the specified <paramref name="oldIndex"/> to the specified <paramref name="newIndex"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="oldIndex"/> or <paramref name="newIndex"/> is less than zero. -or- is greater than or equal to <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool MoveItemOperationInvoke(int oldIndex, int newIndex, out T? movedItem)
        {
            T? proxy = default;
            var ret = ReadWriteLock.LockUpgradeableRead(() =>
            {
                return oldIndex < 0 || oldIndex >= Items.Count
                    ? throw new ArgumentOutOfRangeException(nameof(oldIndex))
                    : newIndex < 0 || newIndex >= Items.Count
                    ? throw new ArgumentOutOfRangeException(nameof(newIndex))
                    : ReadWriteLock.LockWrite(() =>
                {
                    return InternalMoveItem(oldIndex, newIndex, out proxy);
                });
            });
            movedItem = proxy;
            return ret;
        }

        /// <summary>
        /// Notify the observers with move item operation.
        /// </summary>
        /// <param name="oldIndex">
        /// The index of the element to be moved.
        /// </param>
        /// <param name="newIndex">
        /// The new index of the element to move to.
        /// </param>
        /// <param name="movedItem">
        /// The moved element at the specified <paramref name="oldIndex"/> to the specified <paramref name="newIndex"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        protected void MoveItemObservableInvoke(int oldIndex, int newIndex, T? movedItem)
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(IndexerName);
                OnCollectionMove(movedItem, oldIndex, newIndex);
            });
        }

        /// <summary>
        /// Removes the element at the specified <paramref name="index"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> and notify the observers.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <param name="removedItem">
        /// The removed element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- is greater than <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool RemoveItem(int index, out T? removedItem)
        {
            if (RemoveItemOperationInvoke(index, out removedItem))
            {
                RemoveItemObservableInvoke(index, removedItem);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the element at the specified <paramref name="index"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <param name="removedItem">
        /// The removed element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- is greater than <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool RemoveItemOperationInvoke(int index, out T? removedItem)
        {
            T? proxy = default;
            var ret = ReadWriteLock.LockUpgradeableRead(() =>
            {
                return index < 0 || index >= Items.Count
                    ? throw new ArgumentOutOfRangeException(nameof(index))
                    : ReadWriteLock.LockWrite(() =>
                {
                    if (InternalRemoveItems(index, 1, out var removedItems) && removedItems != null)
                    {
                        proxy = removedItems.FirstOrDefault();
                        return true;
                    }

                    return false;
                });
            });
            removedItem = proxy;
            return ret;
        }

        /// <summary>
        /// Notify observers with remove item operation.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <param name="removedItem">
        /// The removed element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        protected void RemoveItemObservableInvoke(int index, T? removedItem)
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(IndexerName);
                OnCollectionRemove(removedItem, index);
            });
        }

        /// <summary>
        /// Removes the elements at the specified <paramref name="index"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> and notify the observers.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the elements to remove.
        /// </param>
        /// <param name="count">
        /// The count of elements to remove.
        /// </param>
        /// <param name="removedItems">
        /// The removed elements at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        protected bool RemoveItems(int index, int count, out IEnumerable<T>? removedItems)
        {
            if (RemoveItemsOperationInvoke(index, count, out removedItems))
            {
                RemoveItemsObservableInvoke(index, removedItems);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the elements at the specified <paramref name="index"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the elements to remove.
        /// </param>
        /// <param name="count">
        /// The count of elements to remove.
        /// </param>
        /// <param name="removedItems">
        /// The removed elements at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        protected bool RemoveItemsOperationInvoke(int index, int count, out IEnumerable<T>? removedItems)
        {
            IEnumerable<T>? proxy = default;
            var ret = ReadWriteLock.LockUpgradeableRead(() =>
            {
                return index < 0
                    ? throw new ArgumentOutOfRangeException(nameof(index))
                    : count < 0
                    ? throw new ArgumentOutOfRangeException(nameof(count))
                    : index + count > Items.Count
                    ? throw new ArgumentException("Index and count do not denote a valid range of elements in the " + GetType().FullName)
                    : ReadWriteLock.LockWrite(() =>
                {
                    return InternalRemoveItems(index, count, out proxy);
                });
            });

            removedItems = proxy;
            return ret;
        }

        /// <summary>
        /// Notify observers with remove items operation.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the elements to remove.
        /// </param>
        /// <param name="removedItems">
        /// The removed elements at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        protected void RemoveItemsObservableInvoke(int index, IEnumerable<T>? removedItems)
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(IndexerName);
                if (removedItems is IList list)
                {
                    OnCollectionRemove(list, index);
                }
                else
                {
                    OnCollectionRemove(removedItems?.ToList(), index);
                }
            });
        }

        /// <summary>
        /// Replaces the element at the specified <paramref name="index"/> and notify the observers.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="item">
        /// The new value for the element at the specified <paramref name="index"/>. The value can be <c>null</c> for reference types.
        /// </param>
        /// <param name="originalItem">
        /// The replaced original element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- is greater than or equal to <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool SetItem(int index, T item, out T? originalItem)
        {
            if (SetItemOperationInvoke(index, item, out originalItem))
            {
                SetItemObservableInvoke(index, item, originalItem);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Replaces the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="item">
        /// The new value for the element at the specified <paramref name="index"/>. The value can be <c>null</c> for reference types.
        /// </param>
        /// <param name="originalItem">
        /// The replaced original element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. -or- is greater than or equal to <see cref="ObservableConcurrentCollectionBase{T}.Count"/>.
        /// </exception>
        protected bool SetItemOperationInvoke(int index, T item, out T? originalItem)
        {
            T? proxy = default;
            var ret = ReadWriteLock.LockUpgradeableRead(() =>
            {
                return index < 0 || index >= Items.Count
                    ? throw new ArgumentOutOfRangeException(nameof(index))
                    : ReadWriteLock.LockWrite(() =>
                {
                    return InternalSetItem(index, item, out proxy);
                });
            });
            originalItem = proxy;
            return ret;
        }

        /// <summary>
        /// Notify observers with set item operation.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="item">
        /// The new value for the element at the specified <paramref name="index"/>. The value can be <c>null</c> for reference types.
        /// </param>
        /// <param name="originalItem">
        /// The replaced original element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        protected void SetItemObservableInvoke(int index, T item, T? originalItem)
        {
            ReadWriteLock.InvokeOnLockExit(() =>
            {
                OnPropertyChanged(IndexerName);
                OnCollectionReplace(originalItem, item, index);
            });
        }

        /// <summary>
        /// Provides an overridable internal operation for <see cref="ClearItems"/>.
        /// </summary>
        /// <param name="oldItems">
        /// The removed items of the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalClearItems(out IEnumerable<T>? oldItems)
        {
            oldItems = Items.ToList();

            Items.Clear();

            return oldItems.Any();
        }

        /// <summary>
        /// Provides an overridable internal operation for <see cref="InsertItem(int, T, out int)"/> and <see cref="InsertItems(int, IEnumerable{T}, out int)"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index at which elements should be inserted.
        /// </param>
        /// <param name="items">
        /// The elements to insert. The value can be null for reference types.
        /// </param>
        /// <param name="lastCount">
        /// The last <see cref="Count"/> of the <see cref="ObservableConcurrentCollectionBase{T}"/> before the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalInsertItems(int index, IEnumerable<T> items, out int lastCount)
        {
            lastCount = Items.Count;

            var insertCount = items.Count();

            if (insertCount == 1)
            {
                Items.Insert(index, items.First());

                return true;
            }
            else if (insertCount > 1)
            {
                Items.InsertRange(index, items);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provides an overridable internal operation for <see cref="MoveItem(int, int, out T)"/>.
        /// </summary>
        /// <param name="oldIndex">
        /// The index of the element to be moved.
        /// </param>
        /// <param name="newIndex">
        /// The new index of the element to move to.
        /// </param>
        /// <param name="movedItem">
        /// The moved element at the specified <paramref name="oldIndex"/> to the specified <paramref name="newIndex"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalMoveItem(int oldIndex, int newIndex, out T? movedItem)
        {
            movedItem = Items[oldIndex];

            Items.RemoveAt(oldIndex);
            Items.Insert(newIndex, movedItem);

            return true;
        }

        /// <summary>
        /// Provides an overridable internal operation for <see cref="RemoveItem(int, out T)"/> and <see cref="RemoveItems(int, int, out IEnumerable{T})"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <param name="count">
        /// The count of elements to remove.
        /// </param>
        /// <param name="oldItems">
        /// The removed elements at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalRemoveItems(int index, int count, out IEnumerable<T>? oldItems)
        {
            if (count == 1)
            {
                oldItems = new T[] { Items[index] };

                Items.RemoveAt(index);

                return true;
            }
            else if (count > 1)
            {
                oldItems = Items.GetRange(index, count);

                Items.RemoveRange(index, count);

                return true;
            }
            else
            {
                oldItems = null;

                return false;
            }
        }

        /// <summary>
        /// Provides an overridable internal operation for <see cref="SetItem(int, T, out T?)"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="item">
        /// The new value for the element at the specified <paramref name="index"/>. The value can be <c>null</c> for reference types.
        /// </param>
        /// <param name="originalItem">
        /// The replaced original element at the specified <paramref name="index"/> from the <see cref="ObservableConcurrentCollectionBase{T}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if operation was executed; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalSetItem(int index, T item, out T? originalItem)
        {
            originalItem = Items[index];

            Items[index] = item;

            return true;
        }

        private protected static ArgumentException WrongTypeException(string propertyName, Type? providedType)
        {
            return new ArgumentException("Expected value type is \"" + typeof(T).FullName + "\" but collection was provided with \"" + (providedType?.FullName ?? "unknown") + "\" value type.", propertyName);
        }

        private protected static NotSupportedException ReadOnlyException(string operationName)
        {
            return new NotSupportedException("Operation \"" + operationName + "\" is not supported. Collection is read-only.");
        }

        T IReadOnlyList<T>.this[int index] => this[index];

        int IReadOnlyCollection<T>.Count => Count;

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    Interlocked.CompareExchange(ref syncRoot, new object(), null);
                }

                return syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index) => CopyTo(array, index);

        private object? syncRoot;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
