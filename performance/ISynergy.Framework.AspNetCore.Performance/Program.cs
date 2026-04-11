using BenchmarkDotNet.Running;

namespace Sample.Api.Performance;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<ApiBenchmarks>();
    }
}
