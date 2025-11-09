using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Static helper methods for <see cref="o:Windows.UI.Xaml.Media.Matrix" />.
/// These extension methods provide WPF-compatible matrix operations for UWP/WinUI platforms.
/// </summary>
public static class MatrixExtensions
{
    /// <summary>
    /// Gets the matrix that represents this transform.
    /// Implements WPF's SkewTransform.Value.
    /// </summary>
    /// <param name="transform">Extended SkewTranform.</param>
    /// <returns>Matrix representing transform.</returns>
    public static Matrix GetMatrix(this ScaleTransform transform)
    {
        return Matrix.Identity.ScaleAt(transform.ScaleX, transform.ScaleY, transform.CenterX, transform.CenterY);
    }

    /// <summary>
    /// Gets the matrix that represents this transform.
    /// Implements WPF's SkewTransform.Value.
    /// </summary>
    /// <param name="transform">Extended SkewTranform.</param>
    /// <returns>Matrix representing transform.</returns>
    public static Matrix GetMatrix(this RotateTransform transform)
    {
        return Matrix.Identity.RotateAt(transform.Angle, transform.CenterX, transform.CenterY);
    }

    /// <summary>
    /// Gets the matrix that represents this transform.
    /// Implements WPF's SkewTransform.Value.
    /// </summary>
    /// <param name="transform">Extended SkewTranform.</param>
    /// <returns>Matrix representing transform.</returns>
    public static Matrix GetMatrix(this SkewTransform transform)
    {
        var matrix = Matrix.Identity;

        var angleX = transform.AngleX;
        var angleY = transform.AngleY;
        var centerX = transform.CenterX;
        var centerY = transform.CenterY;

        var hasCenter = centerX != 0 || centerY != 0;

        if (hasCenter)
        {
            // If we have a center, translate matrix before/after skewing.
            matrix = matrix.Translate(-centerX, -centerY);
            matrix = matrix.Skew(angleX, angleY);
            matrix = matrix.Translate(centerX, centerY);
        }
        else
        {
            matrix = matrix.Skew(angleX, angleY);
        }

        return matrix;
    }

