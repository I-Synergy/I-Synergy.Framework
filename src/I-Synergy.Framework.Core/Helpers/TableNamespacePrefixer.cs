using System;

namespace ISynergy.Helpers
{
    /// <summary>
    /// Generates a table name, with the namspace as a prefix. E.g.: 'namespace_name'.
    /// </summary>
    public class TableNamespacePrefixer
    {
        private const char _delimiter = '_';
        private readonly string _modelBaseNamespace = "ISynergy.Entities.";

        /// <param name="modelBaseNamespace">The starting part of the namespace that will be ignored.</param>
        public TableNamespacePrefixer(string modelBaseNamespace)
        {
            _modelBaseNamespace = modelBaseNamespace ?? throw new ArgumentNullException(nameof(modelBaseNamespace));
        }

        public string Prefix(Type type) => Prefix(type, type?.Name);

        public string Prefix(Type type, string tableName) => GetPrefix(type.Namespace) + _delimiter + tableName;

        public string Prefix(string typeNamespace, string tableName) => GetPrefix(typeNamespace) + _delimiter + tableName;

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
