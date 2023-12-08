namespace ISynergy.Framework.Mathematics;

public static partial class Matrix
{
    /// <summary>
    ///     Gets the number of rows in a jagged matrix.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
    /// <param name="values">The matrix whose number of rows must be computed.</param>
    /// <returns>The number of rows in the matrix.</returns>
    public static int Rows<T>(this IList<IList<T>> values)
    {
        return values.Count;
    }

    /// <summary>
    ///     Gets the number of columns in a jagged matrix.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
    /// <param name="values">The matrix whose number of rows must be computed.</param>
    /// <returns>The number of columns in the matrix.</returns>
    public static int Columns<T>(this IList<IList<T>> values)
    {
        return values[0].Count;
    }

    /// <summary>
    ///     Converts a matrix represented as a nested list of lists into a multi-dimensional matrix.
    /// </summary>
    public static T[,] ToMatrix<T, U>(this IList<IList<T>> values)
    {
        var rows = values.Rows();
        var cols = values.Columns();

        T[,] result = Matrix.Zeros<T>(rows, cols);
        for (var i = 0; i < values.Count; i++)
            for (var j = 0; j < values[i].Count; j++)
                result[i, j] = values[i][j];

        return result;
    }

    /// <summary>
    ///     Converts a matrix represented as a nested list of lists into a jagged matrix.
    /// </summary>
    public static T[][] ToJagged<T>(this IList<IList<T>> values)
    {
        var rows = values.Rows();
        var cols = values.Columns();

        var result = Jagged.Zeros<T>(rows, cols);
        for (var i = 0; i < values.Count; i++)
            for (var j = 0; j < values[i].Count; j++)
                result[i][j] = values[i][j];

        return result;
    }
}