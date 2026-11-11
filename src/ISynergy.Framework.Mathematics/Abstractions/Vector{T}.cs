namespace ISynergy.Framework.Mathematics.Abstractions;

/// <summary>
///   A one-dimensional array viewed as a vector. The wrapper does not copy: it holds the
///   supplied array, so in-place operations keep their current semantics.
/// </summary>
/// <typeparam name="T">The type of the vector elements.</typeparam>
public readonly struct Vector<T>
{
    /// <summary>Initializes a new vector over <paramref name="array"/>.</summary>
    /// <param name="array">The array to wrap. It is not copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
    public Vector(T[] array)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
    }

    /// <summary>Gets the wrapped array.</summary>
    public T[] Array { get; }

    /// <summary>Gets the number of elements.</summary>
    public int Length => Array.Length;

    /// <summary>Gets or sets the element at the given index.</summary>
    /// <param name="index">The element index.</param>
    public T this[int index]
    {
        get => Array[index];
        set => Array[index] = value;
    }

    /// <summary>Returns the wrapped array.</summary>
    public static implicit operator T[](Vector<T> value) => value.Array;

    /// <summary>Returns a span over the wrapped array.</summary>
    public Span<T> AsSpan() => Array.AsSpan();

    /// <summary>Returns a read-only span over the wrapped array.</summary>
    public ReadOnlySpan<T> AsReadOnlySpan() => Array.AsSpan();
}
