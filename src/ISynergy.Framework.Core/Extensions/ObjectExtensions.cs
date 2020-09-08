using System;
using Newtonsoft.Json;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source) =>
            JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));

        /// <summary>
        /// Checks if object is of a nullable type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <returns><c>true</c> if [is nullable type] [the specified self]; otherwise, <c>false</c>.</returns>
        public static bool IsNullableType<T>(this T self) where T : Type =>
            Nullable.GetUnderlyingType(self) != null;
    }
}
