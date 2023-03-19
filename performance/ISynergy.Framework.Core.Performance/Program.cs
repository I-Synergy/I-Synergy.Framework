using BenchmarkDotNet.Running;
using ISynergy.Framework.Core.Performance.Benchmarks;
using ISynergy.Framework.Core.Performance.Models;

namespace ISynergy.Framework.Core.Performance
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MemoryBenchMark<BasicModel>>();
            BenchmarkRunner.Run<MemoryBenchMark<EntityModel>>();
            BenchmarkRunner.Run<MemoryBenchMark<ObservableModel>>();
            Console.ReadLine();
        }
    }
}