using System.Diagnostics;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Represents a structure that defines a size in the two-dimensional space.
    /// </summary>
    [DebuggerDisplay("{Width}, {Height}")]
    public struct Size
    {
        /// <summary>
        /// A <see cref="Size" /> instance which Width and Height are set to 0.
        /// </summary>
        public static readonly Size Empty = new Size(0, 0);

        /// <summary>
        /// A <see cref="Size" /> instance which Width and Height are set to -1.
        /// </summary>
        public static readonly Size Invalid = new Size(-1, -1);

        /// <summary>
        /// The length along the horizontal axis.
        /// </summary>
        public double Width;

        /// <summary>
        /// The length along the vertical axis.
        /// </summary>
        public double Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size" /> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Determines whether two <see cref="Size" /> structures are equal.
        /// </summary>
        /// <param name="size1">The size1.</param>
        /// <param name="size2">The size2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        /// <summary>
        /// Determines whether two <see cref="Size" /> structures are not equal.
        /// </summary>
        /// <param name="size1">The size1.</param>
        /// <param name="size2">The size2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Size size)
            {
                return size == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
