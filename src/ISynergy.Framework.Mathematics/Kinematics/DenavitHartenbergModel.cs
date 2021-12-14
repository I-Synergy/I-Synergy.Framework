using System;
using System.Collections;
using System.Collections.Generic;

namespace ISynergy.Framework.Mathematics.Kinematics
{
    /// <summary>
    ///     Denavit Hartenberg model for joints.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class represents either a model itself or a submodel
    ///         when used with a
    ///         <see cref="DenavitHartenbergNode">
    ///             DenavitHartenbergModelCombinator instance
    ///         </see>
    ///         .
    ///     </para>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     Wikipedia contributors, "Denavit-Hartenberg parameters," Wikipedia,
    ///                     The Free Encyclopedia, available at:
    ///                     http://en.wikipedia.org/wiki/Denavit%E2%80%93Hartenberg_parameters
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <para>
    ///         The following example shows the creation and animation
    ///         of a 2-link planar manipulator.
    ///     </para>
    ///     <code>
    ///    // Create the DH-model at location (0, 0, 0) 
    /// 	  DenavitHartenbergModel model = new DenavitHartenbergModel();
    /// 	  
    /// 	  // Add the first joint
    /// 	  model.Joints.Add(alpha: 0, theta: Math.PI / 4, radius: 35, offset: 0);
    /// 	  
    ///    // Add the second joint
    /// 	  model.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 35, offset: 0);
    /// 	  
    /// 	  // Now move the arm
    /// 	  model.Joints[0].Parameters.Theta += Math.PI / 10;
    /// 	  model.Joints[1].Parameters.Theta -= Math.PI / 10;
    /// 	  
    /// 	  // Calculate the model
    /// 	  model.Compute();
    /// 	</code>
    /// </example>
    /// <seealso cref="DenavitHartenbergNode" />
    /// <seealso cref="DenavitHartenbergParameters" />
    [Serializable]
    public class DenavitHartenbergModel : IEnumerable<DenavitHartenbergJoint>
    {
        // TODO: remove the NonSerialized tags in case AForge.NET implements 
        // [Serializable] for those structs in the future (AForge.NET Issue 349)
        /// <summary>
        ///     Initializes a new instance of the <see cref="DenavitHartenbergModel" />
        ///     class given a specified model position in 3D space.
        /// </summary>
        /// <param name="position">The model's position in 3D space. Default is (0,0,0).</param>
        public DenavitHartenbergModel(Vector3 position)
        {
            Position = position;
            Joints = new DenavitHartenbergJointCollection();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DenavitHartenbergModel" />
        ///     class at the origin of the space (0,0,0).
        /// </summary>
        public DenavitHartenbergModel()
            : this(new Vector3(0, 0, 0))
        {
        }
        /// <summary>
        ///     Gets the model kinematic chain.
        /// </summary>
        public DenavitHartenbergJointCollection Joints { get; }

        /// <summary>
        ///     Gets or sets the model position.
        /// </summary>
        [field: NonSerialized]
        public Vector3 Position { get; private set; }

        /// <summary>
        ///     Gets the transformation matrix T for the full model, given
        ///     as T = T_0 * T_1 * T_2 ...T_n in which T_i is the transform
        ///     matrix for each joint in the model.
        /// </summary>
        [field: NonSerialized]
        public Matrix4x4 Transform { get; private set; }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DenavitHartenbergJoint> GetEnumerator()
        {
            return Joints.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Joints.GetEnumerator();
        }
        /// <summary>
        ///     Computes the entire model, calculating the
        ///     final position for each joint in the model.
        /// </summary>
        /// <returns>The model transformation matrix</returns>
        public void Compute()
        {
            compute(Matrix4x4.Identity, Position);
        }

        /// <summary>
        ///     Calculates the entire model given it is attached to a parent model and computes each joint position.
        /// </summary>
        /// <param name="parent">Parent model this model is attached to.</param>
        /// <returns>Model transform matrix of the whole chain (parent + model).</returns>
        /// <remarks>This function assumes the parent model has already been calculated.</remarks>
        public void Compute(DenavitHartenbergModel parent)
        {
            if (parent is null)
                throw new ArgumentNullException("parent", "Parent cannot be null.");
            // The model position will be the position
            // of the last joint in the parent model:
            //
            Position = parent.Joints[parent.Joints.Count - 1].Position;

            // The model position is the parent model 
            // position, due the following DH formula: 
            //
            //   Pn = Z0 * X0 * Z1 * X1 ... Zn * Xn * P0
            //
            // in which Z and X 4x4 matrices)

            compute(parent.Transform, parent.Position);
        }
        private void compute(Matrix4x4 currentMatrix, Vector3 position)
        {
            Vector4 referencePosition;
            referencePosition.W = 1;
            referencePosition.X = position.X;
            referencePosition.Y = position.Y;
            referencePosition.Z = position.Z;

            // For each joint in this model
            foreach (var joint in this)
                // Compute the joint using forward kinematics
                currentMatrix = joint.Compute(currentMatrix, referencePosition);

            // Update the model transformation matrix
            Transform = currentMatrix;
        }
    }
}