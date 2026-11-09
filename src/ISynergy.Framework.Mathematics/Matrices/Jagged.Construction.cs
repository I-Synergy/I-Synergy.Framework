using ISynergy.Framework.Mathematics.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Random;
using ISynergy.Framework.Mathematics.Vectors;
using System.Collections;



namespace ISynergy.Framework.Mathematics.Matrices;

/// <summary>
///   Jagged matrices.
/// </summary>
/// 
/// <seealso cref="Matrix"/>
/// <seealso cref="Vector"/>
/// 
public static partial class Jagged
{
    /// <summary>
    ///   Creates a zero-valued matrix.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the matrix to be created.</typeparam>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static T[][] Zeros<T>(int rows, int columns)
    {
        T[][] matrix = new T[rows][];
        for (var i = 0; i < matrix.Length; i++)
            matrix[i] = new T[columns];
        return matrix;
    }

    /// <summary>
    ///   Creates a zero-valued rank-3 tensor.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the matrix to be created.</typeparam>
    /// <param name="rows">The number of rows in the tensor.</param>
    /// <param name="columns">The number of columns in the tensor.</param>
    /// <param name="depth">The number of channels in the tensor.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static T[][][] Zeros<T>(int rows, int columns, int depth)
    {
        T[][][] matrix = new T[rows][][];
        for (var i = 0; i < matrix.Length; i++)
        {
            matrix[i] = new T[columns][];
            for (var j = 0; j < matrix[i].Length; j++)
                matrix[i][j] = new T[depth];
        }

        return matrix;
    }

    /// <summary>
    ///   Creates a zero-valued matrix.
    /// </summary>
    /// 
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// 
    /// <returns>A vector of the specified size.</returns>
    /// 
    public static double[][] Zeros(int rows, int columns)
    {
        return Zeros<double>(rows, columns);
    }

    /// <summary>
    ///   Creates a zero-valued rank-3 tensor.
    /// </summary>
    /// 
    /// <param name="rows">The number of rows in the tensor.</param>
    /// <param name="columns">The number of columns in the tensor.</param>
    /// <param name="depth">The number of channels in the tensor.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static double[][][] Zeros(int rows, int columns, int depth)
    {
        return Zeros<double>(rows, columns, depth);
    }

    /// <summary>
    ///   Creates a jagged matrix with all values set to zero.
    /// </summary>
    /// 
    /// <param name="elementType">The type of the elements to be contained in the matrix.</param>
    /// <param name="shape">The number of dimensions that the matrix should have.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static Array Zeros(Type elementType, params int[] shape)
    {
        return Create(elementType, shape, elementType.GetDefaultValue());
    }

    /// <summary>
    ///   Creates a jagged matrix with all values set to zero.
    /// </summary>
    /// 
    /// <param name="shape">The number of dimensions that the matrix should have.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static Array Zeros<T>(params int[] shape)
    {
        return Create(typeof(T), shape, 0);
    }

    /// <summary>
    ///   Creates a zero-valued matrix.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the matrix to be created.</typeparam>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static T[][] Ones<T>(int rows, int columns)
    {
        return Create<T>(rows, columns, Constants.One<T>());
    }

    /// <summary>
    ///   Creates a zero-valued matrix.
    /// </summary>
    /// 
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// 
    /// <returns>A vector of the specified size.</returns>
    ///
    public static double[][] Ones(int rows, int columns)
    {
        return Ones<double>(rows, columns);
    }

    /// <summary>
    ///   Creates a jagged matrix with all values set to a given value.
    /// </summary>
    /// 
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <param name="value">The initial values for the vector.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static T[][] Create<T>(int rows, int columns, T value)
    {
        var matrix = new T[rows][];
        for (var i = 0; i < rows; i++)
        {
            var row = matrix[i] = new T[columns];
            for (var j = 0; j < row.Length; j++)
                row[j] = value;
        }

        return matrix;
    }

