﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Locking
{
    public abstract class LockCleanupTests<T> : IClassFixture<T>, IDisposable
        where T : class, ILockServices
    {
        private readonly IServiceScope _scope;

        protected LockCleanupTests(T services)
        {
            var scopeFactory = services.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
            _scope = scopeFactory.CreateScope();
        }

        private IServiceProvider ServiceProvider => _scope.ServiceProvider;

        public void Dispose()
        {
            _scope.Dispose();
        }

        [Fact]
        public async Task TestCleanupOneAsync()
        {
            var releasedLocks = new HashSet<string>();
            var systemClock = (TestSystemClock)ServiceProvider.GetRequiredService<ISystemClock>();
            var lockManager = ServiceProvider.GetRequiredService<ILockManager>();
            var ct = CancellationToken.None;

            systemClock.RoundTo(DefaultLockTimeRoundingMode.OneSecond);
            await TestSingleLockAsync(releasedLocks, lockManager, ct);
        }

        [Fact]
        public async Task TestCleanupTwoAsync()
        {
            var releasedLocks = new HashSet<string>();
            var lockManager = ServiceProvider.GetRequiredService<ILockManager>();
            var ct = CancellationToken.None;
            var owner = new XElement("test");
            var l1 = new Lock("/", "/", false, owner, LockAccessType.Write, LockShareMode.Shared, TimeSpan.FromMilliseconds(100));
            var l2 = new Lock("/", "/", false, owner, LockAccessType.Write, LockShareMode.Shared, TimeSpan.FromMilliseconds(200));
            var evt = new CountdownEvent(2);
            lockManager.LockReleased += (s, e) =>
            {
                Assert.True(releasedLocks.Add(e.Lock.StateToken));
                evt.Signal();
            };

            var systemClock = (TestSystemClock)ServiceProvider.GetRequiredService<ISystemClock>();
            systemClock.RoundTo(DefaultLockTimeRoundingMode.OneSecond);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await lockManager.LockAsync(l1, ct);
            await lockManager.LockAsync(l2, ct);

            Assert.True(evt.Wait(2000, ct));
            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds >= 190, $"Duration should be at least 200ms, but was {stopwatch.ElapsedMilliseconds}");
        }

        [Fact]
        public async Task TestCleanupOneAfterOneAsync()
        {
            var releasedLocks = new HashSet<string>();
            var systemClock = (TestSystemClock)ServiceProvider.GetRequiredService<ISystemClock>();
            var lockManager = ServiceProvider.GetRequiredService<ILockManager>();
            var ct = CancellationToken.None;

            systemClock.RoundTo(DefaultLockTimeRoundingMode.OneSecond);
            var outerStopwatch = new Stopwatch();
            outerStopwatch.Start();

            await TestSingleLockAsync(releasedLocks, lockManager, ct);
            await TestSingleLockAsync(releasedLocks, lockManager, ct);

            outerStopwatch.Stop();
            Assert.True(
                outerStopwatch.ElapsedMilliseconds >= 200,
                $"Duration should be at least 200ms, but was {outerStopwatch.ElapsedMilliseconds}");
        }

        private async Task TestSingleLockAsync(ISet<string> releasedLocks, ILockManager lockManager, CancellationToken ct)
        {
            var l = new Lock(
                "/",
                "/",
                false,
                new XElement("test"),
                LockAccessType.Write,
                LockShareMode.Exclusive,
                TimeSpan.FromMilliseconds(100));
            var sem = new SemaphoreSlim(0, 1);
            var evt = new EventHandler<LockEventArgs>((s, e) =>
            {
                Assert.True(releasedLocks.Add(e.Lock.StateToken));
                sem.Release();
            });
            lockManager.LockReleased += evt;
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                await lockManager.LockAsync(l, ct);
                Assert.True(await sem.WaitAsync(5000, ct));
                stopwatch.Stop();
                Assert.True(
                    stopwatch.ElapsedMilliseconds >= 90,
                    $"Duration should be at least 100ms, but was {stopwatch.ElapsedMilliseconds}");
            }
            finally
            {
                lockManager.LockReleased -= evt;
            }
        }
    }
}
