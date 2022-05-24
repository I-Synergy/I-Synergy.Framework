using System;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Type utilities.
    /// </summary>
    public static class TypeActivator
    {
        /// <summary>
        /// Creates a new instance from AQN.
        /// </summary>
        /// <param name="assemblyQualifiedName">Name of the assembly qualified.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentException">Unable to resolve object type: " + assemblyQualifiedName</exception>
        public static object CreateInstance(string assemblyQualifiedName)
        {
            var targetType = Type.GetType(assemblyQualifiedName, false, false);

            if (targetType is null)
                throw new ArgumentException("Unable to resolve object type: " + assemblyQualifiedName);

            return CreateInstance(targetType);
        }

        /// <summary>
        /// Creates a new instance of object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public static T CreateInstance<T>() => (T)CreateInstance(typeof(T));

        /// <summary>
        /// general object creation
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentException">Unable to instantiate type: " + targetType.AssemblyQualifiedName + " - Unknown Error</exception>
        public static object CreateInstance(Type targetType)
        {
            object result;

            // Check if type is not a string. It has no parameterless constructor.
            if (Type.GetTypeCode(targetType) == TypeCode.String)
                return string.Empty;

            // get the default constructor and instantiate
            var constructorInfo = targetType.GetConstructor(Array.Empty<Type>());

            // Check if type has no constructors.
            if (constructorInfo is null)
                result = Activator.CreateInstance(targetType);
            else
                result = constructorInfo.Invoke(null);

            if (result is null)
                throw new ArgumentException("Unable to instantiate type: " + targetType.AssemblyQualifiedName + " - Unknown Error");

            return result;
        }
    }
}