    /// <summary>
    /// Implements WPF's Matrix.HasInverse.
    /// Determines if the matrix has an inverse by checking if the determinant is non-zero.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>True if matrix has an inverse; otherwise, false.</returns>
    /// <remarks>
    /// A matrix has an inverse if and only if its determinant is non-zero.
    /// This is equivalent to WPF's <c>matrix.HasInverse</c> property.
    /// </remarks>
    public static bool HasInverse(this Matrix matrix)
    {
        // Calculate determinant: M11 * M22 - M12 * M21
        // Matrix has an inverse if determinant != 0
        return ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)) != 0;
    }

    /// <summary>
    /// Applies a rotation of the specified angle about the origin of this Matrix structure and returns the result.
    /// </summary>
    /// <param name="matrix">Matrix to extend.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <returns>Rotated Matrix.</returns>
    public static Matrix Rotate(this Matrix matrix, double angle)
    {
        return matrix.Multiply(CreateRotationRadians(angle % 360 * (Math.PI / 180.0)));
    }

    /// <summary>
    /// Rotates this matrix about the specified point and returns the new result.
    /// </summary>
    /// <param name="matrix">Matrix to extend.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="centerX">The x-coordinate of the point about which to rotate this matrix.</param>
    /// <param name="centerY">The y-coordinate of the point about which to rotate this matrix.</param>
    /// <returns>Rotated Matrix.</returns>
    public static Matrix RotateAt(this Matrix matrix, double angle, double centerX, double centerY)
    {
        return matrix.Multiply(CreateRotationRadians(angle % 360 * (Math.PI / 180.0), centerX, centerY));
    }

    /// <summary>
    /// Appends the specified scale vector to this Matrix structure and returns the result.
    /// </summary>
    /// <param name="matrix">Matrix to extend.</param>
    /// <param name="scaleX">The value by which to scale this Matrix along the x-axis.</param>
    /// <param name="scaleY">The value by which to scale this Matrix along the y-axis.</param>
    /// <returns>Scaled Matrix.</returns>
    public static Matrix Scale(this Matrix matrix, double scaleX, double scaleY)
    {
        return matrix.Multiply(CreateScaling(scaleX, scaleY));
    }

    /// <summary>
    /// Scales this Matrix by the specified amount about the specified point and returns the result.
    /// </summary>
    /// <param name="matrix">Matrix to extend.</param>
    /// <param name="scaleX">The value by which to scale this Matrix along the x-axis.</param>
    /// <param name="scaleY">The value by which to scale this Matrix along the y-axis.</param>
    /// <param name="centerX">The x-coordinate of the scale operation's center point.</param>
    /// <param name="centerY">The y-coordinate of the scale operation's center point.</param>
    /// <returns>Scaled Matrix.</returns>
    public static Matrix ScaleAt(this Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
    {
        return matrix.Multiply(CreateScaling(scaleX, scaleY, centerX, centerY));
    }

    /// <summary>
    /// Appends a skew of the specified degrees in the x and y dimensions to this Matrix structure and returns the result.
    /// </summary>
    /// <param name="matrix">Matrix to extend.</param>
    /// <param name="skewX">The angle in the x dimension by which to skew this Matrix.</param>
    /// <param name="skewY">The angle in the y dimension by which to skew this Matrix.</param>
    /// <returns>Skewed Matrix.</returns>
    public static Matrix Skew(this Matrix matrix, double skewX, double skewY)
    {
        return matrix.Multiply(CreateSkewRadians(skewX % 360 * (Math.PI / 180.0), skewY % 360 * (Math.PI / 180.0)));
    }

    /// <summary>
    /// Translates the matrix by the given amount and returns the result.
    /// </summary>
    /// <param name="matrix">Matrix to extend.</param>
    /// <param name="offsetX">The offset in the x dimension.</param>
    /// <param name="offsetY">The offset in the y dimension.</param>
    /// <returns>Translated Matrix.</returns>
    public static Matrix Translate(this Matrix matrix, double offsetX, double offsetY)
    {
        return new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX + offsetX, matrix.OffsetY + offsetY);
    }

    /// <summary>
    /// Creates the rotation radians.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <returns>Matrix.</returns>
    internal static Matrix CreateRotationRadians(this double angle)
    {
        return CreateRotationRadians(angle, 0, 0);
    }

    /// <summary>
    /// Creates the rotation radians.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <param name="centerX">The center x.</param>
    /// <param name="centerY">The center y.</param>
    /// <returns>Matrix.</returns>
    internal static Matrix CreateRotationRadians(this double angle, double centerX, double centerY)
    {
        var sin = Math.Sin(angle);
        var cos = Math.Cos(angle);
        var dx = (centerX * (1.0 - cos)) + (centerY * sin);
        var dy = (centerY * (1.0 - cos)) - (centerX * sin);

        return new Matrix(cos, sin, -sin, cos, dx, dy);
    }

    /// <summary>
    /// Creates the scaling.
    /// </summary>
    /// <param name="scaleX">The scale x.</param>
    /// <param name="scaleY">The scale y.</param>
    /// <returns>Matrix.</returns>
    internal static Matrix CreateScaling(this double scaleX, double scaleY)
    {
        return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
    }

    /// <summary>
    /// Creates the scaling.
    /// </summary>
    /// <param name="scaleX">The scale x.</param>
    /// <param name="scaleY">The scale y.</param>
    /// <param name="centerX">The center x.</param>
    /// <param name="centerY">The center y.</param>
    /// <returns>Matrix.</returns>
    internal static Matrix CreateScaling(this double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
    }

    /// <summary>
    /// Creates the skew radians.
    /// </summary>
    /// <param name="skewX">The skew x.</param>
    /// <param name="skewY">The skew y.</param>
    /// <returns>Matrix.</returns>
    internal static Matrix CreateSkewRadians(this double skewX, double skewY)
    {
        return new Matrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0);
    }

    /// <summary>
    /// Implements WPF's Matrix.Multiply.
    /// </summary>
    /// <param name="matrix1">The left matrix.</param>
    /// <param name="matrix2">The right matrix.</param>
    /// <returns>The product of the two matrices.</returns>
    public static Matrix Multiply(this Matrix matrix1, Matrix matrix2)
    {
        // WPF equivalent of following code:
        // return Matrix.Multiply(matrix1, matrix2);
        return new Matrix(
            (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
            (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
            (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
            (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
            (matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21) + matrix2.OffsetX,
            (matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22) + matrix2.OffsetY);
    }

    /// <summary>
    /// Rounds the non-offset elements of a matrix to avoid issues due to floating point imprecision and returns the result.
    /// </summary>
    /// <param name="matrix">The matrix to round.</param>
    /// <param name="decimalsAfterRound">The number of decimals after the round.</param>
    /// <returns>The rounded matrix.</returns>
    public static Matrix Round(this Matrix matrix, int decimalsAfterRound)
    {
        return new Matrix(
            Math.Round(matrix.M11, decimalsAfterRound),
            Math.Round(matrix.M12, decimalsAfterRound),
            Math.Round(matrix.M21, decimalsAfterRound),
            Math.Round(matrix.M22, decimalsAfterRound),
            matrix.OffsetX,
            matrix.OffsetY);
    }

    /// <summary>
    /// Implement WPF's Rect.Transform.
    /// </summary>
    /// <param name="matrix">The matrix to use to transform the rectangle.</param>
    /// <param name="rectangle">The rectangle to transform.</param>
    /// <returns>The transformed rectangle.</returns>
    public static Rect RectTransform(this Matrix matrix, Rect rectangle)
    {
        // WPF equivalent of following code:
        // var rectTransformed = Rect.Transform(rect, matrix);
        var leftTop = matrix.Transform(new Point(rectangle.Left, rectangle.Top));
        var rightTop = matrix.Transform(new Point(rectangle.Right, rectangle.Top));
        var leftBottom = matrix.Transform(new Point(rectangle.Left, rectangle.Bottom));
        var rightBottom = matrix.Transform(new Point(rectangle.Right, rectangle.Bottom));
        var left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
        var top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
        var right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
        var bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
        var rectTransformed = new Rect(left, top, right - left, bottom - top);
        return rectTransformed;
    }

    /// <summary>
    /// Implement WPF's <c>Rect.Transform(Matrix)</c> logic.
    /// </summary>
    /// <param name="rectangle">The rectangle to transform.</param>
    /// <param name="matrix">The matrix to use to transform the rectangle.
    /// </param>
    /// <returns>The transformed rectangle.</returns>
    public static Rect Transform(this Rect rectangle, Matrix matrix)
    {
        Point leftTop = matrix.Transform(new Point(rectangle.Left, rectangle.Top));
        Point rightTop = matrix.Transform(new Point(rectangle.Right, rectangle.Top));
        Point leftBottom = matrix.Transform(new Point(rectangle.Left, rectangle.Bottom));
        Point rightBottom = matrix.Transform(new Point(rectangle.Right, rectangle.Bottom));

        double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
        double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
        double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
        double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));

        return new(left, top, right - left, bottom - top);
    }
}
