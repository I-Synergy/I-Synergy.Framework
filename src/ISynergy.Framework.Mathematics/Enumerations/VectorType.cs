
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Enumerations;

/// <summary>
///   Vector types.
/// </summary>
/// 
public enum VectorType : int
{
    /// <summary>
    ///   The vector is a row vector, meaning it should have a size equivalent 
    ///   to [1 x N] where N is the number of elements in the vector.
    /// </summary>
    /// 
    RowVector = 0,

    /// <summary>
    ///   The vector is a column vector, meaning it should have a size equivalent
    ///   to [N x 1] where N is the number of elements in the vector.
    /// </summary>
    /// 
    ColumnVector = 1
}
