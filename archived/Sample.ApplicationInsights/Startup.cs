﻿using Microsoft.Extensions.Logging;

namespace Sample.ApplicationInsights;

public class Startup(ILogger logger)
{
    /// <summary>
    /// run as an asynchronous operation.
    /// </summary>
    public void Run()
    {
        Console.WriteLine("Sample started...");

        logger.LogInformation("Log information...");
        Console.WriteLine("Log information...");
        Task.Delay(5000).Wait();

        logger.LogCritical("Log critical...");
        Console.WriteLine("Log critical...");
        Task.Delay(5000).Wait();

        logger.LogDebug("Log debug...");
        Console.WriteLine("Log debug...");
        Task.Delay(5000).Wait();

        logger.LogError("Log Error...", new Exception("Test"));
        Console.WriteLine("Log Error...");
        Task.Delay(5000).Wait();

        logger.LogTrace("Log Trace...");
        Console.WriteLine("Log Trace...");
        Task.Delay(5000).Wait();

        logger.LogWarning("Log Warning...");
        Console.WriteLine("Log Warning...");
        Task.Delay(5000).Wait();

        Console.WriteLine("Finished logging...");
        Console.WriteLine("Wait for a minute or press any key to end the processing");

        Task.Delay(5000).Wait();
    }
}
