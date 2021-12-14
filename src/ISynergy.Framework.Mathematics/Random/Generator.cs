﻿using System;
using System.Diagnostics;
using System.Threading;

namespace ISynergy.Framework.Mathematics.Random
{
    /// <summary>
    ///     Framework-wide random number generator. If you would like to always generate
    ///     the same results when using the framework, set the <see cref="Seed" /> property
    ///     of this class to a fixed value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         By setting <see cref="Seed" /> to a given value, it is possible to adjust how
    ///         random numbers are generated within the framework. Preferably, this property
    ///         should be adjusted <b>before</b> other computations.
    ///     </para>
    ///     <para>
    ///         If the <see cref="Seed" /> is set to a value that is less than or equal to zero, all
    ///         generators will start with the same fixed seed, <b>even among multiple threads</b>.
    ///         If set to any other value, the generators in other threads will start with fixed, but
    ///         different, seeds.
    ///     </para>
    /// </remarks>
    public static class Generator
    {
        // Random generator used to seed other generators. It is used to prevent generators 
        // that have been created in short time spans to be initialized with the same seed.
        private static System.Random sourceRandom = new();
        private static readonly object sourceRandomLock = new();

        private static int? sourceSeed;
        private static int sourceLastUpdateTicks;
        private static readonly object sourceSeedLock = new();
        [ThreadStatic] private static int threadLastUpdateTicks;

        [ThreadStatic] private static bool threadOverriden;

        [ThreadStatic] private static int? threadSeed;

        [ThreadStatic] private static System.Random threadRandom;
        /// <summary>
        ///     Gets a value indicating whether the random number generator has been used
        ///     during the execution of any past code. This can be useful to determine whether
        ///     a method or learning algorithm is fully deterministic or not. Note that it is
        ///     also possible for a method to be non-deterministic even if it uses the random
        ///     number generator if it use multiple threads.
        /// </summary>
        public static bool HasBeenAccessed { get; set; }

        /// <summary>
        ///     Gets the timestamp for when the global random generator
        ///     was last changed (i.e. after setting <see cref="Seed" />).
        /// </summary>
        public static long LastUpdateTicks => sourceLastUpdateTicks;

        /// <summary>
        ///     Gets the timestamp for when the thread random generator was last
        ///     changed (i.e. after creating the first random generator in this
        ///     thread context or by setting <see cref="ThreadSeed" />).
        /// </summary>
        public static long ThreadLastUpdateTicks => threadLastUpdateTicks;

        /// <summary>
        ///     Gets or sets the seed for the current thread. Changing
        ///     this seed will not impact other threads or generators
        ///     that have already been created from this thread.
        /// </summary>
        public static int? ThreadSeed
        {
            get => threadSeed;
            set
            {
                threadSeed = value;

                if (threadSeed.HasValue)
                {
                    threadOverriden = true;
                    threadLastUpdateTicks = Environment.TickCount;
                    threadRandom = new System.Random(threadSeed.Value);
                }
                else
                {
                    threadOverriden = false;
                    threadRandom = null;
                }
            }
        }

        /// <summary>
        ///     Gets a reference to the random number generator used internally by
        ///     the ISynergy.Framework.Mathematics.NET classes and methods. Objects retrieved from this property
        ///     should not be shared across threads. Instead, call this property from
        ///     each thread you would like to use a random generator for.
        /// </summary>
        public static System.Random Random
        {
            get
            {
                HasBeenAccessed = true;

                if (threadOverriden)
                    return threadRandom;

                // No possibility of race condition here since its thread static
                if (threadRandom is null || threadLastUpdateTicks < sourceLastUpdateTicks)
                {
                    threadSeed = GetRandomSeed();
                    threadLastUpdateTicks = sourceLastUpdateTicks;
                    threadRandom = threadSeed.HasValue ? new System.Random(threadSeed.Value) : new System.Random();
                }

                return threadRandom;
            }
        }

        /// <summary>
        ///     Sets a random seed for the framework's main
        ///     <see cref="Random">
        ///         internal number
        ///         generator
        ///     </see>
        ///     . Preferably, this method should be called <b>before</b> other
        ///     computations. If set to a value less than or equal to zero, all generators will
        ///     start with the same fixed seed, <b>even among multiple threads</b>. If set to any
        ///     other value, the generators in other threads will start with fixed, but different,
        ///     seeds.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Adjusting the global generator seed causes the calling thread to sleep for 100ms
        ///         so new threads spawned in a short time span after the call can be properly initialized
        ///         with the new random seeds. In order to better control the random behavior of different
        ///         algorithms, please consider specifying random generators directly using appropriate
        ///         interfaces for these algorithms in case they are available.
        ///     </para>
        ///     <para>
        ///         If you do not need to change the seed number for threads other than the current,
        ///         you can adjust the random seed for the current thread using <see cref="ThreadSeed" />
        ///         instead. Setting <see cref="ThreadSeed" /> should not introduce delays.
        ///     </para>
        /// </remarks>
        public static int? Seed
        {
            get => sourceSeed;
            set
            {
                lock (sourceSeedLock)
                {
                    sourceSeed = value;

                    lock (sourceRandomLock)
                    {
                        sourceLastUpdateTicks = Environment.TickCount;

                        if (value.HasValue)
                        {
                            if (value.Value <= 0)
                            {
                                Trace.WriteLine("All threads will be initialized with the same seed: " + value);
                                sourceRandom = null;
                            }
                            else // value.Value > 0
                            {
                                Trace.WriteLine("All threads will be initialized with predictable, but random seeds.");
                                sourceRandom = new System.Random(value.Value);
                            }
                        }
                        else // value is null
                        {
                            Trace.WriteLine("All threads will be initialized with unpredictable random seeds.");
                            var s = unchecked((13 * Thread.CurrentThread.ManagedThreadId) ^ sourceLastUpdateTicks);
                            sourceRandom = new System.Random(s);
                        }

                        threadRandom = null;
                        Thread.Sleep(100); // Make sure the tick count has incremented before returning
                    }
                }
            }
        }
        private static int GetRandomSeed()
        {
            // We initialize new Random objects using the next value from a global 
            // static shared random generator in order to avoid creating many random 
            // objects with the random seed. This guarantees reproducibility but does
            // not compromise the effectiveness of parallel methods that depends on 
            // the generation of true random sequences with different values.
            lock (sourceRandomLock)
            {
                lock (sourceSeedLock)
                {
                    if (sourceRandom is null)
                    {
                        // There is no source random generator. This means we need to initialize the 
                        // generator for the current thread with a value that is (almost) unpredictable, 
                        // but still different from threads being initialized at almost the same time.

                        if (sourceSeed.HasValue)
                        {
                            if (sourceSeed.Value > 0)
                                return unchecked((13 * Thread.CurrentThread.ManagedThreadId) ^ sourceSeed.Value);
                            return sourceSeed.Value;
                        }

                        return unchecked((int)((13 * Thread.CurrentThread.ManagedThreadId) ^ DateTime.Now.Ticks));
                    }

                    return sourceRandom.Next(); // We have a source random generator
                }
            }
        }
    }
}