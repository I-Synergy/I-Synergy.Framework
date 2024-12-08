namespace Sample.Api.Performance;
public class ThreadMetrics
{
    public int TotalThreads { get; set; }
    public int ActiveThreads { get; set; }
    public int ThreadPoolThreads { get; set; }
    public int CompletionPortThreads { get; set; }
    public int AvailableThreadPoolThreads { get; set; }
    public int AvailableCompletionPortThreads { get; set; }
    public int MinThreadPoolThreads { get; set; }
    public int MinCompletionPortThreads { get; set; }
    public int MaxThreadPoolThreads { get; set; }
    public int MaxCompletionPortThreads { get; set; }

    public override string ToString()
    {
        return $"""
                Thread Metrics:
                Total Threads: {TotalThreads}
                Active Threads: {ActiveThreads}
                Thread Pool Threads: {ThreadPoolThreads}
                Completion Port Threads: {CompletionPortThreads}
                Available Thread Pool Threads: {AvailableThreadPoolThreads}
                Available Completion Port Threads: {AvailableCompletionPortThreads}
                Min Thread Pool Threads: {MinThreadPoolThreads}
                Min Completion Port Threads: {MinCompletionPortThreads}
                Max Thread Pool Threads: {MaxThreadPoolThreads}
                Max Completion Port Threads: {MaxCompletionPortThreads}
                """;
    }
}