using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Jagged
{
    /// <summary>
    ///     Converts the values of a matrix using the default converter.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The matrix to be converted.</param>
    public static TOutput[][] Convert<TInput, TOutput>(TInput[,] matrix)
    {
        return Convert(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
    }

    /// <summary>
    ///     Converts the values of a matrix using the default converter.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The matrix to be converted.</param>
    public static TOutput[][] Convert<TInput, TOutput>(this TInput[][] matrix)
    {
        return Convert(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
    }

    /// <summary>
    ///     Converts the values of a matrix using the given converter expression.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The vector to be converted.</param>
    /// <param name="converter">The converter function.</param>
    public static TOutput[][] Convert<TInput, TOutput>(this TInput[,] matrix, Converter<TInput, TOutput> converter)
    {
        var rows = matrix.Rows();
        var cols = matrix.Columns();

        var result = Zeros<TOutput>(rows, cols);
        for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                result[i][j] = converter(matrix[i, j]);

        return result;
    }

    /// <summary>
    ///     Converts the values of a matrix using the given converter expression.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The vector to be converted.</param>
    /// <param name="converter">The converter function.</param>
    public static TOutput[][] Convert<TInput, TOutput>(this TInput[][] matrix, Converter<TInput, TOutput> converter)
    {
        var rows = matrix.Rows();
        var cols = matrix.Columns();

        var result = Zeros<TOutput>(rows, cols);
        for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                result[i][j] = converter(matrix[i][j]);

        return result;
    }

    /// <summary>
    ///     Converts the values of a tensor.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="array">The tensor to be converted.</param>
    public static Array Convert<TOutput>(this Array array)
    {
        return Convert(array, typeof(TOutput));
    }

    /// <summary>
    ///     Converts the values of a tensor.
    /// </summary>
    /// <param name="type">The type of the output.</param>
    /// <param name="array">The tensor to be converted.</param>
    public static Array Convert(this Array array, Type type)
    {
        var r = Zeros(type, array.GetLength(true));

        foreach (var idx in r.GetIndices(true))
        {
            var value = ObjectExtensions.To(array.GetValue(true, idx), type);
            r.SetValue(value, true, idx);
        }

        return r;
    }
}