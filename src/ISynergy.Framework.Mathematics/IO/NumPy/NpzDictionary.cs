#if !NET35 && !NET40
namespace ISynergy.Framework.Mathematics.IO.NumPy;

/// <summary>
///     Lazily-loaded collection of arrays from a compressed .npz archive.
/// </summary>
/// <seealso cref="NpyFormat" />
/// <seealso cref="NpzFormat" />
public class NpzDictionary : NpzDictionary<Array>
{
    private readonly bool jagged;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NpzDictionary{T}" /> class.
    /// </summary>
    /// <param name="stream">The stream from where the arrays should be loaded from.</param>
    /// <param name="jagged">
    ///     Pass true to deserialize matrices as jagged matrices. Pass false
    ///     to deserialize them as multi-dimensional matrices.
    /// </param>
    public NpzDictionary(Stream stream, bool jagged)
        : base(stream)
    {
        this.jagged = jagged;
    }

    /// <summary>
    ///     Loads the array from the specified stream.
    /// </summary>
    protected override Array Load(Stream s)
    {
        if (jagged)
            return NpyFormat.LoadJagged(s);
        return NpyFormat.LoadMatrix(s);
    }

    /*
    public static bool TryRead(Stream stream, bool jagged, out NpzDictionary dictionary)
    {
        long offset = stream.Position;
        dictionary = null;

        try
        {
            dictionary = new NpzDictionary(stream, jagged);
            return true;
        }
        catch
        {
            stream.Position = offset;
        }

        return false;
    }

    public static bool TryRead<T>(Stream stream, bool jagged, out NpzDictionary<T> dictionary)
        where T : class, ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
    {
        long offset = stream.Position;
        dictionary = null;

        try
        {
            dictionary = new NpzDictionary<T>(stream);
            return true;
        }
        catch
        {
            stream.Position = offset;
        }

        return false;
    }
    */
}
#endif