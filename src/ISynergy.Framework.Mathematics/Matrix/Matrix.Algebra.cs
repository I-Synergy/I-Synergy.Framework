using ISynergy.Framework.Mathematics.Formats;

namespace ISynergy.Framework.Mathematics;

/// <summary>
///     Static class Matrix. Defines a set of extension methods
///     that operates mainly on multidimensional arrays and vectors.
/// </summary>
/// <remarks>
///     The matrix class is a static class containing several extension methods.
///     To use this class, import the <see cref="ISynergy.Framework.Mathematics" /> and use the
///     standard .NET's matrices and jagged arrays. When you call the dot (.)
///     operator on those classes, the extension methods offered by this class
///     should become available through IntelliSense auto-complete.
/// </remarks>
/// <example>
///     <h2>Introduction</h2>
///     <para>
///         Declaring and using matrices in the ISynergy.Framework.Mathematics.NET Framework does
///         not requires much. In fact, it does not require anything else
///         that is not already present at the .NET Framework. If you have
///         already existing and working code using other libraries, you
///         don't have to convert your matrices to any special format used
///         by ISynergy.Framework.Mathematics.NET. This is because ISynergy.Framework.Mathematics.NET is built to interoperate
///         with other libraries and existing solutions, relying solely on
///         default .NET structures to work.
///     </para>
///     <para>
///         To begin, please add the following <c>using</c> directive on
///         top of your .cs (or equivalent) source code file:
///     </para>
///     <code>
///     using ISynergy.Framework.Mathematics;
///   </code>
///     <para>
///         This is all you need to start using the ISynergy.Framework.Mathematics.NET matrix library.
///     </para>
///     <h2>Creating matrices</h2>
///     <para>
///         Let's start by declaring a matrix, or otherwise specifying matrices
///         from other sources. The most straightforward way to declare a matrix
///         in ISynergy.Framework.Mathematics.NET is simply using:
///     </para>
///     <code>
///     double[,] matrix = 
///     {
///        { 1, 2 },
///        { 3, 4 },
///        { 5, 6 },
///    };
/// </code>
///     <para>
///         Yep, that is right. You don't need to create any fancy custom Matrix
///         classes or vectors to make ISynergy.Framework.Mathematics.NET work, which is a plus if you
///         have already existent code using other libraries. You are also free
///         to use both the multidimensional matrix syntax above or the jagged
///         matrix syntax below:
///     </para>
///     <code>
///     double[][] matrix = 
///     {
///        new double[] { 1, 2 },
///        new double[] { 3, 4 },
///        new double[] { 5, 6 },
///    };
/// </code>
///     <para>
///         Special purpose matrices can also be created through specialized methods.
///         Those include
///     </para>
///     <code>
///   // Creates a vector of indices
///   int[] idx = Matrix.Indices(0, 10);  // { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
///   
///   // Creates a step vector within a given interval
///   double[] interval = Matrix.Interval(from: -2, to: 4); // { -2, -1, 0, 1, 2, 3, 4 };
///   
///   // Special matrices
///   double[,] I = Matrix.Identity(3);     // creates a 3x3 identity matrix
///   double[,] magic = Matrix.Magic(5);    // creates a magic square matrix of size 5
///   
///   double[] v = Matrix.Vector(5, 1.0);      // generates { 1, 1, 1, 1, 1 }
///   double[,] diagonal = Matrix.Diagonal(v); // matrix with v on its diagonal
/// </code>
///     <para>
///         Another way to declare matrices is by parsing the contents of a string:
///     </para>
///     <code>
///   string str = @"1 2
///                  3 4";
///                  
///   double[,] matrix = Matrix.Parse(str);
/// </code>
///     <para>
///         You can even read directly from matrices formatted in C# syntax:
///     </para>
///     <code>
///   string str = @"double[,] matrix = 
///                  {
///                     { 1, 2 },
///                     { 3, 4 },
///                     { 5, 6 },
///                  }";
///                  
///   double[,] multid = Matrix.Parse(str, CSharpMatrixFormatProvider.InvariantCulture);
///   double[,] jagged = Matrix.ParseJagged(str, CSharpMatrixFormatProvider.InvariantCulture);
///  </code>
///     <para>
///         And even from <a href="http://www.gnu.org/software/octave/">Octave-compatible</a> syntax!
///     </para>
///     <code>
///   string str = "[1 2; 3 4]";
///                  
///   double[,] matrix = Matrix.Parse(str, OctaveMatrixFormatProvider.InvariantCulture);
///  </code>
///     <para>
///         There are also other methods, such as specialization for arrays and other formats.
///         For more details, please take a look on <see cref="CSharpMatrixFormatProvider" />,
///         <see cref="CSharpArrayFormatProvider" />, <see cref="DefaultArrayFormatProvider" />,
///         <see cref="DefaultMatrixFormatProvider" /> and <see cref="Matrix.Parse(string)" />.
///     </para>
///     <h2>Matrix operations</h2>
///     <para>
///         Albeit being simple <see cref="T:double[]" /> matrices, the framework leverages
///         .NET extension methods to support all basic matrix operations. For instance,
///         consider the elementwise operations (also known as dot operations in Octave):
///     </para>
///     <code>
///   double[] vector = { 0, 2, 4 };
///   double[] a = vector.ElementwiseMultiply(2); // vector .* 2, generates { 0,  4,  8 }
///   double[] b = vector.ElementwiseDivide(2);   // vector ./ 2, generates { 0,  1,  2 }
///   double[] c = vector.ElementwisePower(2);    // vector .^ 2, generates { 0,  4, 16 }
/// </code>
///     <para>
///         Operations between vectors, matrices, and both are also completely supported:
///     </para>
///     <code>
///   // Declare two vectors
///   double[] u = { 1, 6, 3 };
///   double[] v = { 9, 4, 2 };
/// 
///   // Products between vectors
///   double inner = u.InnerProduct(v);    // 39.0
///   double[,] outer = u.OuterProduct(v); // see below
///   double[] kronecker = u.KroneckerProduct(v); // { 9, 4, 2, 54, 24, 12, 27, 12, 6 }
///   double[][] cartesian = u.CartesianProduct(v); // all possible pair-wise combinations
/// 
/// /* outer =
///    { 
///       {  9,  4,  2 },
///       { 54, 24, 12 },
///       { 27, 12,  6 },
///    };                  */
/// 
///   // Addition
///   double[] addv = u.Add(v); // { 10, 10, 5 }
///   double[] add5 = u.Add(5); // {  6, 11, 8 }
/// 
///   // Elementwise operations
///   double[] abs = u.Abs();   // { 1, 6, 3 }
///   double[] log = u.Log();   // { 0, 1.79, 1.09 }
///   
///   // Apply *any* function to all elements in a vector
///   double[] cos = u.Apply(Math.Cos); // { 0.54, 0.96, -0.989 }
///   u.ApplyInPlace(Math.Cos); // can also do optionally in-place
/// 
///   
///   // Declare a matrix
///   double[,] M = 
///   {
///      { 0, 5, 2 },
///      { 2, 1, 5 }
///   };
///  
///   // Extract a subvector from v:
///   double[] vcut = v.Submatrix(0, 1); // { 9, 4 }
///   
///   // Some operations between vectors and matrices
///   double[] Mv = m.Multiply(v);    //  { 24, 32 }
///   double[] vM = vcut.Multiply(m); // { 8, 49, 38 }
///   
///   // Some operations between matrices
///   double[,] Md = m.MultiplyByDiagonal(v);   // { { 0, 20, 4 }, { 18, 4, 10 } }
///   double[,] MMt = m.MultiplyByTranspose(m); //   { { 29, 15 }, { 15, 30 } }
/// </code>
///     <para>
///         Please note this is by no means an extensive list; please take a look on
///         all members available on this class or (preferably) use IntelliSense to
///         navigate through all possible options when trying to perform an operation.
///     </para>
/// </example>
/// <seealso cref="Formats.DefaultMatrixFormatProvider" />
/// <seealso cref="Formats.DefaultArrayFormatProvider" />
/// <seealso cref="Mathematics.Formats.OctaveMatrixFormatProvider" />
/// <seealso cref="Formats.OctaveArrayFormatProvider" />
/// <seealso cref="Formats.CSharpMatrixFormatProvider" />
/// <seealso cref="Formats.CSharpArrayFormatProvider" />
public static partial class Matrix
{
    /// <summary>
    ///     Normalizes a vector to have unit length.
    /// </summary>
    /// <param name="vector">A vector.</param>
    /// <param name="norm">A norm to use. Default is <see cref="Norm.Euclidean(double[])" />.</param>
    /// <param name="inPlace">
    ///     True to perform the operation in-place,
    ///     overwriting the original array; false to return a new array.
    /// </param>
    /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
    public static double[] Normalize(this double[] vector, Func<double[], double> norm, bool inPlace = false)
    {
        var r = inPlace ? vector : new double[vector.Length];

        var w = norm(vector);

        if (w == 0)
            for (var i = 0; i < vector.Length; i++)
                r[i] = vector[i];
        else
            for (var i = 0; i < vector.Length; i++)
                r[i] = vector[i] / w;

        return r;
    }

