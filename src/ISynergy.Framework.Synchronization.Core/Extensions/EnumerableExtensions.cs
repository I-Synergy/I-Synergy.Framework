using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ISynergy.Framework.Synchronization.Core.Extensions
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Sorts an enumeration based on dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source enumeration.</param>
        /// <param name="dependencies">dependencies.</param>
        /// <param name="throwOnCycle">if <see langword="true"/> throw exception if Cyclic dependency found.</param>
        /// <param name="defaultCapacity">default capacity of sorterd buffer.</param>
        /// <returns></returns>
        public static IEnumerable<T> SortByDependencies<T>(this IEnumerable<T> source
            , Func<T, IEnumerable<T>> dependencies
            , bool throwOnCycle = false
            , int defaultCapacity = 10)
        {

            if (source is ICollection<T>)
            {
                defaultCapacity = ((ICollection<T>)source).Count + 1;
            }
            var sorted = new List<T>(defaultCapacity);
            var visited = new HashSet<T>();

            foreach (var item in source)
                Visit(item, visited, sorted, dependencies, throwOnCycle);

            return sorted;
        }

        private static void Visit<T>(T item
            , HashSet<T> visited
            , List<T> sorted, Func<T, IEnumerable<T>> dependencies
            , bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                    Visit(dep, visited, sorted, dependencies, throwOnCycle);

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle && !sorted.Contains(item))
                    throw new Exception("Cyclic dependency found");
            }
        }

        public static bool CompareWith<T>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, T, bool> compare)
        {
            // checking null ref
            if (source is null && other is not null || source is not null && other is null)
                return false;

            // If both are null, return true
            if (source is null && other is null)
                return true;

            if (source.Count() != other.Count())
                return false;

            // Check all items are identical
            return source.All(sourceItem => other.Any(otherItem => compare(sourceItem, otherItem)));

        }
        public static bool CompareWith<T>(this IEnumerable<T> source, IEnumerable<T> other) where T : class
        {
            // checking null ref
            if (source is null && other is not null || source is not null && other is null)
                return false;

            // If both are null, return true
            if (source is null && other is null)
                return true;

            if (source.Count() != other.Count())
                return false;

            // Check all items are identical
            return source.All(sourceItem => other.Any(otherItem =>
            {
                var cSourceItem = sourceItem as SyncNamedItem<T>;
                var cOtherItem = otherItem as SyncNamedItem<T>;

                if (cSourceItem is not null && cOtherItem is not null)
                    return cSourceItem.EqualsByProperties(otherItem);
                else
                    return sourceItem.Equals(otherItem);

            }));

        }

        public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };
            if (scheduler is not null)
                options.TaskScheduler = scheduler;

            var block = new ActionBlock<T>(body, options);

            foreach (var item in source)
                block.Post(item);

            block.Complete();
            return block.Completion;
        }
    }
}