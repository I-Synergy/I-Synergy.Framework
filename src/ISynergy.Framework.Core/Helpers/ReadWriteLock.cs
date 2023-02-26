namespace ISynergy.Framework.Core.Helpers
{
    public class ReadWriteLock
    {
        /// <summary>
        /// Gets the wrapped <see cref="ReaderWriterLockSlim"/>.
        /// </summary>
        protected ReaderWriterLockSlim ReaderWriterLockSlim { get; }

        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

        private event Action? OnLockFree;
        private event Action? OnWriteLockFree;
        private event Action? OnReadLockFree;
        private event Action? OnUpgradeableReadLockFree;

        /// <summary>
        /// Creates new instance of <see cref="ReadWriteLock"/> with default <see cref="LockRecursionPolicy.NoRecursion"/>.
        /// </summary>
        public ReadWriteLock()
        {
            ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        /// <summary>
        /// Creates new instance of <see cref="ReadWriteLock"/>, specifying the <see cref="LockRecursionPolicy"/>.
        /// </summary>
        public ReadWriteLock(LockRecursionPolicy recursionPolicy)
        {
            ReaderWriterLockSlim = new ReaderWriterLockSlim(recursionPolicy);
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> action.
        /// </summary>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockRead(Action block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            LockRead(() =>
            {
                block();
                return 0;
            });
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> action.
        /// </summary>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockReadAndForget(Action block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var isLocked = false;

            Task.Run(() =>
            {
                LockRead(() =>
                {
                    isLocked = true;
                    block();
                    return 0;
                });
            });

            while (!isLocked)
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> function.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The object type returned by the <paramref name="block"/> function.
        /// </typeparam>
        /// <param name="block">
        /// The function to be executed inside the lock block.
        /// </param>
        /// <returns>
        /// The object returned by the <paramref name="block"/> function.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public TReturn LockRead<TReturn>(Func<TReturn> block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            try
            {
                ReaderWriterLockSlim.EnterReadLock();
                return block();
            }
            finally
            {
                ReaderWriterLockSlim.ExitReadLock();
                TryInvokeOnReadLockExit();
                TryInvokeOnLockExit();
            }
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> action while having an option to upgrade to write mode.
        /// </summary>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockUpgradeableRead(Action block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            LockUpgradeableRead(() =>
            {
                block();
                return 0;
            });
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> action while having an option to upgrade to write mode.
        /// </summary>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockUpgradeableReadAndForget(Action block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var isLocked = false;

            LockUpgradeableRead(() =>
            {
                isLocked = true;
                block();
                return 0;
            });

            while (!isLocked)
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Locks read operations while executing the <paramref name="block"/> function while having an option to upgrade to write mode.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The object type returned by the <paramref name="block"/> function.
        /// </typeparam>
        /// <param name="block">
        /// The function to be executed inside the lock block.
        /// </param>
        /// <returns>
        /// The object returned by the <paramref name="block"/> function.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public TReturn LockUpgradeableRead<TReturn>(Func<TReturn> block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            try
            {
                ReaderWriterLockSlim.EnterUpgradeableReadLock();
                return block();
            }
            finally
            {
                ReaderWriterLockSlim.ExitUpgradeableReadLock();
                TryInvokeOnUpgradeableReadLockExit();
                TryInvokeOnLockExit();
            }
        }

        /// <summary>
        /// Locks write operations while executing the <paramref name="block"/> action.
        /// </summary>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockWrite(Action block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            LockWrite(() =>
            {
                block();
                return 0;
            });
        }

        /// <summary>
        /// Locks write operations while executing the <paramref name="block"/> action.
        /// </summary>
        /// <param name="block">
        /// The action to be executed inside the lock block.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public void LockWriteAndForget(Action block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var isLocked = false;

            LockWrite(() =>
            {
                isLocked = true;
                block();
                return 0;
            });

            while (!isLocked)
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Locks write operations while executing the <paramref name="block"/> function.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The object type returned by the <paramref name="block"/> function.
        /// </typeparam>
        /// <param name="block">
        /// The function to be executed inside the lock block.
        /// </param>
        /// <returns>
        /// The object returned by the <paramref name="block"/> function.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="block"/> is a null reference.
        /// </exception>
        public TReturn LockWrite<TReturn>(Func<TReturn> block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            try
            {
                ReaderWriterLockSlim.EnterWriteLock();
                return block();
            }
            finally
            {
                ReaderWriterLockSlim.ExitWriteLock();
                TryInvokeOnWriteLockExit();
                TryInvokeOnLockExit();
            }
        }

        /// <summary>
        /// Invoke <see cref="Action"/> on read lock exit.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to invoke on read lock exit.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is a null reference.
        /// </exception>
        public void InvokeOnReadLockExit(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var isFree = true;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (ReaderWriterLockSlim.IsReadLockHeld)
                {
                    try
                    {
                        _lock.EnterWriteLock();
                        OnReadLockFree += action;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    isFree = false;
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            if (isFree)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Invoke <see cref="Action"/> on upgradeable read lock exit.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to invoke on upgradeable read lock exit.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is a null reference.
        /// </exception>
        public void InvokeOnUpgradeableReadLockExit(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var isFree = true;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (ReaderWriterLockSlim.IsUpgradeableReadLockHeld)
                {
                    try
                    {
                        _lock.EnterWriteLock();
                        OnUpgradeableReadLockFree += action;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    isFree = false;
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            if (isFree)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Invoke <see cref="Action"/> on write lock exit.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to invoke on write lock exit.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is a null reference.
        /// </exception>
        public void InvokeOnWriteLockExit(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var isFree = true;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (ReaderWriterLockSlim.IsWriteLockHeld)
                {
                    try
                    {
                        _lock.EnterWriteLock();
                        OnWriteLockFree += action;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    isFree = false;
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            if (isFree)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Invoke <see cref="Action"/> on lock exit.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to invoke on lock exit.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is a null reference.
        /// </exception>
        public void InvokeOnLockExit(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var isFree = true;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (ReaderWriterLockSlim.IsReadLockHeld ||
                    ReaderWriterLockSlim.IsUpgradeableReadLockHeld ||
                    ReaderWriterLockSlim.IsWriteLockHeld)

                {
                    try
                    {
                        _lock.EnterWriteLock();
                        OnLockFree += action;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    isFree = false;
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            if (isFree)
            {
                action.Invoke();
            }
        }

        private void TryInvokeOnReadLockExit()
        {
            Action? action = null;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (!ReaderWriterLockSlim.IsReadLockHeld)
                {
                    action = OnReadLockFree;
                    if (action != null)
                    {
                        try
                        {
                            _lock.EnterWriteLock();
                            OnReadLockFree = null;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            action?.Invoke();
        }

        private void TryInvokeOnUpgradeableReadLockExit()
        {
            Action? action = null;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (!ReaderWriterLockSlim.IsUpgradeableReadLockHeld)
                {
                    action = OnUpgradeableReadLockFree;
                    if (action != null)
                    {
                        try
                        {
                            _lock.EnterWriteLock();
                            OnUpgradeableReadLockFree = null;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            action?.Invoke();
        }

        private void TryInvokeOnWriteLockExit()
        {
            Action? action = null;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (!ReaderWriterLockSlim.IsWriteLockHeld)
                {
                    action = OnWriteLockFree;
                    if (action != null)
                    {
                        try
                        {
                            _lock.EnterWriteLock();
                            OnWriteLockFree = null;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            action?.Invoke();
        }

        private void TryInvokeOnLockExit()
        {
            Action? action = null;

            try
            {
                _lock.EnterUpgradeableReadLock();
                if (!ReaderWriterLockSlim.IsReadLockHeld &&
                    !ReaderWriterLockSlim.IsUpgradeableReadLockHeld &&
                    !ReaderWriterLockSlim.IsWriteLockHeld)
                {
                    action = OnLockFree;
                    if (action != null)
                    {
                        try
                        {
                            _lock.EnterWriteLock();
                            OnLockFree = null;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            action?.Invoke();
        }
    }
}
