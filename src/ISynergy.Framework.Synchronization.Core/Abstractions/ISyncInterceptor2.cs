﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Abstractions
{
    public interface ISyncInterceptor2 : ISyncInterceptor
    {
        Task RunAsync(object args, CancellationToken cancellationToken);
    }
}