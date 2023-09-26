using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class AsyncEnumerableExtensions.
    /// </summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Converts to listasync.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            Argument.IsNotNull(source);

            return ExecuteAsync();

            async Task<List<T>> ExecuteAsync()
            {
                var list = new List<T>();

                await foreach (var element in source)
                {
                    list.Add(element);
                }

                return list;
            }
        }
    }
}
