using BenchmarkDotNet.Attributes;
using ISynergy.Framework.Core.Collections;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Performance.Benchmarks
{
    [MemoryDiagnoser]
    public class MemoryBenchMark<T>
        where T : class, new()
    {
        [Benchmark(Description = "List")]
        //[Arguments(1)]
        //[Arguments(10)]
        //[Arguments(100)]
        //[Arguments(1000)]
        [Arguments(10000)]
        public List<T> CreateList(int count)
        {
            var result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new T());
            }

            return result;
        }

        [Benchmark(Description = "Array")]
        //[Arguments(1)]
        //[Arguments(10)]
        //[Arguments(100)]
        //[Arguments(1000)]
        [Arguments(10000)]
        public T[] CreateArray2(int count)
        {
            var result = new T[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = new T();
            }

            return result;
        }

        [Benchmark(Description = "ObservableCollection")]
        //[Arguments(1)]
        //[Arguments(10)]
        //[Arguments(100)]
        //[Arguments(1000)]
        [Arguments(10000)]
        public ObservableCollection<T> CreateObservableCollection(int count)
        {
            var result = new ObservableCollection<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new T());
            }

            return result;
        }

        [Benchmark(Description = "ObservableConcurrentCollection")]
        //[Arguments(1)]
        //[Arguments(10)]
        //[Arguments(100)]
        //[Arguments(1000)]
        [Arguments(10000)]
        public ObservableConcurrentCollection<T> CreateObservableConcurrentCollection(int count)
        {
            var result = new ObservableConcurrentCollection<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new T());
            }

            return result;
        }
    }
}