    /// <summary>
    ///     Normalizes a vector to have unit length.
    /// </summary>
    /// <param name="vector">A vector.</param>
    /// <param name="norm">A norm to use. Default is <see cref="Norm.Euclidean(float[])" />.</param>
    /// <param name="inPlace">
    ///     True to perform the operation in-place,
    ///     overwriting the original array; false to return a new array.
    /// </param>
    /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
    public static float[] Normalize(this float[] vector, Func<float[], float> norm, bool inPlace = false)
    {
        var r = inPlace ? vector : new float[vector.Length];

        double w = norm(vector);

        if (w == 0)
            for (var i = 0; i < vector.Length; i++)
                r[i] = vector[i];
        else
            for (var i = 0; i < vector.Length; i++)
                r[i] = (float)(vector[i] / w);

        return r;
    }

    /// <summary>
    ///     Normalizes a vector to have unit length.
    /// </summary>
    /// <param name="vector">A vector.</param>
    /// <param name="inPlace">
    ///     True to perform the operation in-place,
    ///     overwriting the original array; false to return a new array.
    /// </param>
    /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
    public static double[] Normalize(this double[] vector, bool inPlace = false)
    {
        return Normalize(vector, Norm.Euclidean, inPlace);
    }

    /// <summary>
    ///     Normalizes a vector to have unit length.
    /// </summary>
    /// <param name="vector">A vector.</param>
    /// <param name="inPlace">
    ///     True to perform the operation in-place,
    ///     overwriting the original array; false to return a new array.
    /// </param>
    /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
    public static float[] Normalize(this float[] vector, bool inPlace = false)
    {
        return Normalize(vector, Norm.Euclidean, inPlace);
    }

