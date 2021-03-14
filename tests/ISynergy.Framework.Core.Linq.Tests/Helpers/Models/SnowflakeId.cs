using System;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Struct SnowflakeId
    /// Implements the <see cref="IEquatable{SnowflakeId}" />
    /// </summary>
    /// <seealso cref="IEquatable{SnowflakeId}" />
    public struct SnowflakeId : IEquatable<SnowflakeId>
    {
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(SnowflakeId other)
        {
            return Value == other.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is SnowflakeId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SnowflakeId left, SnowflakeId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SnowflakeId left, SnowflakeId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SnowflakeId left, int right)
        {
            return (int)left.Value == right;
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SnowflakeId left, int right)
        {
            return (int)left.Value != right;
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SnowflakeId left, ulong right)
        {
            return left.Value == right;
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SnowflakeId left, ulong right)
        {
            return left.Value != right;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public ulong Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeId"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public SnowflakeId(ulong value)
        {
            Value = value;
        }
    }
}
