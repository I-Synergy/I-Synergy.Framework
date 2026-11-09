namespace ISynergy.Framework.Mathematics.Abstractions;

/// <summary>
///   A three-dimensional array viewed as a tensor. The wrapper does not copy: it holds the
///   supplied array, so in-place operations keep their current semantics.
/// </summary>
/// <typeparam name="T">The type of the tensor elements.</typeparam>
public readonly struct Tensor3<T>
{
    /// <summary>Initializes a new tensor over <paramref name="array"/>.</summary>
    /// <param name="array">The array to wrap. It is not copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="array"/> is not a <c>T[,,]</c>.</exception>
    public Tensor3(System.Array array)
    {
        Array = (T[,,])(array ?? throw new ArgumentNullException(nameof(array)));
    }

    /// <summary>Gets the wrapped array.</summary>
    public T[,,] Array { get; }

    /// <summary>Gets the length of the given dimension.</summary>
    /// <param name="dimension">The dimension, 0 to 2.</param>
    public int GetLength(int dimension) => Array.GetLength(dimension);

    /// <summary>Gets or sets the element at the given indices.</summary>
    /// <param name="first">The first index.</param>
    /// <param name="second">The second index.</param>
    /// <param name="third">The third index.</param>
    public T this[int first, int second, int third]
    {
        get => Array[first, second, third];
        set => Array[first, second, third] = value;
    }

    /// <summary>Returns the wrapped array.</summary>
    public static implicit operator T[,,](Tensor3<T> value) => value.Array;
}
