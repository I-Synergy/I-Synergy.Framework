using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Vectors;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Kinematics;

/// <summary>
///     Denavit Hartenberg matrix (commonly referred as T).
/// </summary>
public class DenavitHartenbergMatrix
{
    /// <summary>
    ///     Gets or sets the transformation matrix T (as in T = Z * X).
    /// </summary>
    public Matrix4x4 Transform { get; set; }

    /// <summary>
    ///     Gets or sets the matrix regarding X axis transformations.
    /// </summary>
    public Matrix4x4 X { get; set; }

    /// <summary>
    ///     Gets or sets the matrix regarding Z axis transformations.
    /// </summary>
    public Matrix4x4 Z { get; set; }
    /// <summary>
    ///     Executes the transform calculations (T = Z*X).
    /// </summary>
    /// <returns>Transform matrix T.</returns>
    /// <remarks>Calling this method also updates the Transform property.</remarks>
    public void Compute(DenavitHartenbergParameters parameters)
    {
        // Calculate Z with Z = TranslationZ(d).RotationZ(theta)
        Z = Matrix4x4.Multiply
        (
            Matrix4x4.CreateTranslation(new Vector3(0f, 0f, (float)parameters.Offset)),
            Matrix4x4.CreateRotationZ((float)parameters.Theta)
        );

        // Calculate X with X = TranslationX(radius).RotationZ(alpha)
        X = Matrix4x4.Multiply
        (
            Matrix4x4.CreateTranslation(new Vector3((float)parameters.Radius, 0f, 0f)),
            Matrix4x4.CreateRotationX((float)parameters.Alpha)
        );

        // Calculate the transform with T=Z.X
        Transform = Matrix4x4.Multiply(Z, X);
    }
}