namespace ISynergy.Framework.EntityFramework.Helpers
{
    /// <summary>
    /// Generates a table name, with the namespace as a prefix. E.g.: 'namespace_name'.
    /// </summary>
    public class TableNamespacePrefixer
    {
        /// <summary>
        /// The delimiter
        /// </summary>
        private const char _delimiter = '_';
        /// <summary>
        /// The model base namespace
        /// </summary>
        private readonly string _modelBaseNamespace = "ISynergy.Entities.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TableNamespacePrefixer"/> class.
        /// </summary>
        /// <param name="modelBaseNamespace">The starting part of the namespace that will be ignored.</param>
        /// <exception cref="ArgumentNullException">modelBaseNamespace</exception>
        public TableNamespacePrefixer(string modelBaseNamespace)
        {
            _modelBaseNamespace = modelBaseNamespace ?? throw new ArgumentNullException(nameof(modelBaseNamespace));
        }

        /// <summary>
        /// Prefixes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public string Prefix(Type type) => Prefix(type, type.Name);

        /// <summary>
        /// Prefixes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        public string Prefix(Type type, string tableName) => GetPrefix(type.Namespace) + _delimiter + tableName;

        /// <summary>
        /// Prefixes the specified type namespace.
        /// </summary>
        /// <param name="typeNamespace">The type namespace.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        public string Prefix(string typeNamespace, string tableName) => GetPrefix(typeNamespace) + _delimiter + tableName;

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <param name="typeNamespace">The type namespace.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="InvalidOperationException">Entity should have {_modelBaseNamespace} as namespace!</exception>
        private string GetPrefix(string typeNamespace)
        {
            if (typeNamespace?.StartsWith(_modelBaseNamespace) != true)
                throw new InvalidOperationException($"Entity should have {_modelBaseNamespace} as namespace!");

            var result = typeNamespace
                .Remove(0, _modelBaseNamespace.Length)
                .Replace('.', '_');

            return result;
        }
    }
}
