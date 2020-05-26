using System.Diagnostics;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Represents a Rectangle in the Euclidean plane geometry.
    /// </summary>
    [DebuggerDisplay("{X}, {Y}, {Width}, {Height}")]
    public struct Rectangle
    {
        /// <summary>
        /// Invalid rectangle, which Width and Height properties are set to (-1).
        /// </summary>
        public static readonly Rectangle Invalid = new Rectangle(-1, -1, -1, -1);

        /// <summary>
        /// Empty rectangle which values are zeroes.
        /// </summary>
        public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// The X-coordinate of the rectangle.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(X))]
        public double X;

        /// <summary>
        /// The Y-coordinate of the rectangle.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(Y))]
        public double Y;

        /// <summary>
        /// The length of the rectangle along the X-axis.
        /// </summary>
        public double Width;

        /// <summary>
        /// The length of the rectangle along the Y-axis.
        /// </summary>
        public double Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle" /> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rectangle(double width, double height)
        {
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle" /> struct.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public Rectangle(Point point1, Point point2)
        {
            X = System.Math.Min(point1.X, point2.X);
            Y = System.Math.Min(point1.Y, point2.Y);
            Width = System.Math.Max(System.Math.Max(point1.X, point2.X) - X, 0);
            Height = System.Math.Max(System.Math.Max(point1.Y, point2.Y) - Y, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom side of the rectangle.
        /// </summary>
        /// <value>The bottom.</value>
        public double Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        /// <summary>
        /// Gets the X-coordinate of the right side of the rectangle.
        /// </summary>
        /// <value>The right.</value>
        public double Right
        {
            get
            {
                return X + Width;
            }
        }

        /// <summary>
        /// Gets the center of this rectangle.
        /// </summary>
        /// <value>The center.</value>
        public Point Center
        {
            get
            {
                return new Point(X + (Width / 2), Y + (Height / 2));
            }
        }

        /// <summary>
        /// Gets the location (Top-Left corner) of the rectangle.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
        }

        /// <summary>
        /// Rounds the rectangle's values to the closed whole number.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle Round(Rectangle rect)
        {
            return new Rectangle((int)(rect.X + .5d), (int)(rect.Y + .5d), (int)(rect.Width + .5d), (int)(rect.Height + .5d));
        }

        /// <summary>
        /// Rounds the rectangle's value to the closest less than or equal to whole numbers.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle Floor(Rectangle rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        /// <summary>
        /// Determines whether two <see cref="Rectangle" /> structures are equal.
        /// </summary>
        /// <param name="rect1">The rect1.</param>
        /// <param name="rect2">The rect2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Rectangle rect1, Rectangle rect2)
        {
            return rect1.X == rect2.X && rect1.Y == rect2.Y && rect1.Width == rect2.Width && rect1.Height == rect2.Height;
        }

        /// <summary>
        /// Determines whether two <see cref="Rectangle" /> structures are not equal.
        /// </summary>
        /// <param name="rect1">The rect1.</param>
        /// <param name="rect2">The rect2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Rectangle rect1, Rectangle rect2)
        {
            return !(rect1 == rect2);
        }

        /// <summary>
        /// Gets the difference between two <see cref="Rectangle" /> structures.
        /// </summary>
        /// <param name="rect1">The rect1.</param>
        /// <param name="rect2">The rect2.</param>
        /// <returns>Thickness.</returns>
        public static Thickness Subtract(Rectangle rect1, Rectangle rect2)
        {
            var left = System.Math.Abs(rect1.X - rect2.X);
            var top = System.Math.Abs(rect1.Y - rect2.Y);
            var right = System.Math.Abs(rect1.Right - rect2.Right);
            var bottom = System.Math.Abs(rect1.Bottom - rect2.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Gets a rectangle that has equal width and height and is centered within the specified rect.
        /// </summary>
        /// <param name="rect">The rect to create the square from.</param>
        /// <param name="offset">True to offset the rectangle's location to meet the smaller of the Width and Height properties.</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle ToSquare(Rectangle rect, bool offset)
        {
            var minLength = System.Math.Min(rect.Width, rect.Height);

            if (offset)
            {
                rect.X += (rect.Width - minLength) / 2;
                rect.Y += (rect.Height - minLength) / 2;
            }

            rect.Width = minLength;
            rect.Height = minLength;

            return rect;
        }

        /// <summary>
        /// Centers the specified rectangle within the provided available one.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle CenterRect(Rectangle rect, Rectangle bounds)
        {
            var offsetX = (bounds.Width - rect.Width) / 2;
            var offsetY = (bounds.Height - rect.Height) / 2;

            rect.X = bounds.X + offsetX;
            rect.Y = bounds.Y + offsetY;

            return rect;
        }

        /// <summary>
        /// Determines whether the current rect intersects with the specified one.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool IntersectsWith(Rectangle rect)
        {
            return rect.X <= X + Width &&
                rect.X + rect.Width >= X &&
                rect.Y <= Y + Height &&
                rect.Y + rect.Height >= Y;
        }

        /// <summary>
        /// Determines whether the size of this rect is valid - that is both Width and Height should be bigger than zero.
        /// </summary>
        /// <returns><c>true</c> if [is size valid]; otherwise, <c>false</c>.</returns>
        public bool IsSizeValid()
        {
            return Width > 0 && Height > 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Rectangle rect)
            {
                return rect == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "X = " + X + ", Y = " + Y + ", Width = " + Width + ", Height = " + Height;
        }

        /// <summary>
        /// Determines if this RadRect instance contains the point that is described by the arguments.
        /// </summary>
        /// <param name="x">The X coordinate of the point to check.</param>
        /// <param name="y">The Y coordinate of the point to check.</param>
        /// <returns>Returns true if this rectangle contains the point from the arguments and false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public bool Contains(double x, double y)
        {
            return x >= X && x <= X + Width &&
                   y >= Y && y <= Y + Height;
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
