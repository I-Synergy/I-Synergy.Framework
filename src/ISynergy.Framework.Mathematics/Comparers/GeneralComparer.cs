namespace ISynergy.Framework.Mathematics.Comparers
{
    /// <summary>
    ///     Directions for the General Comparer.
    /// </summary>
    public enum ComparerDirection
    {
        /// <summary>
        ///     Sorting will be performed in ascending order.
        /// </summary>
        Ascending = +1,

        /// <summary>
        ///     Sorting will be performed in descending order.
        /// </summary>
        Descending = -1
    }

    /// <summary>
    ///     General comparer which supports multiple
    ///     directions and comparison of absolute values.
    /// </summary>
    /// <example>
    ///     <code>
    ///   // Assume we have values to sort
    ///   double[] values = { 0, -5, 3, 1, 8 };
    ///   
    ///   // We can create an ad-hoc sorting rule considering only absolute values
    ///   Array.Sort(values, new GeneralComparer(ComparerDirection.Ascending, Math.Abs));
    ///   
    ///   // Result will be { 0, 1, 3, 5, 8 }.
    /// </code>
    /// </example>
    /// <seealso cref="ElementComparer{T}" />
    /// <seealso cref="ArrayComparer{T}" />
    /// <seealso cref="GeneralComparer" />
    /// <seealso cref="CustomComparer{T}" />
    public class GeneralComparer : IComparer<double>, IComparer<int>
    {
        private readonly Func<double, double> map;
        private int direction = 1;

        /// <summary>
        ///     Constructs a new General Comparer.
        /// </summary>
        /// <param name="direction">The direction to compare.</param>
        public GeneralComparer(ComparerDirection direction)
            : this(direction, false)
        {
        }

        /// <summary>
        ///     Constructs a new General Comparer.
        /// </summary>
        /// <param name="direction">The direction to compare.</param>
        /// <param name="useAbsoluteValues">True to compare absolute values, false otherwise. Default is false.</param>
        public GeneralComparer(ComparerDirection direction, bool useAbsoluteValues)
        {
            if (useAbsoluteValues)
                map = Math.Abs;
            else map = a => a;

            this.direction = (int)direction;
        }

        /// <summary>
        ///     Constructs a new General Comparer.
        /// </summary>
        /// <param name="direction">The direction to compare.</param>
        /// <param name="map">
        ///     The mapping function which will be applied to
        ///     each vector element prior to any comparisons.
        /// </param>
        public GeneralComparer(ComparerDirection direction, Func<double, double> map)
        {
            this.map = map;
            this.direction = (int)direction;
        }

        /// <summary>
        ///     Gets or sets the sorting direction
        ///     used by this comparer.
        /// </summary>
        public ComparerDirection Direction
        {
            get => (ComparerDirection)direction;
            set => direction = (int)value;
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than,
        ///     equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(double x, double y)
        {
            return direction * map(x).CompareTo(map(y));
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than,
        ///     equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(int x, int y)
        {
            return direction * map(x).CompareTo(map(y));
        }
    }

    /// <summary>
    ///     General comparer which supports multiple sorting directions.
    /// </summary>
    /// <example>
    ///     <code>
    ///   // Assume we have values to sort
    ///   double[] values = { 0, -5, 3, 1, 8 };
    ///   
    ///   // We can create an ad-hoc sorting rule
    ///   Array.Sort(values, new GeneralComparer&lt;double>(ComparerDirection.Descending));
    ///   
    ///   // Result will be { 8, 5, 3, 1, 0 }.
    /// </code>
    /// </example>
    /// <seealso cref="ElementComparer{T}" />
    /// <seealso cref="ArrayComparer{T}" />
    /// <seealso cref="GeneralComparer" />
    /// <seealso cref="CustomComparer{T}" />
    public class GeneralComparer<T> : IComparer<T> where T : IComparable<T>
    {
        private int direction = 1;

        /// <summary>
        ///     Constructs a new General Comparer.
        /// </summary>
        /// <param name="direction">The direction to compare.</param>
        public GeneralComparer(ComparerDirection direction)
        {
            this.direction = (int)direction;
        }

        /// <summary>
        ///     Gets or sets the sorting direction
        ///     used by this comparer.
        /// </summary>
        public ComparerDirection Direction
        {
            get => (ComparerDirection)direction;
            set => direction = (int)value;
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than,
        ///     equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(T x, T y)
        {
            return direction * x.CompareTo(y);
        }
    }
}