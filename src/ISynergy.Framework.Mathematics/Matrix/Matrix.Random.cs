namespace ISynergy.Framework.Mathematics;

public static partial class Matrix
{
    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[,] Random(int rows, int columns)
    {
        return Random(rows, columns, 0.0, 1.0);
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[,] Random(int size)
    {
        return Random(size, size, 0.0, 1.0);
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[,] Random(int rows, int columns, double min, double max, double[,] result = null)
    {
        if (result is null)
            result = new double[rows, columns];

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                result[i, j] = (double)random.NextDouble() * (max - min) + min;
        return result;
    }

    /// <summary>
    ///   Creates a matrix with uniformly distributed random data.
    /// </summary>
    /// 
    public static double[,] Random(int size, double min, double max, bool symmetric = false, double[,] result = null)
    {
        if (result is null)
            result = new double[size, size];

        var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

        if (symmetric)
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    result[i, j] = result[j, i] = (double)random.NextDouble() * (max - min) + min;
        }
        else
        {
            for (var i = 0; i < size; i++)
                for (var j = i; j < size; j++)
                    result[i, j] = (double)random.NextDouble() * (max - min) + min;
        }
        return result;
    }
}