    /// <summary>
    ///     Multiplies a matrix by itself <c>n</c> times.
    /// </summary>
    public static double[,] Power(this double[,] matrix, int n)
    {
        if (matrix is null)
            throw new ArgumentNullException("matrix");

        if (!matrix.IsSquare())
            throw new ArgumentException("Matrix must be square", "matrix");

        if (n == 0)
            return Matrix.Identity(matrix.GetLength(0));

        // TODO: Reduce the number of memory allocations
        // TODO: Use bitwise operations instead of strings

        var result = matrix;
        var bin = System.Convert.ToString(n, 2);
        for (var i = 1; i < bin.Length; i++)
        {
            result = Matrix.Dot(result, result);

            if (bin[i] == '1')
                result = Matrix.Dot(result, matrix);
        }

        return result;
    }

    /// <summary>
    ///     Computes the Cartesian product of many sets.
    /// </summary>
    /// <remarks>
    ///     References:
    ///     - http://blogs.msdn.com/b/ericlippert/archive/2010/06/28/computing-a-Cartesian-product-with-linq.aspx
    /// </remarks>
    public static IEnumerable<IEnumerable<T>> Cartesian<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> empty = new[] { Enumerable.Empty<T>() };

        return sequences.Aggregate(empty, (accumulator, sequence) =>
            from accumulatorSequence in accumulator
            from item in sequence
            select accumulatorSequence.Concat(new[] { item }));
    }

    /// <summary>
    ///     Computes the Cartesian product of many sets.
    /// </summary>
    public static T[][] Cartesian<T>(params T[][] sequences)
    {
        var result = Cartesian(sequences as IEnumerable<IEnumerable<T>>);

        var list = new List<T[]>();
        foreach (var point in result)
            list.Add(point.ToArray());

        return list.ToArray();
    }

    /// <summary>
    ///     Computes the Cartesian product of two sets.
    /// </summary>
    public static T[][] Cartesian<T>(this T[] sequence1, T[] sequence2)
    {
        return Cartesian(new[] { sequence1, sequence2 });
    }
}