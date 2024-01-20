using ISynergy.Framework.Mathematics.Matrices;

namespace ISynergy.Framework.Mathematics.Environments;

/// <summary>
///   GNU R algorithm environment. Work in progress.
/// </summary>
/// 
public abstract class REnvironment
{
    /// <summary>
    ///   Creates a new vector.
    /// </summary>
    /// 
    protected vec c(params double[] values)
    {
        return values;
    }

    /// <summary>
    ///   Creates a new matrix.
    /// </summary>
    /// 
    protected mat matrix(double[] values, int rows, int cols)
    {
        return Matrix.Reshape(values, rows, cols, MatrixOrder.FortranColumnMajor);
    }
    /// <summary>
    ///   Placeholder vector definition
    /// </summary>
    /// 
    protected vec _
    {
        get { return new vec(null); }
    }
    /// <summary>
    ///   Vector definition operator.
    /// </summary>
    /// 
    protected class vec
    {
        /// <summary>
        ///   Inner vector object
        /// </summary>
        /// 
        public double[] vector;

        /// <summary>
        ///   Initializes a new instance of the <see cref="vec"/> class.
        /// </summary>
        /// 
        public vec(double[] values)
        {
            this.vector = values;
        }

        /// <summary>
        ///   Implements the operator -.
        /// </summary>
        /// 
        public static vec operator -(vec v)
        {
            return v;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// 
        public static vec operator <(vec a, vec v)
        {
            a.vector = v.vector;
            return a;
        }

        /// <summary>
        ///   Implements the operator &gt;.
        /// </summary>
        /// 
        public static vec operator >(vec a, vec v)
        {
            return a;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="T:System.Double[]"/>
        ///   to <see cref="REnvironment.vec"/>.
        /// </summary>
        /// 
        public static implicit operator vec(double[] v)
        {
            return new vec(v);
        }
        /// <summary>
        ///   Performs an implicit conversion from 
        ///   <see cref="REnvironment.vec"/> 
        ///   to <see cref="T:System.Double[]"/>.
        /// </summary>
        /// 
        public static implicit operator double[](vec v)
        {
            return v.vector;
        }
    }

    /// <summary>
    ///   Matrix definition operator.
    /// </summary>
    /// 
    protected class mat
    {
        /// <summary>
        ///   Inner matrix object.
        /// </summary>
        /// 
        public double[,] matrix;

        /// <summary>
        ///   Initializes a new instance of the <see cref="mat"/> class.
        /// </summary>
        /// 
        public mat(double[,] values)
        {
            this.matrix = values;
        }

        /// <summary>
        ///   Implements the operator -.
        /// </summary>
        /// 
        public static mat operator -(mat v)
        {
            return v;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// 
        public static mat operator <(mat a, mat v)
        {
            a.matrix = v.matrix;
            return a;
        }

        /// <summary>
        ///    Implements the operator &gt;.
        /// </summary>
        /// 
        public static mat operator >(mat a, mat v)
        {
            return a;
        }

        /// <summary>
        ///   Performs an implicit conversion from 
        ///   <see cref="T:System.Double[]"/> to 
        ///   <see cref="REnvironment.mat"/>.
        /// </summary>
        /// 
        public static implicit operator mat(double[,] v)
        {
            return new mat(v);
        }
        /// <summary>
        ///   Performs an implicit conversion from 
        ///   <see cref="REnvironment.mat"/> 
        ///   to <see cref="T:System.Double[]"/>.
        /// </summary>
        /// 
        public static implicit operator double[,](mat v)
        {
            return v.matrix;
        }
    }
}