    /// <summary>
    ///   Creates a jagged matrix with all values set to a given value.
    /// </summary>
    /// 
    /// <param name="elementType">The type of the elements to be contained in the matrix.</param>
    /// <param name="shape">The number of dimensions that the matrix should have.</param>
    /// <param name="value">The initial values for the vector.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static Array Create(Type elementType, int[] shape, object value)
    {
        return create(elementType, shape, 0, value);
    }

    /// <summary>
    ///   Creates a jagged matrix with all values set to a given value.
    /// </summary>
    /// 
    /// <param name="shape">The number of dimensions that the matrix should have.</param>
    /// <param name="value">The initial values for the vector.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static Array Create<T>(int[] shape, T value)
    {
        return Create(typeof(T), shape, value);
    }

    /// <summary>
    ///   Creates a matrix with all values set to a given value.
    /// </summary>
    /// 
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <param name="values">The initial values for the matrix.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static T[][] Create<T>(int rows, int columns, params T[] values)
    {
        if (values.Length == 0)
            return Zeros<T>(rows, columns);
        return new Matrix<T>(values.Reshape(rows, columns)).ToJagged();
    }

    /// <summary>
    ///   Creates a matrix with the given rows.
    /// </summary>
    /// 
    /// <param name="rows">The row vectors in the matrix.</param>
    /// 
    public static T[][] Create<T>(Jagged<T> rows)
    {
        return rows.Array.Copy();
    }

    /// <summary>
    ///   Creates a matrix with all values set to a given value.
    /// </summary>
    /// 
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <param name="values">The initial values for the matrix.</param>
    /// <param name="transpose">Whether to transpose the matrix when copying or not. Default is false.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    public static T[][] Create<T>(int rows, int columns, Jagged<T> values, bool transpose = false)
    {
        var result = Zeros<T>(rows, columns);
        Matrix.CopyTo(values.Array, destination: new Jagged<T>(result), transpose: transpose);
        return result;
    }

    /// <summary>
    ///   Creates a matrix with the given values.
    /// </summary>
    /// 
    /// <param name="values">The values in the matrix.</param>
    /// 
    public static T[][] Create<T>(Matrix<T> values)
    {
        return values.ToJagged();
    }

    private static Array create(Type elementType, int[] shape, int dimension, object value)
    {
        Type arrayType = elementType.MakeArrayType(shape.Length - dimension - 1, jagged: true);
        Array array = Array.CreateInstance(arrayType, shape[dimension]);

        if (dimension < shape.Length - 1)
        {
            for (var i = 0; i < array.Length; i++)
                array.SetValue(create(elementType, shape, dimension + 1, value), i);
        }
        else
        {
            for (var i = 0; i < array.Length; i++)
                array.SetValue(value, i);
        }

        return array;
    }

