using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mathematics.Abstractions;
using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Vectors;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;



namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{
    /// <summary>
    ///     Converts a jagged-array into a multidimensional array.
    /// </summary>
    public static Array DeepToMatrix(this Array array)
    {
        var shape = array.GetLength();
        var totalLength = array.GetTotalLength();
        var elementType = array.GetInnerMostType();

        var flat = array.DeepFlatten();

        var result = Array.CreateInstance(elementType, shape);
        Buffer.BlockCopy(flat, 0, result, 0, totalLength);
        return result;
    }

    /// <summary>
    ///     Converts a jagged-array into a multidimensional array.
    /// </summary>
    public static T[,] ToMatrix<T>(this Jagged<T> array, bool transpose = false)
    {
        var arrayArray = array.Array;
        var rows = arrayArray.Length;
        if (rows == 0) return new T[0, rows];
        var cols = arrayArray[0].Length;

        T[,] m;

        if (transpose)
        {
            m = new T[cols, rows];
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    m[j, i] = arrayArray[i][j];
        }
        else
        {
            m = new T[rows, cols];
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    m[i, j] = arrayArray[i][j];
        }

        return m;
    }

    /// <summary>
    ///     Converts an array into a multidimensional array.
    /// </summary>
    public static T[,] ToMatrix<T>(this T[] array, bool asColumnVector = false)
    {
        if (asColumnVector)
        {
            var m = new T[array.Length, 1];
            for (var i = 0; i < array.Length; i++)
                m[i, 0] = array[i];
            return m;
        }
        else
        {
            var m = new T[1, array.Length];
            for (var i = 0; i < array.Length; i++)
                m[0, i] = array[i];
            return m;
        }
    }



    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static double[,] ToMatrix(this DataTable table)
    {
        return ToMatrix<double>(table);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static double[,] ToMatrix(this DataTable table, out string[] columnNames)
    {
        return ToMatrix<double>(table, out columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static double[,] ToMatrix(this DataTable table, IFormatProvider provider)
    {
        return ToMatrix<double>(table, provider);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static double[,] ToMatrix(this DataTable table, params string[] columnNames)
    {
        return ToMatrix<double>(table, columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static T[,] ToMatrix<T>(this DataTable table, IFormatProvider provider)
    {
        string[] names;
        return ToMatrix<T>(table, out names, provider);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static T[,] ToMatrix<T>(this DataTable table)
    {
        string[] names;
        return ToMatrix<T>(table, out names);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static T[,] ToMatrix<T>(this DataTable table, out string[] columnNames)
    {
        var m = new T[table.Rows.Count, table.Columns.Count];
        columnNames = new string[table.Columns.Count];

        for (var j = 0; j < table.Columns.Count; j++)
        {
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var obj = table.Rows[i][j];
                m[i, j] = (T)System.Convert.ChangeType(obj, typeof(T));
            }

            columnNames[j] = table.Columns[j].Caption;
        }

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static T[,] ToMatrix<T>(this DataTable table, out string[] columnNames, IFormatProvider provider)
    {
        var m = new T[table.Rows.Count, table.Columns.Count];
        columnNames = new string[table.Columns.Count];

        for (var j = 0; j < table.Columns.Count; j++)
        {
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var obj = table.Rows[i][j];
                m[i, j] = (T)System.Convert.ChangeType(obj, typeof(T), provider);
            }

            columnNames[j] = table.Columns[j].Caption;
        }

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static T[,] ToMatrix<T>(this DataTable table, params string[] columnNames)
    {
        var m = new T[table.Rows.Count, columnNames.Length];

        for (var j = 0; j < columnNames.Length; j++)
            for (var i = 0; i < table.Rows.Count; i++)
                m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][columnNames[j]], typeof(T));

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static T[,] ToMatrix<T>(this DataTable table, IFormatProvider provider, params string[] columnNames)
    {
        var m = new T[table.Rows.Count, columnNames.Length];

        for (var j = 0; j < columnNames.Length; j++)
            for (var i = 0; i < table.Rows.Count; i++)
                m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][columnNames[j]], typeof(T), provider);

        return m;
    }

    /// <summary>
    ///     Converts an array into a multidimensional array.
    /// </summary>
    public static T[][] ToJagged<T>(this T[] array, bool asColumnVector = true)
    {
        if (asColumnVector)
        {
            var m = new T[array.Length][];
            for (var i = 0; i < array.Length; i++)
                m[i] = [array[i]];
            return m;
        }

        return [array];
    }

    /// <summary>
    ///     Converts a multidimensional array into a jagged array.
    /// </summary>
    public static T[][] ToJagged<T>(this Matrix<T> matrix, bool transpose = false)
    {
        var matrixArray = matrix.Array;
        T[][] array;

        if (transpose)
        {
            var cols = matrixArray.GetLength(1);

            array = new T[cols][];
            for (var i = 0; i < cols; i++)
                array[i] = matrixArray.GetColumn(i);
        }
        else
        {
            var rows = matrixArray.GetLength(0);

            array = new T[rows][];
            for (var i = 0; i < rows; i++)
                array[i] = matrixArray.GetRow(i);
        }

        return array;
    }

    /// <summary>
    ///     Converts a multidimensional array into a jagged array.
    /// </summary>
    public static T[][][] ToJagged<T>(this Tensor3<T> matrix)
    {
        var matrixArray = matrix.Array;
        var rows = matrixArray.GetLength(0);
        var cols = matrixArray.GetLength(1);
        var depth = matrixArray.GetLength(2);

        var array = new T[rows][][];
        for (var i = 0; i < rows; i++)
        {
            var row = array[i] = new T[cols][];
            for (var j = 0; j < row.Length; j++)
            {
                var plane = row[j] = new T[depth];
                for (var k = 0; k < plane.Length; k++)
                    plane[k] = matrixArray[i, j, k];
            }
        }

        return array;
    }
    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[][] ToJagged(this DataTable table)
    {
        return ToJagged<double>(table);
    }

    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[][] ToJagged(this DataTable table, IFormatProvider provider)
    {
        return ToJagged<double>(table, provider);
    }

    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[][] ToJagged(this DataTable table, out string[] columnNames)
    {
        return ToJagged<double>(table, out columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[][] ToJagged(this DataTable table, IFormatProvider provider, out string[] columnNames)
    {
        return ToJagged<double>(table, provider, out columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[][] ToJagged(this DataTable table, params string[] columnNames)
    {
        return ToJagged<double>(table, columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a T[][] array.
    /// </summary>
    public static T[][] ToJagged<T>(this DataTable table)
    {
        string[] names;
        return ToJagged<T>(table, out names);
    }

    /// <summary>
    ///     Converts a DataTable to a T[][] array.
    /// </summary>
    public static T[][] ToJagged<T>(this DataTable table, IFormatProvider provider)
    {
        string[] names;
        return ToJagged<T>(table, provider, out names);
    }

    /// <summary>
    ///     Converts a DataTable to a T[][] array.
    /// </summary>
    public static T[][] ToJagged<T>(this DataTable table, out string[] columnNames)
    {
        var m = new T[table.Rows.Count][];
        columnNames = new string[table.Columns.Count];

        for (var i = 0; i < table.Rows.Count; i++)
            m[i] = new T[table.Columns.Count];

        for (var j = 0; j < table.Columns.Count; j++)
        {
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var value = table.Rows[i][j];
                m[i][j] = (T)System.Convert.ChangeType(value, typeof(T));
            }

            columnNames[j] = table.Columns[j].Caption;
        }

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a T[][] array.
    /// </summary>
    public static T[][] ToJagged<T>(this DataTable table, IFormatProvider provider, out string[] columnNames)
    {
        var m = new T[table.Rows.Count][];
        columnNames = new string[table.Columns.Count];

        for (var i = 0; i < table.Rows.Count; i++)
            m[i] = new T[table.Columns.Count];

        for (var j = 0; j < table.Columns.Count; j++)
        {
            for (var i = 0; i < table.Rows.Count; i++)
                m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][j], typeof(T), provider);

            columnNames[j] = table.Columns[j].Caption;
        }

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a T[][] array.
    /// </summary>
    public static T[][] ToJagged<T>(this DataTable table, params string[] columnNames)
    {
        var m = new T[table.Rows.Count][];

        for (var i = 0; i < table.Rows.Count; i++)
            m[i] = new T[columnNames.Length];

        for (var j = 0; j < columnNames.Length; j++)
        {
            var col = table.Columns[columnNames[j]];

            for (var i = 0; i < table.Rows.Count; i++)
                m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));
        }

        return m;
    }

    /// <summary>
    ///     Creates a vector containing every index that can be used to
    ///     address a given <paramref name="array" />, in order.
    /// </summary>
    /// <param name="array">The array whose indices will be returned.</param>
    /// <param name="deep">
    ///     Pass true to retrieve all dimensions of the array,
    ///     even if it contains nested arrays (as in jagged matrices).
    /// </param>
    /// <param name="max">
    ///     Bases computations on the maximum length possible for
    ///     each dimension (in case the jagged matrices has different lengths).
    /// </param>
    /// <param name="order">
    ///     The direction to access the matrix. Pass 1 to read the
    ///     matrix in row-major order. Pass 0 to read in column-major order. Default is
    ///     1 (row-major, c-style order).
    /// </param>
    /// <returns>
    ///     An enumerable object that can be used to iterate over all
    ///     positions of the given <paramref name="array">System.Array</paramref>.
    /// </returns>
    /// <example>
    ///     <code>
    ///   double[,] a = 
    ///   { 
    ///      { 5.3, 2.3 },
    ///      { 4.2, 9.2 }
    ///   };
    ///   
    ///   foreach (int[] idx in a.GetIndices())
    ///   {
    ///      // Get the current element
    ///      double e = (double)a.GetValue(idx);
    ///   }
    /// </code>
    /// </example>
    /// <seealso cref="Vector.GetIndices{T}(T[])" />
    public static IEnumerable<int[]> GetIndices(this Array array, bool deep = false, bool max = false,
        MatrixOrder order = MatrixOrder.Default)
    {
        return array.GetLength(deep, max)
            .Sequences(firstColumnChangesFaster: order == MatrixOrder.FortranColumnMajor);
    }


    /// <summary>
    ///     Converts the values of a vector using the given converter expression.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="vector">The vector to be converted.</param>
    public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector)
    {
        return Convert(vector, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
    }

    /// <summary>
    ///     Converts the values of a matrix using the default converter.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The matrix to be converted.</param>
    public static TOutput[,] Convert<TInput, TOutput>(this Matrix<TInput> matrix)
    {
        return Convert(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
    }

    /// <summary>
    ///     Converts the values of a matrix using the default converter.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The matrix to be converted.</param>
    public static TOutput[,] Convert<TInput, TOutput>(Jagged<TInput> matrix)
    {
        return Convert<TInput, TOutput>(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
    }

    /// <summary>
    ///     Converts the values of a vector using the given converter expression.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="vector">The vector to be converted.</param>
    /// <param name="converter">The converter function.</param>
    public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector,
        Converter<TInput, TOutput>
            converter)
    {
        return Array.ConvertAll(vector, converter);
    }

    /// <summary>
    ///     Converts the values of a matrix using the given converter expression.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The matrix to be converted.</param>
    /// <param name="converter">The converter function.</param>
    public static TOutput[,] Convert<TInput, TOutput>(this Jagged<TInput> matrix,
        Converter<TInput, TOutput>
            converter)
    {
        var matrixArray = matrix.Array;
        var result = Matrix.CreateAs<TInput, TOutput>(matrixArray);

        for (var i = 0; i < matrixArray.Length; i++)
            for (var j = 0; j < matrixArray[i].Length; j++)
                result[i, j] = converter(matrixArray[i][j]);

        return result;
    }

    /// <summary>
    ///     Converts the values of a matrix using the given converter expression.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="matrix">The vector to be converted.</param>
    /// <param name="converter">The converter function.</param>
    public static TOutput[,] Convert<TInput, TOutput>(this Matrix<TInput> matrix,
        Converter<TInput, TOutput>
            converter)
    {
        var matrixArray = matrix.Array;
        var rows = matrixArray.GetLength(0);
        var cols = matrixArray.GetLength(1);

        var result = new TOutput[rows, cols];
        for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                result[i, j] = converter(matrixArray[i, j]);

        return result;
    }

    /// <summary>
    ///     Converts an object into another type, irrespective of whether
    ///     the conversion can be done at compile time or not. This can be
    ///     used to convert generic types to numeric types during runtime.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="array">The vector or array to be converted.</param>
    public static TOutput To<TOutput>(this Array array)
    {
        return To(array, typeof(TOutput)).To<TOutput>();
    }

    /// <summary>
    ///     Converts an object into another type, irrespective of whether
    ///     the conversion can be done at compile time or not. This can be
    ///     used to convert generic types to numeric types during runtime.
    /// </summary>
    /// <param name="array">The vector or array to be converted.</param>
    /// <param name="outputType">The type of the output.</param>
    public static object To(this Array array, Type outputType)
    {
        if (array.GetType() == outputType)
            return array;

        if (!outputType.IsArray && array.Length == 1)
            // The array has exactly one element; all its indices are zero.
            return array.GetValue(new int[array.Rank]).To(outputType);

        Array result;

        if (outputType.IsJagged())
            // multidimensional or jagged -> jagged
            result = Jagged.CreateAs(array, outputType);
        else
            // multidimensional or jagged -> multidimensional
            result = Matrix.CreateAs(array, outputType);

        Copy(array, result);

        return result;
    }

    /// <summary>
    ///     Copies elements from an array to another array even if one
    ///     is a jagged array and the other a multidimensional array.
    /// </summary>
    /// <param name="source">The array whose elements should be copied from.</param>
    /// <param name="destination">The array where elements will be written to.</param>
    public static void Copy(this Array source, Array destination)
    {
        var outputElementType = destination.GetInnerMostType();

        if (source.GetType() == destination.GetType() && source.IsMatrix() && destination.IsMatrix())
        {
            if (outputElementType.IsPrimitive)
                Buffer.BlockCopy(source, 0, destination, 0, source.Length * Marshal.SizeOf(outputElementType));
            else
                Array.Copy(source, destination, source.Length);
        }
        else
        {
            var deep = true;

            if (destination.GetLength().Contains(-1))
            {
                // Result is a jagged array where not all dimensions have been specified
                var a = source.GetIndices(deep).GetEnumerator();

                while (a.MoveNext())
                {
                    var inputValue = source.GetValue(deep, a.Current);
                    var outputValue = convertValue(outputElementType, inputValue);
                    destination.SetValue(outputValue, deep, a.Current);
                }
            }
            else
            {
                // Both matrices have been completely specified
                var a = source.GetIndices(deep).GetEnumerator();
                var b = destination.GetIndices(deep).GetEnumerator();

                while (a.MoveNext() && b.MoveNext())
                {
                    var inputValue = source.GetValue(deep, a.Current);
                    var outputValue = convertValue(outputElementType, inputValue);
                    destination.SetValue(outputValue, deep, b.Current);
                }
            }
        }
    }
    /// <summary>
    ///     Gets the value at the specified position in the multidimensional System.Array.
    ///     The indexes are specified as an array of 32-bit integers.
    /// </summary>
    /// <param name="array">A jagged or multidimensional array.</param>
    /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
    /// <param name="indices">
    ///     A one-dimensional array of 32-bit integers that represent the
    ///     indexes specifying the position of the System.Array element to get.
    /// </param>
    public static object GetValue(this Array array, bool deep, int[] indices)
    {
        if (array.IsVector())
            return array.GetValue(indices);

        if (deep && array.IsJagged())
        {
            var current = array.GetValue(indices[0]) as Array;
            if (indices.Length == 1)
                return current;
            var last = indices.Get(1, 0);
            return GetValue(current, true, last);
        }

        return array.GetValue(indices);
    }

    /// <summary>
    ///     Gets the value at the specified position in the multidimensional System.Array.
    ///     The indexes are specified as an array of 32-bit integers.
    /// </summary>
    /// <param name="array">A jagged or multidimensional array.</param>
    /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
    /// <param name="indices">
    ///     A one-dimensional array of 32-bit integers that represent the
    ///     indexes specifying the position of the System.Array element to get.
    /// </param>
    /// <param name="value">The value retrieved from the array.</param>
    public static bool TryGetValue(this Array array, bool deep, int[] indices, out object value)
    {
        value = null;

        if (array.IsVector())
        {
            if (indices.Length > array.Rank)
                return false;

            for (var i = 0; i < indices.Length; i++)
                if (indices[i] > array.GetUpperBound(i))
                    return false;

            value = array.GetValue(indices);
            return true;
        }

        if (deep && array.IsJagged())
        {
            var current = array.GetValue(indices[0]) as Array;
            if (indices.Length == 1)
            {
                value = current;
                return true;
            }

            var last = indices.Get(1, 0);
            return TryGetValue(current, true, last, out value);
        }

        value = array.GetValue(indices);
        return true;
    }

    /// <summary>
    ///     Sets a value to the element at the specified position in the multidimensional
    ///     or jagged System.Array. The indexes are specified as an array of 32-bit integers.
    /// </summary>
    /// <param name="array">A jagged or multidimensional array.</param>
    /// <param name="value">The new value for the specified element.</param>
    /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
    /// <param name="indices">
    ///     A one-dimensional array of 32-bit integers that represent
    ///     the indexes specifying the position of the element to set.
    /// </param>
    public static void SetValue(this Array array, object value, bool deep, int[] indices)
    {
        if (deep && array.IsJagged())
        {
            var current = array.GetValue(indices[0]) as Array;
            var last = indices.Get(1, 0);
            var length = last.Length == 0 ? 1 : Enumerable.Max(last) + 1;
            if (current is null || current.Length < length)
            {
                if (current is null)
                {
                    current = Array.CreateInstance(array.GetType().GetElementType(), length);
                }
                else
                {
                    var r = Array.CreateInstance(array.GetType().GetElementType(), length);
                    current.CopyTo(r, 0);
                    current = r;
                }
            }

            SetValue(current, value, true, last);
        }
        else
        {
            array.SetValue(value, indices);
        }
    }

    private static object convertValue(Type outputElementType, object inputValue)
    {
        var inputArray = inputValue as Array;
        if (inputArray is not null)
            return To(inputArray, outputElementType);
        return inputValue.To(outputElementType);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static DataTable ToTable(this Matrix<double> matrix)
    {
        var cols = matrix.Array.GetLength(1);

        var columnNames = new string[cols];
        for (var i = 0; i < columnNames.Length; i++)
            columnNames[i] = "Column " + i;
        return ToTable(matrix, columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static DataTable ToTable(this Matrix<double> matrix, params string[] columnNames)
    {
        var table = new DataTable();
        table.Locale = CultureInfo.CurrentCulture;

        var rows = matrix.Array.GetLength(0);
        var cols = matrix.Array.GetLength(1);

        for (var i = 0; i < cols; i++)
            table.Columns.Add(columnNames[i], typeof(double));

        for (var i = 0; i < rows; i++)
            table.Rows.Add(matrix.Array.GetRow(i).Convert(x => (object)x));

        return table;
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static DataTable ToTable(this Jagged<double> matrix)
    {
        var cols = matrix.Array[0].Length;

        var columnNames = new string[cols];
        for (var i = 0; i < columnNames.Length; i++)
            columnNames[i] = "Column " + i;
        return ToTable(matrix, columnNames);
    }

    /// <summary>
    ///     Converts a DataTable to a double[,] array.
    /// </summary>
    public static DataTable ToTable(this Jagged<double> matrix, params string[] columnNames)
    {
        var table = new DataTable();
        table.Locale = CultureInfo.CurrentCulture;

        for (var i = 0; i < columnNames.Length; i++)
            table.Columns.Add(columnNames[i], typeof(double));

        for (var i = 0; i < matrix.Array.Length; i++)
            table.Rows.Add(matrix.Array[i].Convert(x => (object)x));

        return table;
    }

    /// <summary>
    ///     Converts an array of values into a <see cref="DataTable" />,
    ///     attempting to guess column types by inspecting the data.
    /// </summary>
    /// <param name="values">The values to be converted.</param>
    /// <returns>A <see cref="DataTable" /> containing the given values.</returns>
    /// <example>
    ///     <code>
    /// // Specify some data in a table format
    /// //
    /// object[,] data = 
    /// {
    ///     { "Id", "IsSmoker", "Age" },
    ///     {   0,       1,        10  },
    ///     {   1,       1,        15  },
    ///     {   2,       0,        40  },
    ///     {   3,       1,        20  },
    ///     {   4,       0,        70  },
    ///     {   5,       0,        55  },
    /// };
    /// 
    /// // Create a new table with the data
    /// DataTable dataTable = data.ToTable();
    /// </code>
    /// </example>
    public static DataTable ToTable(this Matrix<object> values)
    {
        var columnNames = new string[values.Array.Columns()];
        for (var i = 0; i < columnNames.Length; i++)
            columnNames[i] = "Column " + i;
        return ToTable(values, columnNames);
    }

    /// <summary>
    ///     Converts an array of values into a <see cref="DataTable" />,
    ///     attempting to guess column types by inspecting the data.
    /// </summary>
    /// <param name="matrix">The values to be converted.</param>
    /// <param name="columnNames">The column names to use in the data table.</param>
    /// <returns>A <see cref="DataTable" /> containing the given values.</returns>
    /// <example>
    ///     <code>
    /// // Specify some data in a table format
    /// //
    /// object[,] data = 
    /// {
    ///     { "Id", "IsSmoker", "Age" },
    ///     {   0,       1,        10  },
    ///     {   1,       1,        15  },
    ///     {   2,       0,        40  },
    ///     {   3,       1,        20  },
    ///     {   4,       0,        70  },
    ///     {   5,       0,        55  },
    /// };
    /// 
    /// // Create a new table with the data
    /// DataTable dataTable = data.ToTable();
    /// </code>
    /// </example>
    public static DataTable ToTable(this Matrix<object> matrix, string[] columnNames)
    {
        var matrixArray = matrix.Array;
        var table = new DataTable();
        table.Locale = CultureInfo.InvariantCulture;

        var headers = matrixArray.GetRow(0);

        if (headers.All(x => x is string))
        {
            // Get first data row to set types
            var first = matrixArray.GetRow(1);

            // Assume first row is header row
            for (var i = 0; i < first.Length; i++)
                table.Columns.Add(headers[i] as string, first[i].GetType());

            // Parse all the other rows
            var rows = matrixArray.GetLength(0);
            for (var i = 1; i < rows; i++)
            {
                var row = matrixArray.GetRow(i);
                table.Rows.Add(row);
            }
        }
        else
        {
            for (var i = 0; i < matrixArray.Columns(); i++)
            {
                var columnType = GetHighestEnclosingType(matrixArray.GetColumn(i));
                table.Columns.Add(columnNames[i], columnType);
            }

            var rows = matrixArray.GetLength(0);
            for (var i = 0; i < rows; i++)
            {
                var row = matrixArray.GetRow(i);
                table.Rows.Add(row);
            }
        }

        return table;
    }

    private static Type GetHighestEnclosingType(object[] values)
    {
        var types = values.Select(x => x is not null ? x.GetType() : null);
        if (types.Any(x => x == typeof(object)))
            return typeof(object);
        if (types.Any(x => x == typeof(string)))
            return typeof(string);
        if (types.Any(x => x == typeof(decimal)))
            return typeof(decimal);
        if (types.Any(x => x == typeof(double)))
            return typeof(double);
        if (types.Any(x => x == typeof(float)))
            return typeof(float);
        if (types.Any(x => x == typeof(int)))
            return typeof(int);
        if (types.Any(x => x == typeof(uint)))
            return typeof(uint);
        if (types.Any(x => x == typeof(short)))
            return typeof(int);
        if (types.Any(x => x == typeof(byte)))
            return typeof(int);
        if (types.Any(x => x == typeof(sbyte)))
            return typeof(int);

        return typeof(object);
    }
    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[] ToVector(this DataTable table)
    {
        return ToVector<double>(table);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static double[] ToVector(this DataTable table, IFormatProvider provider)
    {
        return ToVector<double>(table, provider);
    }

    /// <summary>
    ///     Converts a DataTable to a double[][] array.
    /// </summary>
    public static double[] ToVector(this DataTable table, out string columnName)
    {
        return ToVector<double>(table, out columnName);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static double[] ToVector(this DataTable table, IFormatProvider provider, out string columnName)
    {
        return ToVector<double>(table, provider, out columnName);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static double[] ToVector(this DataTable table, string columnName)
    {
        return ToVector<double>(table, columnName);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static T[] ToVector<T>(this DataTable table)
    {
        string name;
        return ToVector<T>(table, out name);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static T[] ToVector<T>(this DataTable table, IFormatProvider provider)
    {
        string name;
        return ToVector<T>(table, provider, out name);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static T[] ToVector<T>(this DataTable table, out string columnName)
    {
        if (table.Columns.Count > 1)
            throw new ArgumentException(
                "The given table has more than one column. Please specify which column should be converted.");

        columnName = table.Columns[0].ColumnName;
        return table.Columns[0].ToArray<T>();
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static T[] ToVector<T>(this DataTable table, IFormatProvider provider, out string columnName)
    {
        if (table.Columns.Count > 1)
            throw new ArgumentException(
                "The given table has more than one column. Please specify which column should be converted.");

        columnName = table.Columns[0].ColumnName;
        return table.Columns[0].ToArray<T>(provider);
    }

    /// <summary>
    ///     Converts a DataTable to a double[] array.
    /// </summary>
    public static T[] ToVector<T>(this DataTable table, string columnName)
    {
        return table.Columns[columnName].ToArray<T>();
    }
    /// <summary>
    ///     Converts a DataColumn to a double[] array.
    /// </summary>
    public static double[] ToArray(this DataColumn column)
    {
        return ToArray<double>(column);
    }

    /// <summary>
    ///     Converts a DataColumn to a double[] array.
    /// </summary>
    public static double[] ToArray(this DataColumn column, IFormatProvider provider)
    {
        return ToArray<double>(column, provider);
    }

    /// <summary>
    ///     Converts a DataColumn to a double[] array.
    /// </summary>
    public static double[] ToArray(this DataRow row, IFormatProvider provider, params string[] colNames)
    {
        return ToArray<double>(row, provider, colNames);
    }

    /// <summary>
    ///     Converts a DataColumn to a double[] array.
    /// </summary>
    public static double[] ToArray(this DataRow row, params string[] colNames)
    {
        return ToArray<double>(row, colNames);
    }

    /// <summary>
    ///     Converts a DataColumn to a double[] array.
    /// </summary>
    public static T[] ToArray<T>(this DataColumn column)
    {
        var m = new T[column.Table.Rows.Count];

        for (var i = 0; i < m.Length; i++)
            m[i] = (T)System.Convert.ChangeType(column.Table.Rows[i][column], typeof(T));

        return m;
    }

    /// <summary>
    ///     Converts a DataColumn to a double[] array.
    /// </summary>
    public static T[] ToArray<T>(this DataColumn column, IFormatProvider provider)
    {
        var m = new T[column.Table.Rows.Count];

        for (var i = 0; i < m.Length; i++)
            m[i] = (T)System.Convert.ChangeType(column.Table.Rows[i][column], typeof(T), provider);

        return m;
    }

    /// <summary>
    ///     Converts a DataColumn to a generic array.
    /// </summary>
    public static T[] ToArray<T>(this DataRow row, params string[] colNames)
    {
        var m = new T[colNames.Length];

        for (var i = 0; i < m.Length; i++)
            m[i] = (T)System.Convert.ChangeType(row[colNames[i]], typeof(T));

        return m;
    }

    /// <summary>
    ///     Converts a DataColumn to a generic array.
    /// </summary>
    public static T[] ToArray<T>(this DataRow row, IFormatProvider provider, params string[] colNames)
    {
        var m = new T[colNames.Length];

        for (var i = 0; i < m.Length; i++)
            m[i] = (T)System.Convert.ChangeType(row[colNames[i]], typeof(T), provider);

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a generic array.
    /// </summary>
    public static T[] ToArray<T>(this DataTable table, string columnName)
    {
        var m = new T[table.Rows.Count];

        var col = table.Columns[columnName];
        for (var i = 0; i < table.Rows.Count; i++)
            m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));

        return m;
    }

    /// <summary>
    ///     Converts a DataTable to a generic array.
    /// </summary>
    public static T[] ToArray<T>(this DataTable table, IFormatProvider provider, string columnName)
    {
        var m = new T[table.Rows.Count];

        var col = table.Columns[columnName];
        for (var i = 0; i < table.Rows.Count; i++)
            m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T), provider);

        return m;
    }

    /// <summary>
    ///   Converts a integer to a boolean.
    /// </summary>
    /// 
    public static bool[] ToBoolean(this int[] value)
    {
        return ToBoolean(value, new bool[value.Length]);
    }

    /// <summary>
    ///   Converts a integer array to a boolean array.
    /// </summary>
    /// 
    public static bool[] ToBoolean(this int[] value, bool[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = value[i] != 0;
        return result;
    }
    /// <summary>
    ///   Converts a string to a boolean.
    /// </summary>
    /// 
    public static bool[] ToBoolean(this string[] value)
    {
        return ToBoolean(value, new bool[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a boolean.
    /// </summary>
    /// 
    public static bool[,] ToBoolean(this Matrix<string> value)
    {
        return ToBoolean(value, new Matrix<bool>(Matrix.CreateAs<string, bool>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a boolean.
    /// </summary>
    /// 
    public static bool[,,] ToBoolean(this Tensor3<string> value)
    {
        return ToBoolean(value, new Tensor3<bool>(Matrix.CreateAs<string, bool>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a boolean.
    /// </summary>
    /// 
    public static bool[][] ToBoolean(this Jagged<string> value)
    {
        return ToBoolean(value, new Jagged<bool>(Jagged.CreateAs<string, bool>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a boolean.
    /// </summary>
    /// 
    public static bool[][][] ToBoolean(this string[][][] value)
    {
        return ToBoolean(value, Jagged.CreateAs<string, bool>(value));
    }




    /// <summary>
    ///   Converts a string array to a boolean array.
    /// </summary>
    /// 
    public static bool[] ToBoolean(this string[] value, bool[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Boolean.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional boolean array.
    /// </summary>
    /// 
    public static bool[,] ToBoolean(this Matrix<string> value, Matrix<bool> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Boolean.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional boolean array.
    /// </summary>
    /// 
    public static bool[,,] ToBoolean(this Tensor3<string> value, Tensor3<bool> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Boolean.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged boolean array.
    /// </summary>
    /// 
    public static bool[][] ToBoolean(this Matrix<string> value, Jagged<bool> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Boolean.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged boolean array.
    /// </summary>
    /// 
    public static bool[][] ToBoolean(this Jagged<string> value, Jagged<bool> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Boolean.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged boolean array.
    /// </summary>
    /// 
    public static bool[][][] ToBoolean(this string[][][] value, bool[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Boolean.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional boolean array.
    /// </summary>
    /// 
    public static bool[,] ToBoolean(this Jagged<string> value, Matrix<bool> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Boolean.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a boolean to a integer.
    /// </summary>
    /// 
    public static int[] ToInt32(this bool[] value)
    {
        return ToInt32(value, new int[value.Length]);
    }

    /// <summary>
    ///   Converts a boolean array to a integer array.
    /// </summary>
    /// 
    public static int[] ToInt32(this bool[] value, int[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = value[i] ? 1 : 0;
        return result;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a integer.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Jagged<double> value)
    {
        return ToInt32(value, new Jagged<int>(Jagged.CreateAs<double, int>(value.Array)));
    }

    /// <summary>
    ///   Converts a boolean to a integer.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Jagged<bool> value)
    {
        return ToInt32(value, new Jagged<int>(Jagged.CreateAs<bool, int>(value.Array)));
    }

    /// <summary>
    ///   Converts a jagged double-precision floating point array to a jagged integer array.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Jagged<double> value, Jagged<int> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = (Int32)value.Array[i][j];
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged boolean array to a jagged integer array.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Jagged<bool> value, Jagged<int> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = value.Array[i][j] ? 1 : 0;
        return result.Array;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a integer.
    /// </summary>
    /// 
    public static int[,] ToInt32(this Matrix<double> value)
    {
        return ToInt32(value, new Matrix<int>(Matrix.CreateAs<double, int>(value.Array)));
    }

    /// <summary>
    ///   Converts a multidimensional double-precision floating point array to a multidimensional integer array.
    /// </summary>
    /// 
    public static int[,] ToInt32(this Matrix<double> value, Matrix<int> result)
    {
        int rows = value.Array.GetLength(0);
        int cols = value.Array.GetLength(1);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result.Array[i, j] = (Int32)value.Array[i, j];

        return result.Array;
    }

    /// <summary>
    ///   Converts a string to a integer.
    /// </summary>
    /// 
    public static int[] ToInt32(this string[] value)
    {
        return ToInt32(value, new int[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a integer.
    /// </summary>
    /// 
    public static int[,] ToInt32(this Matrix<string> value)
    {
        return ToInt32(value, new Matrix<int>(Matrix.CreateAs<string, int>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a integer.
    /// </summary>
    /// 
    public static int[,,] ToInt32(this Tensor3<string> value)
    {
        return ToInt32(value, new Tensor3<int>(Matrix.CreateAs<string, int>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a integer.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Jagged<string> value)
    {
        return ToInt32(value, new Jagged<int>(Jagged.CreateAs<string, int>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a integer.
    /// </summary>
    /// 
    public static int[][][] ToInt32(this string[][][] value)
    {
        return ToInt32(value, Jagged.CreateAs<string, int>(value));
    }

    /// <summary>
    ///   Converts a string array to a integer array.
    /// </summary>
    /// 
    public static int[] ToInt32(this string[] value, int[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Int32.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional integer array.
    /// </summary>
    /// 
    public static int[,] ToInt32(this Matrix<string> value, Matrix<int> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Int32.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional integer array.
    /// </summary>
    /// 
    public static int[,,] ToInt32(this Tensor3<string> value, Tensor3<int> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Int32.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged integer array.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Matrix<string> value, Jagged<int> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Int32.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged integer array.
    /// </summary>
    /// 
    public static int[][] ToInt32(this Jagged<string> value, Jagged<int> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Int32.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged integer array.
    /// </summary>
    /// 
    public static int[][][] ToInt32(this string[][][] value, int[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Int32.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional integer array.
    /// </summary>
    /// 
    public static int[,] ToInt32(this Jagged<string> value, Matrix<int> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Int32.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a single-precision floating point.
    /// </summary>
    /// 
    public static float[][] ToSingle(this Jagged<double> value)
    {
        return ToSingle(value, new Jagged<float>(Jagged.CreateAs<double, float>(value.Array)));
    }

    /// <summary>
    ///   Converts a jagged double-precision floating point array to a jagged single-precision floating point array.
    /// </summary>
    /// 
    public static float[][] ToSingle(this Jagged<double> value, Jagged<float> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = (Single)value.Array[i][j];
        return result.Array;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a single-precision floating point.
    /// </summary>
    /// 
    public static float[,,] ToSingle(this Tensor3<double> value)
    {
        return ToSingle(value, new Tensor3<float>(Matrix.CreateAs<double, float>(value.Array)));
    }

    /// <summary>
    ///   Converts a multidimensional double-precision floating point array to a multidimensional single-precision floating point array.
    /// </summary>
    /// 
    public static float[,,] ToSingle(this Tensor3<double> value, Tensor3<float> result)
    {
        int d0 = value.Array.GetLength(0);
        int d1 = value.Array.GetLength(1);
        int d2 = value.Array.GetLength(2);
        for (int i = 0; i < d0; i++)
            for (int j = 0; j < d1; j++)
                for (int k = 0; k < d2; k++)
                    result.Array[i, j, k] = (Single)value.Array[i, j, k];

        return result.Array;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a single-precision floating point.
    /// </summary>
    /// 
    public static float[,] ToSingle(this Matrix<double> value)
    {
        return ToSingle(value, new Matrix<float>(Matrix.CreateAs<double, float>(value.Array)));
    }

    /// <summary>
    ///   Converts a multidimensional double-precision floating point array to a multidimensional single-precision floating point array.
    /// </summary>
    /// 
    public static float[,] ToSingle(this Matrix<double> value, Matrix<float> result)
    {
        int rows = value.Array.GetLength(0);
        int cols = value.Array.GetLength(1);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result.Array[i, j] = (Single)value.Array[i, j];

        return result.Array;
    }

    /// <summary>
    ///   Converts a string to a single-precision floating point.
    /// </summary>
    /// 
    public static float[,] ToSingle(this Matrix<string> value)
    {
        return ToSingle(value, new Matrix<float>(Matrix.CreateAs<string, float>(value.Array)));
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional single-precision floating point array.
    /// </summary>
    /// 
    public static float[,] ToSingle(this Matrix<string> value, Matrix<float> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Single.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a string to a single-precision floating point.
    /// </summary>
    /// 
    public static float[] ToSingle(this string[] value)
    {
        return ToSingle(value, new float[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a single-precision floating point.
    /// </summary>
    /// 
    public static float[,,] ToSingle(this Tensor3<string> value)
    {
        return ToSingle(value, new Tensor3<float>(Matrix.CreateAs<string, float>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a single-precision floating point.
    /// </summary>
    /// 
    public static float[][] ToSingle(this Jagged<string> value)
    {
        return ToSingle(value, new Jagged<float>(Jagged.CreateAs<string, float>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a single-precision floating point.
    /// </summary>
    /// 
    public static float[][][] ToSingle(this string[][][] value)
    {
        return ToSingle(value, Jagged.CreateAs<string, float>(value));
    }




    /// <summary>
    ///   Converts a string array to a single-precision floating point array.
    /// </summary>
    /// 
    public static float[] ToSingle(this string[] value, float[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Single.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional single-precision floating point array.
    /// </summary>
    /// 
    public static float[,,] ToSingle(this Tensor3<string> value, Tensor3<float> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Single.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged single-precision floating point array.
    /// </summary>
    /// 
    public static float[][] ToSingle(this Matrix<string> value, Jagged<float> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Single.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged single-precision floating point array.
    /// </summary>
    /// 
    public static float[][] ToSingle(this Jagged<string> value, Jagged<float> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Single.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged single-precision floating point array.
    /// </summary>
    /// 
    public static float[][][] ToSingle(this string[][][] value, float[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Single.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional single-precision floating point array.
    /// </summary>
    /// 
    public static float[,] ToSingle(this Jagged<string> value, Matrix<float> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Single.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a short integer.
    /// </summary>
    /// 
    public static short[,] ToInt16(this Matrix<double> value)
    {
        return ToInt16(value, new Matrix<short>(Matrix.CreateAs<double, short>(value.Array)));
    }

    /// <summary>
    ///   Converts a multidimensional double-precision floating point array to a multidimensional short integer array.
    /// </summary>
    /// 
    public static short[,] ToInt16(this Matrix<double> value, Matrix<short> result)
    {
        int rows = value.Array.GetLength(0);
        int cols = value.Array.GetLength(1);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result.Array[i, j] = (Int16)value.Array[i, j];

        return result.Array;
    }
    /// <summary>
    ///   Converts a string to a short integer.
    /// </summary>
    /// 
    public static short[] ToInt16(this string[] value)
    {
        return ToInt16(value, new short[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a short integer.
    /// </summary>
    /// 
    public static short[,] ToInt16(this Matrix<string> value)
    {
        return ToInt16(value, new Matrix<short>(Matrix.CreateAs<string, short>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a short integer.
    /// </summary>
    /// 
    public static short[,,] ToInt16(this Tensor3<string> value)
    {
        return ToInt16(value, new Tensor3<short>(Matrix.CreateAs<string, short>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a short integer.
    /// </summary>
    /// 
    public static short[][] ToInt16(this Jagged<string> value)
    {
        return ToInt16(value, new Jagged<short>(Jagged.CreateAs<string, short>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a short integer.
    /// </summary>
    /// 
    public static short[][][] ToInt16(this string[][][] value)
    {
        return ToInt16(value, Jagged.CreateAs<string, short>(value));
    }




    /// <summary>
    ///   Converts a string array to a short integer array.
    /// </summary>
    /// 
    public static short[] ToInt16(this string[] value, short[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Int16.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional short integer array.
    /// </summary>
    /// 
    public static short[,] ToInt16(this Matrix<string> value, Matrix<short> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Int16.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional short integer array.
    /// </summary>
    /// 
    public static short[,,] ToInt16(this Tensor3<string> value, Tensor3<short> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Int16.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged short integer array.
    /// </summary>
    /// 
    public static short[][] ToInt16(this Matrix<string> value, Jagged<short> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Int16.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged short integer array.
    /// </summary>
    /// 
    public static short[][] ToInt16(this Jagged<string> value, Jagged<short> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Int16.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged short integer array.
    /// </summary>
    /// 
    public static short[][][] ToInt16(this string[][][] value, short[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Int16.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional short integer array.
    /// </summary>
    /// 
    public static short[,] ToInt16(this Jagged<string> value, Matrix<short> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Int16.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a double-precision floating point to a decimal fixed-point.
    /// </summary>
    /// 
    public static decimal[] ToDecimal(this double[] value)
    {
        return ToDecimal(value, new decimal[value.Length]);
    }

    /// <summary>
    ///   Converts a double-precision floating point array to a decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[] ToDecimal(this double[] value, decimal[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = (Decimal)value[i];
        return result;
    }
    /// <summary>
    ///   Converts a string to a decimal fixed-point.
    /// </summary>
    /// 
    public static decimal[] ToDecimal(this string[] value)
    {
        return ToDecimal(value, new decimal[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a decimal fixed-point.
    /// </summary>
    /// 
    public static decimal[,] ToDecimal(this Matrix<string> value)
    {
        return ToDecimal(value, new Matrix<decimal>(Matrix.CreateAs<string, decimal>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a decimal fixed-point.
    /// </summary>
    /// 
    public static decimal[,,] ToDecimal(this Tensor3<string> value)
    {
        return ToDecimal(value, new Tensor3<decimal>(Matrix.CreateAs<string, decimal>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a decimal fixed-point.
    /// </summary>
    /// 
    public static decimal[][] ToDecimal(this Jagged<string> value)
    {
        return ToDecimal(value, new Jagged<decimal>(Jagged.CreateAs<string, decimal>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a decimal fixed-point.
    /// </summary>
    /// 
    public static decimal[][][] ToDecimal(this string[][][] value)
    {
        return ToDecimal(value, Jagged.CreateAs<string, decimal>(value));
    }




    /// <summary>
    ///   Converts a string array to a decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[] ToDecimal(this string[] value, decimal[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Decimal.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[,] ToDecimal(this Matrix<string> value, Matrix<decimal> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Decimal.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[,,] ToDecimal(this Tensor3<string> value, Tensor3<decimal> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Decimal.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[][] ToDecimal(this Matrix<string> value, Jagged<decimal> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Decimal.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[][] ToDecimal(this Jagged<string> value, Jagged<decimal> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Decimal.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[][][] ToDecimal(this string[][][] value, decimal[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Decimal.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional decimal fixed-point array.
    /// </summary>
    /// 
    public static decimal[,] ToDecimal(this Jagged<string> value, Matrix<decimal> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Decimal.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a string to a long integer.
    /// </summary>
    /// 
    public static long[] ToInt64(this string[] value)
    {
        return ToInt64(value, new long[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a long integer.
    /// </summary>
    /// 
    public static long[,] ToInt64(this Matrix<string> value)
    {
        return ToInt64(value, new Matrix<long>(Matrix.CreateAs<string, long>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a long integer.
    /// </summary>
    /// 
    public static long[,,] ToInt64(this Tensor3<string> value)
    {
        return ToInt64(value, new Tensor3<long>(Matrix.CreateAs<string, long>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a long integer.
    /// </summary>
    /// 
    public static long[][] ToInt64(this Jagged<string> value)
    {
        return ToInt64(value, new Jagged<long>(Jagged.CreateAs<string, long>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a long integer.
    /// </summary>
    /// 
    public static long[][][] ToInt64(this string[][][] value)
    {
        return ToInt64(value, Jagged.CreateAs<string, long>(value));
    }

    /// <summary>
    ///   Converts a string array to a long integer array.
    /// </summary>
    /// 
    public static long[] ToInt64(this string[] value, long[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Int64.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional long integer array.
    /// </summary>
    /// 
    public static long[,] ToInt64(this Matrix<string> value, Matrix<long> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Int64.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional long integer array.
    /// </summary>
    /// 
    public static long[,,] ToInt64(this Tensor3<string> value, Tensor3<long> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Int64.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged long integer array.
    /// </summary>
    /// 
    public static long[][] ToInt64(this Matrix<string> value, Jagged<long> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Int64.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged long integer array.
    /// </summary>
    /// 
    public static long[][] ToInt64(this Jagged<string> value, Jagged<long> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Int64.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged long integer array.
    /// </summary>
    /// 
    public static long[][][] ToInt64(this string[][][] value, long[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Int64.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional long integer array.
    /// </summary>
    /// 
    public static long[,] ToInt64(this Jagged<string> value, Matrix<long> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Int64.Parse(value.Array[i][j]);
        return result.Array;
    }
    /// <summary>
    ///   Converts a string to a 8-bit byte.
    /// </summary>
    /// 
    public static byte[] ToByte(this string[] value)
    {
        return ToByte(value, new byte[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a 8-bit byte.
    /// </summary>
    /// 
    public static byte[,] ToByte(this Matrix<string> value)
    {
        return ToByte(value, new Matrix<byte>(Matrix.CreateAs<string, byte>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a 8-bit byte.
    /// </summary>
    /// 
    public static byte[,,] ToByte(this Tensor3<string> value)
    {
        return ToByte(value, new Tensor3<byte>(Matrix.CreateAs<string, byte>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a 8-bit byte.
    /// </summary>
    /// 
    public static byte[][] ToByte(this Jagged<string> value)
    {
        return ToByte(value, new Jagged<byte>(Jagged.CreateAs<string, byte>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a 8-bit byte.
    /// </summary>
    /// 
    public static byte[][][] ToByte(this string[][][] value)
    {
        return ToByte(value, Jagged.CreateAs<string, byte>(value));
    }




    /// <summary>
    ///   Converts a string array to a 8-bit byte array.
    /// </summary>
    /// 
    public static byte[] ToByte(this string[] value, byte[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = Byte.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional 8-bit byte array.
    /// </summary>
    /// 
    public static byte[,] ToByte(this Matrix<string> value, Matrix<byte> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = Byte.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional 8-bit byte array.
    /// </summary>
    /// 
    public static byte[,,] ToByte(this Tensor3<string> value, Tensor3<byte> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = Byte.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged 8-bit byte array.
    /// </summary>
    /// 
    public static byte[][] ToByte(this Matrix<string> value, Jagged<byte> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = Byte.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged 8-bit byte array.
    /// </summary>
    /// 
    public static byte[][] ToByte(this Jagged<string> value, Jagged<byte> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = Byte.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged 8-bit byte array.
    /// </summary>
    /// 
    public static byte[][][] ToByte(this string[][][] value, byte[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Byte.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional 8-bit byte array.
    /// </summary>
    /// 
    public static byte[,] ToByte(this Jagged<string> value, Matrix<byte> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = Byte.Parse(value.Array[i][j]);
        return result.Array;
    }
    /// <summary>
    ///   Converts a string to a signed 7-bit byte.
    /// </summary>
    /// 
    public static sbyte[] ToSByte(this string[] value)
    {
        return ToSByte(value, new sbyte[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a signed 7-bit byte.
    /// </summary>
    /// 
    public static sbyte[,] ToSByte(this Matrix<string> value)
    {
        return ToSByte(value, new Matrix<sbyte>(Matrix.CreateAs<string, sbyte>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a signed 7-bit byte.
    /// </summary>
    /// 
    public static sbyte[,,] ToSByte(this Tensor3<string> value)
    {
        return ToSByte(value, new Tensor3<sbyte>(Matrix.CreateAs<string, sbyte>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a signed 7-bit byte.
    /// </summary>
    /// 
    public static sbyte[][] ToSByte(this Jagged<string> value)
    {
        return ToSByte(value, new Jagged<sbyte>(Jagged.CreateAs<string, sbyte>(value.Array)));
    }

    /// <summary>
    ///   Converts a string to a signed 7-bit byte.
    /// </summary>
    /// 
    public static sbyte[][][] ToSByte(this string[][][] value)
    {
        return ToSByte(value, Jagged.CreateAs<string, sbyte>(value));
    }




    /// <summary>
    ///   Converts a string array to a signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[] ToSByte(this string[] value, sbyte[] result)
    {
        for (int i = 0; i < value.Length; i++)
            result[i] = SByte.Parse(value[i]);
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[,] ToSByte(this Matrix<string> value, Matrix<sbyte> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                result.Array[i, j] = SByte.Parse(value.Array[i, j]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[,,] ToSByte(this Tensor3<string> value, Tensor3<sbyte> result)
    {
        int r = value.Array.GetLength(0);
        int c = value.Array.GetLength(1);
        int d = value.Array.GetLength(2);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                for (int k = 0; k < d; k++)
                    result.Array[i, j, k] = SByte.Parse(value.Array[i, j, k]);

        return result.Array;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[][] ToSByte(this Matrix<string> value, Jagged<sbyte> result)
    {
        for (int i = 0; i < result.Array.Length; i++)
            for (int j = 0; j < result.Array[i].Length; j++)
                result.Array[i][j] = SByte.Parse(value.Array[i, j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[][] ToSByte(this Jagged<string> value, Jagged<sbyte> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i][j] = SByte.Parse(value.Array[i][j]);
        return result.Array;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[][][] ToSByte(this string[][][] value, sbyte[][][] result)
    {
        for (int i = 0; i < value.Length; i++)
            for (int j = 0; j < value[i].Length; j++)
                for (int k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = SByte.Parse(value[i][j][k]);
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional signed 7-bit byte array.
    /// </summary>
    /// 
    public static sbyte[,] ToSByte(this Jagged<string> value, Matrix<sbyte> result)
    {
        for (int i = 0; i < value.Array.Length; i++)
            for (int j = 0; j < value.Array[i].Length; j++)
                result.Array[i, j] = SByte.Parse(value.Array[i][j]);
        return result.Array;
    }


}