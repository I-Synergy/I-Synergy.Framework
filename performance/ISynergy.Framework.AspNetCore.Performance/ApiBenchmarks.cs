using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;

namespace Sample.Api.Performance;

[MemoryDiagnoser]
public class ApiBenchmarks
{
    private readonly WebApplicationFactory<Sample.Api.Program> _factory;
    private readonly HttpClient _client;

    [Params(1, 10)]
    public int NumberOfRequests { get; set; }

    public ApiBenchmarks()
    {
        _factory = new WebApplicationFactory<Sample.Api.Program>();
        _client = _factory.CreateClient();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Benchmark(Description = "Async (All the way)")]
    public async Task GetEntitiesBenchmarkAsync()
    {
        var metricsBefore = CaptureThreadMetrics();
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var failCount = 0;
        var maxThreads = 0;

        Console.WriteLine($"\nStarting {NumberOfRequests} GET requests");
        Console.WriteLine(metricsBefore);

        var tasks = new List<Task>();
        for (int i = 0; i < NumberOfRequests; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var response = await _client.GetAsync("/asynchronous");
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref successCount);
                    else
                        Interlocked.Increment(ref failCount);

                    var currentThreads = CaptureThreadMetrics().TotalThreads;
                    Interlocked.Exchange(ref maxThreads, Math.Max(maxThreads, currentThreads));
                }
                catch
                {
                    Interlocked.Increment(ref failCount);
                }
            }));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();
        var metricsAfter = CaptureThreadMetrics();

        LogThreadMetricsChange("GET Requests", metricsBefore, metricsAfter);
        Console.WriteLine($"""
            Performance Metrics:
            Maximum threads: {maxThreads}
            Successful requests: {successCount}
            Failed requests: {failCount}
            Total time: {stopwatch.ElapsedMilliseconds}ms
            Average time per request: {stopwatch.ElapsedMilliseconds / (double)NumberOfRequests}ms
            """);
    }

    [Benchmark(Description = "Synchronous")]
    public void GetEntitiesBenchmark()
    {
        var metricsBefore = CaptureThreadMetrics();
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var failCount = 0;
        var maxThreads = 0;

        Console.WriteLine($"\nStarting {NumberOfRequests} GET requests");
        Console.WriteLine(metricsBefore);

        for (int i = 0; i < NumberOfRequests; i++)
        {
            try
            {
                var response = _client.GetAsync("/synchronous").Result;
                if (response.IsSuccessStatusCode)
                    Interlocked.Increment(ref successCount);
                else
                    Interlocked.Increment(ref failCount);

                var currentThreads = CaptureThreadMetrics().TotalThreads;
                Interlocked.Exchange(ref maxThreads, Math.Max(maxThreads, currentThreads));
            }
            catch
            {
                Interlocked.Increment(ref failCount);
            }
        }

        stopwatch.Stop();
        var metricsAfter = CaptureThreadMetrics();

        LogThreadMetricsChange("GET Requests", metricsBefore, metricsAfter);
        Console.WriteLine($"""
            Performance Metrics:
            Maximum threads: {maxThreads}
            Successful requests: {successCount}
            Failed requests: {failCount}
            Total time: {stopwatch.ElapsedMilliseconds}ms
            Average time per request: {stopwatch.ElapsedMilliseconds / (double)NumberOfRequests}ms
            """);
    }

    [Benchmark(Description = "Synchronous with Async (getawaiter, result)")]
    public void GetEntitiesBenchmarkBadAsync()
    {
        var metricsBefore = CaptureThreadMetrics();
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var failCount = 0;
        var maxThreads = 0;

        Console.WriteLine($"\nStarting {NumberOfRequests} GET requests");
        Console.WriteLine(metricsBefore);

        for (int i = 0; i < NumberOfRequests; i++)
        {
            try
            {
                var response = _client.GetAsync("/badasync").Result;
                if (response.IsSuccessStatusCode)
                    Interlocked.Increment(ref successCount);
                else
                    Interlocked.Increment(ref failCount);

                var currentThreads = CaptureThreadMetrics().TotalThreads;
                Interlocked.Exchange(ref maxThreads, Math.Max(maxThreads, currentThreads));
            }
            catch
            {
                Interlocked.Increment(ref failCount);
            }
        }

        stopwatch.Stop();
        var metricsAfter = CaptureThreadMetrics();

        LogThreadMetricsChange("GET Requests", metricsBefore, metricsAfter);
        Console.WriteLine($"""
            Performance Metrics:
            Maximum threads: {maxThreads}
            Successful requests: {successCount}
            Failed requests: {failCount}
            Total time: {stopwatch.ElapsedMilliseconds}ms
            Average time per request: {stopwatch.ElapsedMilliseconds / (double)NumberOfRequests}ms
            """);
    }


    public static ThreadMetrics CaptureThreadMetrics()
    {
        ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
        ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableCompletionPortThreads);

        var process = Process.GetCurrentProcess();
        var processThreads = process.Threads;
        var activeThreads = processThreads.Cast<ProcessThread>()
            .Count(t => t.ThreadState == System.Diagnostics.ThreadState.Running);

        return new ThreadMetrics
        {
            TotalThreads = processThreads.Count,
            ActiveThreads = activeThreads,
            ThreadPoolThreads = maxWorkerThreads - availableWorkerThreads,
            CompletionPortThreads = maxCompletionPortThreads - availableCompletionPortThreads,
            AvailableThreadPoolThreads = availableWorkerThreads,
            AvailableCompletionPortThreads = availableCompletionPortThreads,
            MinThreadPoolThreads = minWorkerThreads,
            MinCompletionPortThreads = minCompletionPortThreads,
            MaxThreadPoolThreads = maxWorkerThreads,
            MaxCompletionPortThreads = maxCompletionPortThreads
        };
    }

    public static void LogThreadMetricsChange(string operation, ThreadMetrics before, ThreadMetrics after)
    {
        Console.WriteLine($"""

                Thread Metrics for {operation}:
                ----------------------------------------
                Total Threads: {before.TotalThreads} -> {after.TotalThreads} (Δ{after.TotalThreads - before.TotalThreads})
                Active Threads: {before.ActiveThreads} -> {after.ActiveThreads} (Δ{after.ActiveThreads - before.ActiveThreads})
                Thread Pool Usage: {before.ThreadPoolThreads} -> {after.ThreadPoolThreads} (Δ{after.ThreadPoolThreads - before.ThreadPoolThreads})
                Completion Port Usage: {before.CompletionPortThreads} -> {after.CompletionPortThreads} (Δ{after.CompletionPortThreads - before.CompletionPortThreads})
                ----------------------------------------
                """);
    }
}