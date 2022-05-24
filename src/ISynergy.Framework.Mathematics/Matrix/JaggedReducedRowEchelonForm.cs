namespace ISynergy.Framework.Mathematics
{
    using System;

    /// <summary>
    ///   Reduced row Echelon form
    /// </summary>
    /// 
    public class JaggedReducedRowEchelonForm
    {

        private double[][] rref;
        private int rows;
        private int cols;

        private int[] pivot;
        private int? freeCount;

        /// <summary>
        ///   Reduces a matrix to reduced row Echelon form.
        /// </summary>
        /// 
        /// <param name="value">The matrix to be reduced.</param>
        /// <param name="inPlace">
        ///   Pass <see langword="true"/> to perform the reduction in place. The matrix
        ///   <paramref name="value"/> will be destroyed in the process, resulting in less
        ///   memory consumption.</param>
        ///   
        public JaggedReducedRowEchelonForm(double[][] value, bool inPlace = false)
        {
            if (value is null)
                throw new ArgumentNullException("value");

            rref = inPlace ? value : value.MemberwiseClone();

            int lead = 0;
            rows = rref.Length;
            cols = rref[0].Length;

            pivot = new int[rows];
            for (var i = 0; i < pivot.Length; i++)
                pivot[i] = i;
            for (var r = 0; r < rows; r++)
            {
                if (cols <= lead)
                    break;

                int i = r;

                while (rref[i][lead] == 0)
                {
                    i++;

                    if (i >= rows)
                    {
                        i = r;

                        if (lead < cols - 1)
                            lead++;
                        else break;
                    }
                }

                if (i != r)
                {
                    // Swap rows i and r
                    for (var j = 0; j < cols; j++)
                    {
                        var temp = rref[r][j];
                        rref[r][j] = rref[i][j];
                        rref[i][j] = temp;
                    }

                    // Update indices
                    {
                        var temp = pivot[r];
                        pivot[r] = pivot[i];
                        pivot[i] = temp;
                    }
                }

                // Set to reduced row echelon form
                var div = rref[r][lead];
                if (div != 0)
                {
                    for (var j = 0; j < cols; j++)
                        rref[r][j] /= div;
                }

                for (var j = 0; j < rows; j++)
                {
                    if (j != r)
                    {
                        var sub = rref[j][lead];
                        for (var k = 0; k < cols; k++)
                            rref[j][k] -= sub * rref[r][k];
                    }
                }

                lead++;
            }
        }

        /// <summary>
        ///   Gets the pivot indicating the position
        ///   of the original rows before the swap.
        /// </summary>
        /// 
        public int[] Pivot { get { return pivot; } }

        /// <summary>
        ///   Gets the matrix in row reduced Echelon form.
        /// </summary>
        public double[][] Result { get { return rref; } }

        /// <summary>
        ///   Gets the number of free variables (linear
        ///   dependent rows) in the given matrix.
        /// </summary>
        public int FreeVariables
        {
            get
            {
                if (freeCount is null)
                    freeCount = count();

                return freeCount.Value;
            }
        }

        private int count()
        {
            for (var i = rows - 1; i >= 0; i--)
            {
                for (var j = 0; j < cols; j++)
                {
                    if (rref[i][j] != 0)
                        return rows - i - 1;
                }
            }

            return 0;
        }

    }
}
