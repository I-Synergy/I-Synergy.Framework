using BenchmarkDotNet.Attributes;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Performance.Benchmarks
{
    [MemoryDiagnoser]
    public class MemoryBenchMark<T>
        where T : class, new()
    {
        [Benchmark(Description = "List")]
        [Arguments(1)]
        [Arguments(10)]
        [Arguments(100)]
        [Arguments(1000)]
        //[Arguments(10000)]
        //[Arguments(100000)]
        public List<T> CreateList(int count)
        {
            List<T> result = new();
            for (int i = 0; i < count; i++)
            {
                result.Add(new T());
            }

            return result;
        }

        [Benchmark(Description = "Array")]
        [Arguments(1)]
        [Arguments(10)]
        [Arguments(100)]
        [Arguments(1000)]
        //[Arguments(10000)]
        //[Arguments(100000)]
        public T[] CreateArray2(int count)
        {
            T[] result = new T[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = new T();
            }

            return result;
        }

        [Benchmark(Description = "ObservableCollection")]
        [Arguments(1)]
        [Arguments(10)]
        [Arguments(100)]
        [Arguments(1000)]
        //[Arguments(10000)]
        //[Arguments(100000)]
        public ObservableCollection<T> CreateObservableCollection(int count)
        {
            ObservableCollection<T> result = new();
            for (int i = 0; i < count; i++)
            {
                result.Add(new T());
            }

            return result;
        }
    }
}
