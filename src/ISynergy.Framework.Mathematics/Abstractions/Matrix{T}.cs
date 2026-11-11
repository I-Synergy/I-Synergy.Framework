namespace ISynergy.Framework.Mathematics.Abstractions;

/// <summary>
///   A two-dimensional array viewed as a matrix. The wrapper does not copy: it holds the
///   supplied array, so in-place operations keep their current semantics.
/// </summary>
/// <typeparam name="T">The type of the matrix elements.</typeparam>
public readonly struct Matrix<T>
{
    /// <summary>Initializes a new matrix over <paramref name="array"/>.</summary>
    /// <param name="array">The array to wrap. It is not copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="array"/> is not a <c>T[,]</c>.</exception>
    public Matrix(System.Array array)
    {
        Array = (T[,])(array ?? throw new ArgumentNullException(nameof(array)));
    }

    /// <summary>Gets the wrapped array.</summary>
    public T[,] Array { get; }

    /// <summary>Gets the number of rows.</summary>
    public int Rows => Array.GetLength(0);

    /// <summary>Gets the number of columns.</summary>
    public int Columns => Array.GetLength(1);

    /// <summary>Gets the total number of elements.</summary>
    public int Length => Array.Length;

    /// <summary>Gets or sets the element at the given row and column.</summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
    public T this[int row, int column]
    {
        get => Array[row, column];
        set => Array[row, column] = value;
    }

    /// <summary>Returns the wrapped array.</summary>
    public static implicit operator T[,](Matrix<T> value) => value.Array;
}
