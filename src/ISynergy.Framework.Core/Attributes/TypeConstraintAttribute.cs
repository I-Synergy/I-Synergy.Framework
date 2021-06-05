using System;

namespace ISynergy.Framework.Core.Attributes
{
	/// <summary>
	/// Specifies type constraints on the AssociatedObject of IBehavior"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class TypeConstraintAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeConstraintAttribute"/> class.
		/// </summary>
		/// <param name="constraint">The constraint type.</param>
		public TypeConstraintAttribute(Type constraint)
		{
			this.Constraint = constraint;
		}

		/// <summary>
		/// Gets the constraint type.
		/// </summary>
		public Type Constraint
		{
			get;
			private set;
		}
	}
}
