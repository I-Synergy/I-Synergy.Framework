#if !NET35 && !NET40
using System.Diagnostics.CodeAnalysis;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


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
    [RequiresUnreferencedCode("Calls NpzDictionary<T>(Stream) which uses reflection-based type conversion.")]
    [RequiresDynamicCode("Calls NpzDictionary<T>(Stream) which requires dynamic code generation.")]
    public NpzDictionary(Stream stream, bool jagged)
        : base(stream)
    {
        this.jagged = jagged;
    }

    /// <summary>
    ///     Loads the array from the specified stream.
    /// </summary>
    [RequiresUnreferencedCode("Calls NpyFormat.LoadJagged(Stream) which is not AOT-safe.")]
    [RequiresDynamicCode("Calls NpyFormat.LoadJagged/LoadMatrix which require dynamic code generation.")]
    protected override Array Load(Stream s)
    {
        if (jagged)
            return NpyFormat.LoadJagged(s);
        return NpyFormat.LoadMatrix(s);
    }
}
#endif