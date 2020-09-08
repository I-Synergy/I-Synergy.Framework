namespace ISynergy.Framework.Core.Linq.Parsers.SupportedMethods
{
    /// <summary>
    /// Interface IEnumerableSignatures
    /// </summary>
    internal interface IEnumerableSignatures
    {
        /// <summary>
        /// Alls the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void All(bool predicate);
        /// <summary>
        /// Anies this instance.
        /// </summary>
        void Any();
        /// <summary>
        /// Anies the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void Any(bool predicate);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(decimal? selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(decimal selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(double? selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(double selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(float? selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(float selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(int? selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(int selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(long? selector);
        /// <summary>
        /// Averages the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Average(long selector);
        /// <summary>
        /// Casts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        void Cast(string type);
        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Contains(object selector);
        /// <summary>
        /// Counts this instance.
        /// </summary>
        void Count();
        /// <summary>
        /// Counts the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void Count(bool predicate);
        /// <summary>
        /// Defaults if empty.
        /// </summary>
        void DefaultIfEmpty();
        /// <summary>
        /// Defaults if empty.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        void DefaultIfEmpty(object defaultValue);
        /// <summary>
        /// Distincts this instance.
        /// </summary>
        void Distinct();
        /// <summary>
        /// Firsts this instance.
        /// </summary>
        void First();
        /// <summary>
        /// Firsts the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void First(bool predicate);
        /// <summary>
        /// Firsts the or default.
        /// </summary>
        void FirstOrDefault();
        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void FirstOrDefault(bool predicate);
        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        void GroupBy(object keySelector);
        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="elementSelector">The element selector.</param>
        void GroupBy(object keySelector, object elementSelector);
        /// <summary>
        /// Lasts this instance.
        /// </summary>
        void Last();
        /// <summary>
        /// Lasts the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void Last(bool predicate);
        /// <summary>
        /// Lasts the or default.
        /// </summary>
        void LastOrDefault();
        /// <summary>
        /// Lasts the or default.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void LastOrDefault(bool predicate);
        /// <summary>
        /// Longs the count.
        /// </summary>
        void LongCount();
        /// <summary>
        /// Longs the count.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void LongCount(bool predicate);
        /// <summary>
        /// Determines the maximum of the parameters.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Max(object selector);
        /// <summary>
        /// Determines the minimum of the parameters.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Min(object selector);
        /// <summary>
        /// Ofs the type.
        /// </summary>
        /// <param name="type">The type.</param>
        void OfType(string type);
        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void OrderBy(object selector);
        /// <summary>
        /// Orders the by descending.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void OrderByDescending(object selector);
        /// <summary>
        /// Selects the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Select(object selector);
        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void SelectMany(object selector);
        /// <summary>
        /// Singles this instance.
        /// </summary>
        void Single();
        /// <summary>
        /// Singles the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void Single(bool predicate);
        /// <summary>
        /// Singles the or default.
        /// </summary>
        void SingleOrDefault();
        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void SingleOrDefault(bool predicate);
        /// <summary>
        /// Skips the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        void Skip(int count);
        /// <summary>
        /// Skips the while.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void SkipWhile(bool predicate);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(decimal? selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(decimal selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(double? selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(double selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(float? selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(float selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(int? selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(int selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(long? selector);
        /// <summary>
        /// Sums the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Sum(long selector);
        /// <summary>
        /// Takes the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        void Take(int count);
        /// <summary>
        /// Takes the while.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void TakeWhile(bool predicate);
        /// <summary>
        /// Thens the by.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void ThenBy(object selector);
        /// <summary>
        /// Thens the by descending.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void ThenByDescending(object selector);
        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        void Where(bool predicate);

        // Executors
        /// <summary>
        /// Converts to array.
        /// </summary>
        void ToArray();
        /// <summary>
        /// Converts to list.
        /// </summary>
        void ToList();
    }
}
