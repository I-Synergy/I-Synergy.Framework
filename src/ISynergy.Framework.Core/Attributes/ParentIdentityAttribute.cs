using System;

namespace ISynergy.Framework.Core.Attributes
{
    /// <summary>
    /// Class ParentIdentityAttribute. This class cannot be inherited.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParentIdentityAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentIdentityAttribute"/> class.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        public ParentIdentityAttribute(Type propertyType)
        {
            PropertyType = propertyType;
        }
    }
}
