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
    protected static Vec c(params double[] values) // NOSONAR
    {
        return values;
    }

    /// <summary>
    ///   Creates a new matrix.
    /// </summary>
    /// 
    protected static Mat matrix(double[] values, int rows, int cols) // NOSONAR
    {
        return Matrix.Reshape(values, rows, cols, MatrixOrder.FortranColumnMajor);
    }
    /// <summary>
    ///   Placeholder vector definition
    /// </summary>
    /// 
    protected static Vec _
    {
        get { return new Vec(null); }
    }
    /// <summary>
    ///   Vector definition operator.
    /// </summary>
    /// 
    protected class Vec
    {
        /// <summary>
        ///   Inner vector object
        /// </summary>
        ///
        public double[] vector { get; set; } // NOSONAR

        /// <summary>
        ///   Initializes a new instance of the <see cref="Vec"/> class.
        /// </summary>
        ///
        public Vec(double[] values)
        {
            this.vector = values;
        }

        /// <summary>
        ///   Implements the operator -.
        /// </summary>
        /// 
        public static Vec operator -(Vec v)
        {
            return v;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// 
        public static Vec operator <(Vec a, Vec v)
        {
            a.vector = v.vector;
            return a;
        }

        /// <summary>
        ///   Implements the operator &gt;.
        /// </summary>
        /// 
        public static Vec operator >(Vec a, Vec v)
        {
            return a;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="T:System.Double[]"/>
        ///   to <see cref="REnvironment.Vec"/>.
        /// </summary>
        /// 
        public static implicit operator Vec(double[] v)
        {
            return new Vec(v);
        }
        /// <summary>
        ///   Performs an implicit conversion from 
        ///   <see cref="REnvironment.Vec"/> 
        ///   to <see cref="T:System.Double[]"/>.
        /// </summary>
        /// 
        public static implicit operator double[](Vec v)
        {
            return v.vector;
        }
    }

    /// <summary>
    ///   Matrix definition operator.
    /// </summary>
    /// 
    protected class Mat
    {
        /// <summary>
        ///   Inner matrix object.
        /// </summary>
        ///
        public double[,] data { get; set; } // NOSONAR

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mat"/> class.
        /// </summary>
        ///
        public Mat(double[,] values)
        {
            this.data = values;
        }

        /// <summary>
        ///   Implements the operator -.
        /// </summary>
        /// 
        public static Mat operator -(Mat v)
        {
            return v;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// 
        public static Mat operator <(Mat a, Mat v)
        {
            a.data = v.data;
            return a;
        }

        /// <summary>
        ///    Implements the operator &gt;.
        /// </summary>
        /// 
        public static Mat operator >(Mat a, Mat v)
        {
            return a;
        }

        /// <summary>
        ///   Performs an implicit conversion from 
        ///   <see cref="T:System.Double[]"/> to 
        ///   <see cref="REnvironment.Mat"/>.
        /// </summary>
        /// 
        public static implicit operator Mat(double[,] v)
        {
            return new Mat(v);
        }
        /// <summary>
        ///   Performs an implicit conversion from 
        ///   <see cref="REnvironment.Mat"/> 
        ///   to <see cref="T:System.Double[]"/>.
        /// </summary>
        /// 
        public static implicit operator double[,](Mat v)
        {
            return v.data;
        }
    }
}
