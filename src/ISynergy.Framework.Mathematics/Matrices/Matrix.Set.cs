using ISynergy.Framework.Mathematics.Abstractions;
using ISynergy.Framework.Mathematics.Vectors;



namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The value to which matrix elements will be set.</param>
    /// 
    public static void Set<T>(this T[][] destination, T value)
    {
        Set(destination, new Jagged<T>(Jagged.Create(destination.Rows(), destination.Columns(), value)));
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The value to which matrix elements will be set.</param>
    /// 
    public static void Set<T>(this T[,] destination, T value)
    {
        Set(destination, new Matrix<T>(Matrix.Create(destination.Rows(), destination.Columns(), value)));
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// 
    public static void Set<T>(this T[][] destination, Jagged<T> value)
    {
        set(destination, null, value.Array, null, null);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// 
    public static void Set<T>(this T[,] destination, Matrix<T> value)
    {
        set(destination, null, value.Array, null, null);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="startColumn">Start column index</param>
    /// <param name="endColumn">End column index</param>
    /// 
    public static void Set<T>(this T[][] destination, Jagged<T> value, int[] rowIndices, int startColumn, int endColumn)
    {
        int endIndex = end(endColumn, destination.Columns());
        int[] columnIndices = Vector.Range(startColumn, endIndex);
        set(destination, columnIndices, value.Array, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="startColumn">Start column index</param>
    /// <param name="endColumn">End column index</param>
    /// 
    public static void Set<T>(this T[,] destination, Matrix<T> value, int[] rowIndices, int startColumn, int endColumn)
    {
        int endIndex = end(endColumn, destination.Columns());
        int[] columnIndices = Vector.Range(startColumn, endIndex);
        set(destination, columnIndices, value.Array, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="startColumn">Start column index</param>
    /// <param name="endColumn">End column index</param>
    /// 
    public static void Set<T>(this T[][] destination, T value, int[] rowIndices, int startColumn, int endColumn)
    {
        Set(destination, new Jagged<T>(Jagged.Create(destination.Rows(), destination.Columns(), value)), rowIndices, startColumn, endColumn);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="columnIndices">Array of column indices.</param>
    /// <param name="startRow">Start row index</param>
    /// <param name="endRow">End row index</param>
    /// 
    public static void Set<T>(this T[,] destination, Matrix<T> value, int startRow, int endRow, int[] columnIndices)
    {
        int endIndex = end(endRow, destination.Rows());
        int[] rowIndices = Vector.Range(startRow, endIndex);
        set(destination, columnIndices, value.Array, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Returns a sub matrix extracted from the current matrix.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="columnIndices">Array of column indices.</param>
    /// <param name="startRow">Start row index</param>
    /// <param name="endRow">End row index</param>
    /// 
    public static void Set<T>(this T[][] destination, Jagged<T> value, int startRow, int endRow, int[] columnIndices)
    {
        int endIndex = end(endRow, destination.Rows());
        int[] rowIndices = Vector.Range(startRow, endIndex);
        set(destination, columnIndices, value.Array, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="startColumn">Start column index</param>
    /// <param name="endColumn">End column index</param>
    /// 
    public static void Set<T>(this T[,] destination, T value, int[] rowIndices, int startColumn, int endColumn)
    {
        Matrix<T> source = new(Matrix.Create(destination.Rows(), destination.Columns(), value));
        Set(destination, source, rowIndices, startColumn, endColumn);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="startRow">Starting row index</param>
    /// <param name="endRow">End row index</param>
    /// <param name="columnIndices">Array of column indices</param>
    /// 
    public static void Set<T>(this T[][] destination, T value, int startRow, int endRow, int[] columnIndices)
    {
        Jagged<T> source = new(Jagged.Create(destination.Rows(), destination.Columns(), value));
        Set(destination, source, startRow, endRow, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="startRow">Starting row index</param>
    /// <param name="endRow">End row index</param>
    /// <param name="columnIndices">Array of column indices</param>
    /// 
    public static void Set<T>(this T[,] destination, T value, int startRow, int endRow, int[] columnIndices)
    {
        Matrix<T> source = new(Matrix.Create(destination.Rows(), destination.Columns(), value));
        Set(destination, source, startRow, endRow, columnIndices);
    }
    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="startRow">Starting row index</param>
    /// <param name="endRow">End row index</param>
    /// <param name="startCol">Starting column index</param>
    /// <param name="endCol">End column index</param>
    /// 
    public static void Set<T>(this T[][] destination, T value, int startRow, int endRow, int startCol, int endCol)
    {
        T[][] values = Jagged.Create<T>(destination.Rows(), destination.Columns(), value);
        int rowIndex = end(endRow, destination.Rows());
        int[] rowIndices = Vector.Range(startRow, rowIndex);
        int colIndex = end(endCol, destination.Columns());
        int[] columnIndices = Vector.Range(startCol, colIndex);
        set(destination, columnIndices, values, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="startRow">Starting row index</param>
    /// <param name="endRow">End row index</param>
    /// <param name="startCol">Starting column index</param>
    /// <param name="endCol">End column index</param>
    /// 
    public static void Set<T>(this T[,] destination, T value, int startRow, int endRow, int startCol, int endCol)
    {
        T[,] values = Matrix.Create<T>(destination.Rows(), destination.Columns(), value);
        int rowIndex = end(endRow, destination.Rows());
        int[] rowIndices = Vector.Range(startRow, rowIndex);
        int colIndex = end(endCol, destination.Columns());
        int[] columnIndices = Vector.Range(startCol, colIndex);
        set(destination, columnIndices, values, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="columnIndices">Array of column indices</param>
    /// 
    public static void Set<T>(this T[][] destination, T value, int[] rowIndices, int[] columnIndices)
    {
        var values = Jagged.Create<T>(destination.Rows(), destination.Columns(), value);
        set(destination, columnIndices, values, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="columnIndices">Array of column indices</param>
    /// 
    public static void Set<T>(this T[,] destination, T value, int[] rowIndices, int[] columnIndices)
    {
        var values = Matrix.Create<T>(destination.Rows(), destination.Columns(), value);
        set(destination, columnIndices, values, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="columnIndices">Array of column indices</param>
    /// 
    public static void Set<T>(this T[][] destination, Jagged<T> value, int[] rowIndices, int[] columnIndices)
    {
        set(destination, columnIndices, value.Array, rowIndices, columnIndices);
    }

    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="rowIndices">Array of row indices</param>
    /// <param name="columnIndices">Array of column indices</param>
    /// 
    public static void Set<T>(this T[,] destination, Matrix<T> value, int[] rowIndices, int[] columnIndices)
    {
        set(destination, columnIndices, value.Array, rowIndices, columnIndices);
    }
    /// <summary>
    ///   Sets a region of a matrix to the given values.
    /// </summary>
    /// 
    /// <param name="destination">The matrix where elements will be set.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="indices">Array of indices.</param>
    /// 
    public static void Set<T>(this T[] destination, T value, int[] indices)
    {
        ArgumentNullException.ThrowIfNull(destination);

        ArgumentNullException.ThrowIfNull(indices);

        for (var i = 0; i < indices.Length; i++)
            destination[Matrix.index(indices[i], destination.Length)] = value;
    }

    /// <summary>
    ///   Sets a subvector to the given value.
    /// </summary>
    /// 
    /// <param name="destination">The vector to return the subvector from.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="startRow">Starting index.</param>
    /// <param name="endRow">End index.</param>
    /// 
    public static void Set<T>(this T[] destination, T value, int startRow, int endRow)
    {
        endRow = end(endRow, destination.Length);
        for (var i = startRow; i < endRow; i++)
            destination[i] = value;
    }

    /// <summary>
    ///   Sets a subvector to the given value.
    /// </summary>
    /// 
    /// <param name="destination">The vector to return the subvector from.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="index">The index of the element to be set.</param>
    /// 
    public static void Set<T>(this T[] destination, T value, int index)
    {
        ArgumentNullException.ThrowIfNull(destination);

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, destination.Length);

        index = Matrix.index(index, destination.Length);

        destination[index] = value;
    }

    /// <summary>
    ///   Sets a subvector to the given value.
    /// </summary>
    /// 
    /// <param name="destination">The vector to return the subvector from.</param>
    /// <param name="value">The matrix of values to which matrix elements will be set.</param>
    /// <param name="indices">Array of indices.</param>
    /// 
    public static void Set<T>(this List<T> destination, T value, int[] indices)
    {
        ArgumentNullException.ThrowIfNull(destination);

        ArgumentNullException.ThrowIfNull(indices);

        for (var i = 0; i < indices.Length; i++)
            destination[indices[i]] = value;
    }

    // The destination row indices are always the same array as the source row indices
    // at every call site, and the mapping uses the source rows, so no separate
    // destination row index argument is required.
    private static void set<T>(this
        T[,] dst, int[] dstColumnIndices,
        T[,] src, int[] srcRowIndices, int[] srcColumnIndices)
    {
        ArgumentNullException.ThrowIfNull(src);

        ArgumentNullException.ThrowIfNull(dst);

        if (srcRowIndices is null)
            srcRowIndices = Vector.Range(0, src.Rows());

        if (srcColumnIndices is null)
            srcColumnIndices = Vector.Range(0, src.Columns());

        if (dstColumnIndices is null)
            dstColumnIndices = Vector.Range(0, dst.Columns());

        for (var i = 0; i < srcRowIndices.Length; i++)
        {
            int si = Matrix.index(srcRowIndices[i], src.Rows());
            int di = Matrix.index(srcRowIndices[i], dst.Rows());

            for (var j = 0; j < srcColumnIndices.Length; j++)
            {
                int sj = Matrix.index(srcColumnIndices[j], src.Columns());
                int dj = Matrix.index(dstColumnIndices[j], dst.Columns());
                dst[di, dj] = src[si, sj];
            }
        }
    }

    private static void set<T>(this
        T[][] dst, int[] dstColumnIndices,
        T[][] src, int[] srcRowIndices, int[] srcColumnIndices)
    {
        ArgumentNullException.ThrowIfNull(src);

        ArgumentNullException.ThrowIfNull(dst);

        if (srcRowIndices is null)
            srcRowIndices = Vector.Range(0, src.Rows());
        if (srcColumnIndices is null)
            srcColumnIndices = Vector.Range(0, src.Columns());

        if (dstColumnIndices is null)
            dstColumnIndices = Vector.Range(0, dst.Columns());

        for (var i = 0; i < srcRowIndices.Length; i++)
        {
            int si = Matrix.index(srcRowIndices[i], src.Rows());
            int di = Matrix.index(srcRowIndices[i], dst.Rows());

            for (var j = 0; j < srcColumnIndices.Length; j++)
            {
                int sj = Matrix.index(srcColumnIndices[j], src.Columns());
                int dj = Matrix.index(dstColumnIndices[j], dst.Columns());
                dst[di][dj] = src[si][sj];
            }
        }
    }

}
