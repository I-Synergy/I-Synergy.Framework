using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Vectors;

namespace ISynergy.Framework.Mathematics.Kinematics;

/// <summary>
///     Denavit-Hartenberg Model Joint.
/// </summary>
[Serializable]
public class DenavitHartenbergJoint
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DenavitHartenbergJoint" /> class.
    /// </summary>
    /// <param name="parameters">
    ///     The
    ///     <see cref="DenavitHartenbergParameters">
    ///         parameters
    ///     </see>
    ///     to be used to create the joint.
    /// </param>
    public DenavitHartenbergJoint(DenavitHartenbergParameters parameters)
    {
        Matrix = new DenavitHartenbergMatrix();
        Position = new Vector3(0, 0, 0);
        Parameters = parameters;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DenavitHartenbergJoint" /> class.
    /// </summary>
    /// <param name="alpha">Angle in radians on the Z axis relatively to the last joint.</param>
    /// <param name="theta">Angle in radians on the X axis relatively to the last joint.</param>
    /// <param name="radius">Length or radius of the joint.</param>
    /// <param name="offset">Offset along Z axis relatively to the last joint.</param>
    public DenavitHartenbergJoint(double alpha = 0, double theta = 0, double radius = 0, double offset = 0)
        : this(new DenavitHartenbergParameters(alpha, theta, radius, offset))
    {
    }

    /// <summary>
    ///     Gets or sets the current <see cref="DenavitHartenbergMatrix" /> associated with this joint.
    /// </summary>
    public DenavitHartenbergMatrix Matrix { get; private set; }

    /// <summary>
    ///     Gets or sets the position of this joint.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    ///     Gets or sets the parameters for this joint.
    /// </summary>
    public DenavitHartenbergParameters Parameters { get; set; }

    /// <summary>
    ///     Updates the joint transformation matrix and position
    ///     given a model transform matrix and reference position.
    /// </summary>
    public Matrix4x4 Compute(Matrix4x4 transformMatrix, Vector4 referencePosition)
    {
        // Calculate the new joint
        Matrix.Compute(Parameters);

        // Calculate the joint transformation matrix relative to the full model
        transformMatrix = transformMatrix * Matrix.Transform;

        // Calculate the position of the joint as:
        // Pn = Z0 * X0 * Z1 * X1 ... Zi * Xi * P0
        Position = Matrix4x4.Multiply(transformMatrix, referencePosition).ToVector3();

        return transformMatrix; // Return the new matrix, to pass along the chain
    }
}

/// <summary>
///     Collection of Denavit Hartenberg Joints.
/// </summary>
[Serializable]
public class DenavitHartenbergJointCollection : List<DenavitHartenbergJoint>
{
    /// <summary>
    ///     Adds an object to the end of this <see cref="DenavitHartenbergJointCollection" />.
    /// </summary>
    /// <param name="parameters">
    ///     The
    ///     <see cref="DenavitHartenbergParameters">
    ///         parameters
    ///     </see>
    ///     specifying the joint to be added.
    /// </param>
    public void Add(DenavitHartenbergParameters parameters)
    {
        Add(parameters.Alpha, parameters.Theta, parameters.Radius, parameters.Offset);
    }

    /// <summary>
    ///     Adds an object to the end of this <see cref="DenavitHartenbergJointCollection" />.
    /// </summary>
    /// <param name="alpha">Angle in radians on the Z axis relatively to the last joint.</param>
    /// <param name="theta">Angle in radians on the X axis relatively to the last joint.</param>
    /// <param name="radius">Length or radius of the joint.</param>
    /// <param name="offset">Offset along Z axis relatively to the last joint.</param>
    public void Add(double alpha = 0, double theta = 0, double radius = 0, double offset = 0)
    {
        Add(new DenavitHartenbergJoint(alpha, theta, radius, offset));
    }
}