    /// <summary>
    ///   Creates a jagged matrix with all values set to a given value.
    /// </summary>
    /// 
    /// <param name="size">The number of rows and columns in the matrix.</param>
    /// <param name="value">The initial values for the matrix.</param>
    /// 
    /// <returns>A matrix of the specified size.</returns>
    /// 
    /// <seealso cref="Matrix.Create{T}(int, int, T)"/>
    public static T[][] Square<T>(int size, T value)
    {
        return Create(size, size, value);
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the ones in the positions where <paramref name="mask"/>
    ///   are true, which are set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="mask">The boolean mask determining where ones will be placed.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    ///   is one and the others are zero.</returns>
    /// 
    public static T[][] OneHot<T>(bool[] mask)
    {
        return OneHot<T>(mask, new Jagged<T>(Jagged.Create<T>(mask.Length, 2)));
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    /// is one and the others are zero.</returns>
    /// 
    public static T[][] OneHot<T>(int[] indices)
    {
        return OneHot<T>(indices, indices.Max() + 1);
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
    /// </summary>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    /// is one and the others are zero.</returns>
    /// 
    public static double[][] OneHot(int[] indices) // NOSONAR
    {
        return OneHot(indices, indices.Max() + 1);
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    /// is one and the others are zero.</returns>
    /// 
    public static T[][] OneHot<T>(int[] indices, int columns)
    {
        return OneHot<T>(indices, new Jagged<T>(Jagged.Create<T>(indices.Length, columns)));
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
    /// </summary>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    /// is one and the others are zero.</returns>
    /// 
    public static double[][] OneHot(int[] indices, int columns)
    {
        return OneHot(indices, new Jagged<double>(Jagged.Create<double>(indices.Length, columns)));
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the ones in the positions where <paramref name="mask"/>
    ///   are true, which are set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="mask">The boolean mask determining where ones will be placed.</param>
    /// <param name="result">The matrix where the one-hot should be marked.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    ///   is one and the others are zero.</returns>
    /// 
    public static T[][] OneHot<T>(bool[] mask, Jagged<T> result)
    {
        var resultArray = result.Array;
        var one = Constants.One<T>();
        for (var i = 0; i < mask.Length; i++)
            if (mask[i])
                resultArray[i][0] = one;
            else
                resultArray[i][1] = one;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="result">The matrix where the one-hot should be marked.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    /// is one and the others are zero.</returns>
    /// 
    public static T[][] OneHot<T>(int[] indices, Jagged<T> result)
    {
        var resultArray = result.Array;
        var one = Constants.One<T>();
        for (var i = 0; i < indices.Length; i++)
            resultArray[i][indices[i]] = one;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
    /// </summary>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="result">The matrix where the one-hot should be marked.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    /// is one and the others are zero.</returns>
    /// 
    public static double[][] OneHot(int[] indices, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < indices.Length; i++)
            resultArray[i][indices[i]] = 1;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix of k-hot vectors, where all values at each row are 
    ///   zero except for the ones in the positions where <paramref name="mask"/>
    ///   are true, which are set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="mask">The boolean mask determining where ones will be placed.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    ///   is one and the others are zero.</returns>
    /// 
    public static T[][] KHot<T>(Jagged<bool> mask)
    {
        return KHot<T>(mask, new Jagged<T>(Jagged.CreateAs<bool, T>(mask)));
    }

    /// <summary>
    ///   Creates a matrix of k-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
    /// 
    /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
    ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
    /// 
    public static T[][] KHot<T>(Jagged<int> indices, int columns)
    {
        return KHot<T>(indices, new Jagged<T>(Jagged.Create<T>(indices.Rows, columns)));
    }

    /// <summary>
    ///   Creates a matrix of k-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
    /// </summary>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
    /// 
    /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
    ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
    /// 
    public static double[][] KHot(Jagged<int> indices, int columns)
    {
        return KHot(indices, new Jagged<double>(Jagged.Create<double>(indices.Rows, columns)));
    }

    /// <summary>
    ///   Creates a matrix of k-hot vectors, where all values at each row are 
    ///   zero except for the ones in the positions where <paramref name="mask"/>
    ///   are true, which are set to one.
    /// </summary>
    /// 
    /// <param name="mask">The boolean mask determining where ones will be placed.</param>
    /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    ///   is one and the others are zero.</returns>
    /// 
    public static double[][] KHot(Jagged<bool> mask, int columns)
    {
        return KHot(mask, new Jagged<double>(Jagged.Create<double>(mask.Rows, columns)));
    }

    /// <summary>
    ///   Creates a matrix of one-hot vectors, where all values at each row are 
    ///   zero except for the ones in the positions where <paramref name="mask"/>
    ///   are true, which are set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="mask">The boolean mask determining where ones will be placed.</param>
    /// <param name="result">The matrix where the one-hot should be marked.</param>
    /// 
    /// <returns>A matrix containing one-hot vectors where only a single position
    ///   is one and the others are zero.</returns>
    /// 
    public static T[][] KHot<T>(Jagged<bool> mask, Jagged<T> result)
    {
        var maskArray = mask.Array;
        var resultArray = result.Array;
        var one = Constants.One<T>();
        for (var i = 0; i < maskArray.Length; i++)
            for (var j = 0; j < maskArray[0].Length; j++)
                if (maskArray[i][j])
                    resultArray[i][j] = one;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix of k-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type for the matrix.</typeparam>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="result">The matrix where the one-hot should be marked.</param>
    /// 
    /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
    ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
    /// 
    public static T[][] KHot<T>(Jagged<int> indices, Jagged<T> result)
    {
        var indicesArray = indices.Array;
        var resultArray = result.Array;
        var one = Constants.One<T>();
        for (var i = 0; i < indicesArray.Length; i++)
            for (var j = 0; j < indicesArray[0].Length; j++)
                resultArray[i][indicesArray[i][j]] = one;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix of k-hot vectors, where all values at each row are 
    ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
    /// </summary>
    /// 
    /// <param name="indices">The rows's dimension which will be marked as one.</param>
    /// <param name="result">The matrix where the one-hot should be marked.</param>
    /// 
    /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
    ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
    /// 
    public static double[][] KHot(Jagged<int> indices, Jagged<double> result)
    {
        var indicesArray = indices.Array;
        var resultArray = result.Array;
        for (var i = 0; i < indicesArray.Length; i++)
            for (var j = 0; j < indicesArray[i].Length; j++)
                resultArray[i][indicesArray[i][j]] = 1;
        return resultArray;
    }

    /// <summary>
    ///   Creates a new multidimensional matrix with the same shape as another matrix.
    /// </summary>
    /// 
    public static Array CreateAs(Array matrix, Type type)
    {
        int[] outputShape = Matrix.GetShape(matrix, type);

        // multidimensional or jagged -> jagged
        return Jagged.Create(elementType: type.GetInnerMostType(), shape: outputShape, value: type.GetDefaultValue());
    }

    /// <summary>
    ///   Creates a new multidimensional matrix with the same shape as another matrix.
    /// </summary>
    /// 
    public static T[][] CreateAs<T>(Matrix<T> matrix)
    {
        int rows = matrix.Array.GetLength(0);
        int cols = matrix.Array.GetLength(1);
        T[][] r = new T[rows][];
        for (var i = 0; i < r.Length; i++)
            r[i] = new T[cols];
        return r;
    }

    /// <summary>
    ///   Returns a new multidimensional matrix.
    /// </summary>
    /// 
    public static T[][] CreateAs<T>(Jagged<T> matrix)
    {
        var matrixArray = matrix.Array;
        T[][] r = new T[matrixArray.Length][];
        for (var i = 0; i < r.Length; i++)
            r[i] = new T[matrixArray[i].Length];
        return r;
    }

    /// <summary>
    ///   Returns a new multidimensional matrix.
    /// </summary>
    /// 
    public static TOutput[][] CreateAs<TInput, TOutput>(Matrix<TInput> matrix)
    {
        int rows = matrix.Array.GetLength(0);
        int cols = matrix.Array.GetLength(1);
        var r = new TOutput[rows][];
        for (var i = 0; i < r.Length; i++)
            r[i] = new TOutput[cols];
        return r;
    }

    /// <summary>
    ///   Returns a new multidimensional matrix.
    /// </summary>
    /// 
    public static TOutput[][] CreateAs<TInput, TOutput>(Jagged<TInput> matrix)
    {
        var matrixArray = matrix.Array;
        var r = new TOutput[matrixArray.Length][];
        for (var i = 0; i < r.Length; i++)
            r[i] = new TOutput[matrixArray[i].Length];
        return r;
    }

    /// <summary>
    ///   Returns a new multidimensional matrix.
    /// </summary>
    /// 
    public static TOutput[][][] CreateAs<TInput, TOutput>(Jagged<TInput[]> matrix)
    {
        var matrixArray = matrix.Array;
        var r = new TOutput[matrixArray.Length][][];
        for (var i = 0; i < r.Length; i++)
        {
            r[i] = new TOutput[matrixArray[i].Length][];
            for (var j = 0; j < r[i].Length; j++)
                r[i][j] = new TOutput[matrixArray[i][j].Length];
        }
        return r;
    }

    /// <summary>
    ///   Creates a 1xN matrix with a single row vector of size N.
    /// </summary>
    /// 
    public static T[][] RowVector<T>(params T[] values)
    {
        return new T[][] { values };
    }

    /// <summary>
    ///   Creates a Nx1 matrix with a single column vector of size N.
    /// </summary>
    /// 
    public static T[][] ColumnVector<T>(params T[] values)
    {
        T[][] column = new T[values.Length][];
        for (var i = 0; i < column.Length; i++)
            column[i] = [values[i]];

        return column;
    }

    /// <summary>
    ///   Creates a square matrix with ones across its diagonal.
    /// </summary>
    /// 
    public static double[][] Identity(int size)
    {
        return Diagonal(size, 1.0);
    }

    /// <summary>
    ///   Creates a square matrix with ones across its diagonal.
    /// </summary>
    /// 
    public static T[][] Identity<T>(int size)
    {
        return Diagonal(size, Constants.One<T>());
    }

    /// <summary>
    ///   Creates a jagged magic square matrix.
    /// </summary>
    /// 
    public static double[][] Magic(int size)
    {
        return new Matrix<double>(Matrix.Magic(size)).ToJagged();
    }

    #region Diagonal matrices

    /// <summary>
    ///   Returns a square diagonal matrix of the given size.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int size, T value)
    {
        return Diagonal(size, value, new Jagged<T>(Jagged.Create<T>(size, size)));
    }

    /// <summary>
    ///   Returns a square diagonal matrix of the given size.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int size, T value, Jagged<T> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < size; i++)
            resultArray[i][i] = value;
        return resultArray;
    }

    /// <summary>
    ///   Returns a matrix of the given size with value on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int rows, int cols, T value)
    {
        return Diagonal(rows, cols, value, new Jagged<T>(Jagged.Create<T>(rows, cols)));
    }

    /// <summary>
    ///   Returns a matrix of the given size with value on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int rows, int cols, T value, Jagged<T> result)
    {
        var resultArray = result.Array;
        int min = Math.Min(rows, cols);
        for (var i = 0; i < min; i++)
            resultArray[i][i] = value;
        return resultArray;
    }

    /// <summary>
    ///   Return a square matrix with a vector of values on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(T[] values)
    {
        return Diagonal(values, new Jagged<T>(Jagged.Create<T>(values.Length, values.Length)));
    }

    /// <summary>
    ///   Return a square matrix with a vector of values on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(T[] values, Jagged<T> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < values.Length; i++)
            resultArray[i][i] = values[i];
        return resultArray;
    }

    /// <summary>
    ///   Return a square matrix with a vector of values on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int size, T[] values)
    {
        return Diagonal(size, size, values);
    }

    /// <summary>
    ///   Return a square matrix with a vector of values on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int size, T[] values, Jagged<T> result)
    {
        return Diagonal(size, size, values, result);
    }

    /// <summary>
    ///   Returns a matrix with a vector of values on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int rows, int cols, T[] values)
    {
        return Diagonal(rows, cols, values, new Jagged<T>(Jagged.Create<T>(rows, cols)));
    }

    /// <summary>
    ///   Returns a matrix with a vector of values on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(int rows, int cols, T[] values, Jagged<T> result)
    {
        var resultArray = result.Array;
        int size = Math.Min(rows, Math.Min(cols, values.Length));
        for (var i = 0; i < size; i++)
            resultArray[i][i] = values[i];
        return resultArray;
    }

    /// <summary>
    ///   Returns a block-diagonal matrix with the given matrices on its diagonal.
    /// </summary>
    /// 
    public static T[][] Diagonal<T>(Jagged<T[]> blocks)
    {
        var blocksArray = blocks.Array;
        int rows = 0;
        int cols = 0;
        for (var i = 0; i < blocksArray.Length; i++)
        {
            rows += blocksArray[i].Rows();
            cols += blocksArray[i].Columns();
        }

        var result = Jagged.Create<T>(rows, cols);
        int currentRow = 0;
        int currentCol = 0;
        for (var i = 0; i < blocksArray.Length; i++)
        {
            for (var r = 0; r < blocksArray[i].Length; r++)
            {
                for (var c = 0; c < blocksArray[i][r].Length; c++)
                    result[currentRow + r][currentCol + c] = blocksArray[i][r][c];
            }

            currentRow = blocksArray[i].Length;
            currentCol = blocksArray[i][0].Length;
        }

        return result;
    }

    #endregion

    /// <summary>
    ///   Transforms a vector into a matrix of given dimensions.
    /// </summary>
    /// 
    public static T[][] Reshape<T>(T[] array, int rows, int cols, MatrixOrder order = MatrixOrder.Default)
    {
        return Jagged.Reshape(array, rows, cols, Jagged.Create<T>(rows, cols), order);
    }

    /// <summary>
    ///   Transforms a vector into a matrix of given dimensions.
    /// </summary>
    /// 
    public static T[][] Reshape<T>(this T[] array, int rows, int cols, T[][] result, MatrixOrder order = MatrixOrder.Default)
    {
        if (order == MatrixOrder.CRowMajor)
        {
            int k = 0;
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    result[i][j] = array[k++];
        }
        else
        {
            int k = 0;
            for (var j = 0; j < cols; j++)
                for (var i = 0; i < rows; i++)
                    result[i][j] = array[k++];
        }

        return result;
    }

    #region Random matrices

    /// <summary>
    ///   Creates a square matrix matrix with random data.
    /// </summary>
    /// 
    public static T[][] Random<T>(int size, IRandomNumberGenerator<T> generator,
        bool symmetric = false, Jagged<T> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<T>(size, size);

        if (!symmetric)
        {
            for (var i = 0; i < size; i++)
                resultArray[i] = generator.Generate(size);
        }
        else
        {
            for (var i = 0; i < size; i++)
            {
                T[] row = generator.Generate(size / 2, resultArray[i]);
                for (int start = 0, end = size - 1; start < size / 2; start++, end--)
                    row[end] = row[start];
            }
        }

        return resultArray;
    }

    /// <summary>
    ///   Creates a rows-by-cols matrix with random data.
    /// </summary>
    /// 
    public static T[][] Random<T>(int rows, int cols,
        IRandomNumberGenerator<T> generator, Jagged<T> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<T>(rows, cols);

        for (var i = 0; i < rows; i++)
            resultArray[i] = generator.Generate(cols);
        return resultArray;
    }

    #endregion

    /// <summary>
    ///   Enumerates through all elements in a matrix.
    /// </summary>
    /// 
    /// <param name="array">The array to be iterated.</param>
    /// <param name="shape">The full shape of <paramref name="array"/> .</param>
    /// 
    public static IEnumerable Enumerate(this Array array, int[] shape)
    {
        if (array.IsMatrix())
        {
            if (array.Rank != shape.Length)
                throw new NotSupportedException();

            foreach (var e in array)
                yield return e;
            yield break;
        }

        var arrays = new Stack<Array>();
        var counters = new Stack<int>();

        arrays.Push(array);
        counters.Push(0);
        int depth = 1;

        Array a = array;
        int i = 0;

        while (arrays.Count > 0)
        {
            if (i >= shape[depth - 1])
            {
                a = arrays.Pop();
                i = counters.Pop() + 1;
                depth--;
            }
            else
            {
                if (a is null || i >= a.Length)
                {
                    if (depth == shape.Length)
                    {
                        yield return null;
                        i++;
                    }
                    else
                    {
                        arrays.Push(a);
                        counters.Push(i);
                        a = null;
                        i = 0;
                        depth++;
                    }
                }
                else
                {
                    if (depth == shape.Length)
                    {
                        yield return a.GetValue(i);
                        i++;
                    }
                    else
                    {
                        arrays.Push(a);
                        counters.Push(i);
                        a = (Array)a.GetValue(i);
                        i = 0;
                        depth++;
                    }
                }
            }
        }
    }

    /// <summary>
    ///   Enumerates through all elements in a matrix.
    /// </summary>
    /// 
    /// <param name="array">The array to be iterated.</param>
    /// <param name="shape">The full shape of <paramref name="array"/> .</param>
    /// 
    public static IEnumerable<T> Enumerate<T>(this Array array, int[] shape)
    {
        if (array.IsMatrix())
        {
            if (array.Rank != shape.Length)
                throw new NotSupportedException();

            foreach (var e in array)
                yield return (T)e;
            yield break;
        }

        var arrays = new Stack<Array>();
        var counters = new Stack<int>();

        arrays.Push(array);
        counters.Push(0);
        int depth = 1;

        Array a = array;
        int i = 0;

        while (arrays.Count > 0)
        {
            if (i >= shape[depth - 1])
            {
                a = arrays.Pop();
                i = counters.Pop() + 1;
                depth--;
            }
            else
            {
                if (a is null || i >= a.Length)
                {
                    if (depth == shape.Length)
                    {
                        int n = shape[shape.Length - 1];
                        for (; i < n; i++)
                            yield return default(T);
                    }
                    else
                    {
                        arrays.Push(a);
                        counters.Push(i);
                        a = null;
                        i = 0;
                        depth++;
                    }
                }
                else
                {
                    if (depth == shape.Length)
                    {
                        T[] t = (T[])a;
                        for (; i < t.Length; i++)
                            yield return t[i];
                    }
                    else
                    {
                        arrays.Push(a);
                        counters.Push(i);
                        a = (Array)a.GetValue(i);
                        i = 0;
                        depth++;
                    }
                }
            }
        }
    }

    /// <summary>
    ///   Enumerates through all elements in a matrix.
    /// </summary>
    /// 
    /// <param name="array">The array to be iterated.</param>
    /// 
    public static IEnumerable<T> Enumerate<T>(this Array array)
    {
        if (array.IsMatrix())
        {
            foreach (var e in array)
                yield return (T)e;
            yield break;
        }

        var arrays = new Stack<Array>();
        var counters = new Stack<int>();

        arrays.Push(array);
        counters.Push(0);
        int depth = 1;

        Array a = array;
        int i = 0;

        while (arrays.Count > 0)
        {
            if (i >= a.Length)
            {
                a = arrays.Pop();
                i = counters.Pop() + 1;
                depth--;
            }
            else
            {
                Object e = a.GetValue(i);
                T[] next = e as T[];
                if (next is not null)
                {
                    foreach (T t in next)
                        yield return t;
                    i++;
                }
                else
                {
                    arrays.Push(a);
                    counters.Push(i);
                    a = (Array)e;
                    i = 0;
                    depth++;
                }
            }
        }
    }

    /// <summary>
    ///   Enumerates through all elements in a matrix.
    /// </summary>
    /// 
    /// <param name="array">The array to be iterated.</param>
    /// 
    public static IEnumerable Enumerate(this Array array)
    {
        if (array.IsMatrix())
        {
            foreach (var e in array)
                yield return e;
            yield break;
        }

        var arrays = new Stack<Array>();
        var counters = new Stack<int>();

        arrays.Push(array);
        counters.Push(0);
        int depth = 1;

        Array a = array;
        int i = 0;

        while (arrays.Count > 0)
        {
            if (i >= a.Length)
            {
                a = arrays.Pop();
                i = counters.Pop() + 1;
                depth--;
            }
            else
            {
                Object e = a.GetValue(i);
                Array next = e as Array;
                if (next is null)
                {
                    yield return e;
                    i++;
                }
                else
                {
                    arrays.Push(a);
                    counters.Push(i);
                    a = next;
                    i = 0;
                    depth++;
                }
            }
        }
    }

    /// <summary>
    ///   Gets the transpose of a matrix.
    /// </summary>
    /// 
    /// <param name="matrix">A matrix.</param>
    /// 
    /// <returns>The transpose of the given matrix.</returns>
    /// 
    public static T[][] Transpose<T>(Matrix<T> matrix)
    {
        var matrixArray = matrix.Array;
        int rows = matrixArray.Rows();
        if (rows == 0)
            return new T[rows][];
        int cols = matrixArray.Columns();

        T[][] result = new T[cols][];
        for (var j = 0; j < result.Length; j++)
        {
            result[j] = new T[rows];
            for (var i = 0; i < result[j].Length; i++)
                result[j][i] = matrixArray[i, j];
        }

        return result;
    }

}