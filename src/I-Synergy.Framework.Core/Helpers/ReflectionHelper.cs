using System;
using System.Linq.Expressions;

namespace ISynergy.Helpers
{
    /// <summary>
    /// Class containing methods for extracting member information using reflection.
    /// </summary>
    public static class Members
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T">The type of the class or interface.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression pointing to a property.</param>
        /// <returns>The name of the property.</returns>
        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var member = (MemberExpression)propertyExpression.Body;
            return member.Member.Name;
        }
    }
}
