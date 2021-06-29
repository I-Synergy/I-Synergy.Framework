namespace ISynergy.Framework.Mathematics.IO
{
    using System;

    /// <summary>
    ///   Sparse matrix representation used by
    ///   <see cref="MatReader">.MAT files</see>.
    /// </summary>
    /// 
    public class MatSparse
    {
        /// <summary>
        ///   Gets the sparse row index vector.
        /// </summary>
        /// 
        public int[] Rows { get; private set; }

        /// <summary>
        ///   Gets the sparse column index vector.
        /// </summary>
        /// 
        public int[] Columns { get; private set; }

        /// <summary>
        ///   Gets the values vector.
        /// </summary>
        /// 
        public Array Values { get; private set; }

        internal MatSparse(int[] ir, int[] ic, Array values)
        {
            Rows = ir;
            Columns = ic;
            Values = values;
        }
    }
}
