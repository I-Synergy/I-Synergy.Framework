namespace ISynergy.Framework.Mathematics.Abstractions;

/// <summary>
///   A jagged (array-of-arrays) matrix. The wrapper does not copy: it holds the supplied
///   arrays, so in-place operations keep their current semantics.
/// </summary>
/// <typeparam name="T">The type of the matrix elements.</typeparam>
public readonly struct Jagged<T>
{
    /// <summary>Initializes a new jagged matrix over <paramref name="array"/>.</summary>
    /// <param name="array">The array to wrap. It is not copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="array"/> is not a <c>T[][]</c>.</exception>
    public Jagged(System.Array array)
    {
        Array = (T[][])(array ?? throw new ArgumentNullException(nameof(array)));
    }

    /// <summary>Gets the wrapped array.</summary>
    public T[][] Array { get; }

    /// <summary>Gets the number of rows.</summary>
    public int Rows => Array.Length;

    /// <summary>Gets the number of elements in the given row.</summary>
    /// <param name="row">The row index.</param>
    public int Columns(int row) => Array[row].Length;

    /// <summary>Gets the elements of the given row.</summary>
    /// <param name="row">The row index.</param>
    public T[] this[int row] => Array[row];

    /// <summary>Returns the wrapped array.</summary>
    public static implicit operator T[][](Jagged<T> value) => value.Array;
}
