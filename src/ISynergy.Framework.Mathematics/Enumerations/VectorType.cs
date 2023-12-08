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
