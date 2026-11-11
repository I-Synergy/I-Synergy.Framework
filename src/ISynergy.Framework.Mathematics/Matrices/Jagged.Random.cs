

using ISynergy.Framework.Mathematics.Abstractions;

namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Jagged
{
    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[][] Random(int rows, int columns)
    {
        return Random(rows, columns, 0.0, 1.0);
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[][] Random(int size)
    {
        return Random(size, size, 0.0, 1.0);
    }
    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static int[][] Random(int rows, int columns, int min, int max, Jagged<int> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<int>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = random.Next(min, max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static int[][] Random(int size, int min, int max, bool symmetric = false, Jagged<int> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<int>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = random.Next(min, max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = random.Next(min, max);
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static short[][] Random(int rows, int columns, short min, short max, Jagged<short> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<short>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (short)random.Next((int)min, (int)max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static short[][] Random(int size, short min, short max, bool symmetric = false, Jagged<short> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<short>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (short)random.Next((int)min, (int)max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (short)random.Next((int)min, (int)max);
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static byte[][] Random(int rows, int columns, byte min, byte max, Jagged<byte> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<byte>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (byte)random.Next((int)min, (int)max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static byte[][] Random(int size, byte min, byte max, bool symmetric = false, Jagged<byte> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<byte>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (byte)random.Next((int)min, (int)max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (byte)random.Next((int)min, (int)max);
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static sbyte[][] Random(int rows, int columns, sbyte min, sbyte max, Jagged<sbyte> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<sbyte>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (sbyte)random.Next((int)min, (int)max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static sbyte[][] Random(int size, sbyte min, sbyte max, bool symmetric = false, Jagged<sbyte> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<sbyte>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (sbyte)random.Next((int)min, (int)max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (sbyte)random.Next((int)min, (int)max);
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static long[][] Random(int rows, int columns, long min, long max, Jagged<long> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<long>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (long)random.Next((int)min, (int)max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static long[][] Random(int size, long min, long max, bool symmetric = false, Jagged<long> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<long>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (long)random.Next((int)min, (int)max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (long)random.Next((int)min, (int)max);
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static ulong[][] Random(int rows, int columns, ulong min, ulong max, Jagged<ulong> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<ulong>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (ulong)random.Next((int)min, (int)max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static ulong[][] Random(int size, ulong min, ulong max, bool symmetric = false, Jagged<ulong> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<ulong>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (ulong)random.Next((int)min, (int)max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (ulong)random.Next((int)min, (int)max);
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static ushort[][] Random(int rows, int columns, ushort min, ushort max, Jagged<ushort> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<ushort>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (ushort)random.Next((int)min, (int)max);
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static ushort[][] Random(int size, ushort min, ushort max, bool symmetric = false, Jagged<ushort> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<ushort>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (ushort)random.Next((int)min, (int)max);
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (ushort)random.Next((int)min, (int)max);
        }
        return resultArray;
    }
    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static float[][] Random(int rows, int columns, float min, float max, Jagged<float> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<float>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (float)random.NextDouble() * (max - min) + min;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static float[][] Random(int size, float min, float max, bool symmetric = false, Jagged<float> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<float>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (float)random.NextDouble() * (max - min) + min;
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (float)random.NextDouble() * (max - min) + min;
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[][] Random(int rows, int columns, double min, double max, Jagged<double> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<double>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = random.NextDouble() * (max - min) + min;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[][] Random(int size, double min, double max, bool symmetric = false, Jagged<double> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<double>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = random.NextDouble() * (max - min) + min;
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = random.NextDouble() * (max - min) + min;
        }
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static decimal[][] Random(int rows, int columns, decimal min, decimal max, Jagged<decimal> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<decimal>(rows, columns);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                resultArray[i][j] = (decimal)random.NextDouble() * (max - min) + min;
        return resultArray;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static decimal[][] Random(int size, decimal min, decimal max, bool symmetric = false, Jagged<decimal> result = default)
    {
        var resultArray = result.Array;
        if (resultArray is null)
            resultArray = Jagged.Create<decimal>(size, size);

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = resultArray[j][i] = (decimal)random.NextDouble() * (max - min) + min;
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    resultArray[i][j] = (decimal)random.NextDouble() * (max - min) + min;
        }
        return resultArray;
    }

}