using System.Collections.Concurrent;

namespace ISynergy.Framework.Core.Helpers
{
    /// <summary>
    /// Provides a read-write with key based locker.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key.
    /// </typeparam>
    public class ReadWriteLockDictionary<TKey> : ReadWriteLock
        where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, Lock> locks = new();

        /// <summary>
        /// Creates new instance of <see cref="ReadWriteLockDictionary{TKey}"/>.
        /// </summary>
        public ReadWriteLockDictionary()
        {
            locks = new ConcurrentDictionary<TKey, Lock>();
        }

        /// <summary>
        /// Creates new instance of <see cref="ReadWriteLockDictionary{TKey}"/>, specifying the <see cref="LockRecursionPolicy"/>.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The <see cref="LockRecursionPolicy"/> of the locker.
        /// </param>
        public ReadWriteLockDictionary(LockRecursionPolicy recursionPolicy)
            : base(recursionPolicy)
        {
            locks = new ConcurrentDictionary<TKey, Lock>();
        }

        /// <summary>
        /// Creates new instance of <see cref="ReadWriteLockDictionary{TKey}"/>, specifying the <see cref="IEqualityComparer{TKey}"/> of the <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="equalityComparer">
        /// The equality comparison implementation to use when comparing keys.
        /// </param>
        public ReadWriteLockDictionary(IEqualityComparer<TKey> equalityComparer)
        {
            locks = new ConcurrentDictionary<TKey, Lock>(equalityComparer);
        }

        /// <summary>
        /// Creates new instance of <see cref="ReadWriteLockDictionary{TKey}"/>, specifying the <see cref="LockRecursionPolicy"/> and the <see cref="IEqualityComparer{TKey}"/> of the <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The <see cref="LockRecursionPolicy"/> of the locker.
        /// </param>
        /// <param name="equalityComparer">
        /// The equality comparison implementation to use when comparing keys.
        /// </param>
        public ReadWriteLockDictionary(LockRecursionPolicy recursionPolicy, IEqualityComparer<TKey> equalityComparer)
            : base(recursionPolicy)
        {
            locks = new ConcurrentDictionary<TKey, Lock>(equalityComparer);
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> action.
        /// </summary>
        /// <param name="key">
        /// The key of the locker.
        /// </param>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockRead(TKey key, Action block)
        {
            LockRead(() =>
            {
                var pathLock = locks.GetOrAdd(key, _ => new Lock(this, key));
                Interlocked.Increment(ref pathLock.Lockers);
                pathLock.ReadWriteLock.LockRead(block);
                if (pathLock.Lockers <= 1)
                {
                    locks.TryRemove(key, out _);
                }
                else
                {
                    Interlocked.Decrement(ref pathLock.Lockers);
                }
            });
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> function.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The object type returned by the <paramref name="block"/> function.
        /// </typeparam>
        /// <param name="key">
        /// The key of the locker.
        /// </param>
        /// <param name="block">
        /// The function to be executed inside the lock block.
        /// </param>
        /// <returns>
        /// The object returned by the <paramref name="block"/> function.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public TReturn LockRead<TReturn>(TKey key, Func<TReturn> block)
        {
            return LockRead(() =>
            {
                var pathLock = locks.GetOrAdd(key, _ => new Lock(this, key));
                Interlocked.Increment(ref pathLock.Lockers);
                var result = pathLock.ReadWriteLock.LockRead(block);
                Interlocked.Decrement(ref pathLock.Lockers);

                if (pathLock.Lockers <= 1)
                {
                    locks.TryRemove(key, out _);
                }
                else
                {
                    Interlocked.Decrement(ref pathLock.Lockers);
                }

                return result;
            });
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> action with the option upgrade to write mode.
        /// </summary>
        /// <param name="key">
        /// The key of the locker.
        /// </param>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockUpgradeableRead(TKey key, Action block)
        {
            LockRead(() =>
            {
                var pathLock = locks.GetOrAdd(key, _ => new Lock(this, key));
                Interlocked.Increment(ref pathLock.Lockers);
                pathLock.ReadWriteLock.LockUpgradeableRead(block);

                if (pathLock.Lockers <= 1)
                {
                    locks.TryRemove(key, out _);
                }
                else
                {
                    Interlocked.Decrement(ref pathLock.Lockers);
                }
            });
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> function with the option upgrade to write mode.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The object type returned by the <paramref name="block"/> function.
        /// </typeparam>
        /// <param name="key">
        /// The key of the locker.
        /// </param>
        /// <param name="block">
        /// The function to be executed inside the lock block.
        /// </param>
        /// <returns>
        /// The object returned by the <paramref name="block"/> function.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public TReturn LockUpgradeableRead<TReturn>(TKey key, Func<TReturn> block)
        {
            return LockRead(() =>
            {
                var pathLock = locks.GetOrAdd(key, _ => new Lock(this, key));
                Interlocked.Increment(ref pathLock.Lockers);
                var result = pathLock.ReadWriteLock.LockUpgradeableRead(block);
                Interlocked.Decrement(ref pathLock.Lockers);
                if (pathLock.Lockers <= 1)
                {
                    locks.TryRemove(key, out _);
                }
                else
                {
                    Interlocked.Decrement(ref pathLock.Lockers);
                }

                return result;
            });
        }

        /// <summary>
        /// Locks write operations while executing the <paramref name="block"/> action.
        /// </summary>
        /// <param name="key">
        /// The key of the locker.
        /// </param>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockWrite(TKey key, Action block)
        {
            LockRead(() =>
            {
                var pathLock = locks.GetOrAdd(key, _ => new Lock(this, key));
                Interlocked.Increment(ref pathLock.Lockers);
                pathLock.ReadWriteLock.LockWrite(block);
                Interlocked.Decrement(ref pathLock.Lockers);
                if (pathLock.Lockers <= 1)
                {
                    locks.TryRemove(key, out _);
                }
                else
                {
                    Interlocked.Decrement(ref pathLock.Lockers);
                }
            });
        }

        /// <summary>
        /// Locks write operations while executing the <paramref name="block"/> function.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The object type returned by the <paramref name="block"/> function.
        /// </typeparam>
        /// <param name="key">
        /// The key of the locker.
        /// </param>
        /// <param name="block">
        /// The function to be executed inside the lock block.
        /// </param>
        /// <returns>
        /// The object returned by the <paramref name="block"/> function.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public TReturn LockWrite<TReturn>(TKey key, Func<TReturn> block)
        {
            return LockRead(() =>
            {
                var pathLock = locks.GetOrAdd(key, _ => new Lock(this, key));
                Interlocked.Increment(ref pathLock.Lockers);
                var result = pathLock.ReadWriteLock.LockWrite(block);
                Interlocked.Decrement(ref pathLock.Lockers);
                if (pathLock.Lockers <= 1)
                {
                    locks.TryRemove(key, out _);
                }
                else
                {
                    Interlocked.Decrement(ref pathLock.Lockers);
                }

                return result;
            });
        }

        private class Lock
        {
            public int Lockers = 0;
            public ReadWriteLockDictionary<TKey> Dictionary;
            public TKey Path;
            public ReadWriteLock ReadWriteLock;

            public Lock(ReadWriteLockDictionary<TKey> dictionary, TKey path)
            {
                Dictionary = dictionary;
                Path = path;
                ReadWriteLock = new ReadWriteLock(dictionary.ReaderWriterLockSlim.RecursionPolicy);
            }
        }
    }
}
