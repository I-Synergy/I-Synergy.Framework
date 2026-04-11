namespace ISynergy.Framework.Mathematics.IO.Mat;

using System;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


/// <summary>
///   Sparse matrix representation used by
///   <see cref="MatReader">.MAT files</see>.
/// </summary>
/// 
public class MatSparse
{
    /// <summary>
    ///   Gets the sparse row index vector.
    /// </summary>
    /// 
    public int[] Rows { get; private set; }

    /// <summary>
    ///   Gets the sparse column index vector.
    /// </summary>
    /// 
    public int[] Columns { get; private set; }

    /// <summary>
    ///   Gets the values vector.
    /// </summary>
    /// 
    public Array Values { get; private set; }

    internal MatSparse(int[] ir, int[] ic, Array values)
    {
        Rows = ir;
        Columns = ic;
        Values = values;
    }
}
