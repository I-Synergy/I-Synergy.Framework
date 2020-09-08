namespace ISynergy.Framework.Payment.ContractResolvers
{
    /// <summary>
    /// Class DeliminatorSeparatedPropertyNamesContractResolver.
    /// Implements the <see cref="DefaultContractResolver" />
    /// </summary>
    /// <seealso cref="DefaultContractResolver" />
    public class DeliminatorSeparatedPropertyNamesContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// The separator
        /// </summary>
        private readonly string _separator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliminatorSeparatedPropertyNamesContractResolver" /> class.
        /// </summary>
        /// <param name="separator">The separator.</param>
        protected DeliminatorSeparatedPropertyNamesContractResolver(char separator)
        {
            _separator = separator.ToString();
        }

        /// <summary>
        /// Resolves the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>System.String.</returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            for (var j = propertyName.Length - 1; j > 0; j--)
                if (j > 0 && char.IsUpper(propertyName[j]) || j > 0 && char.IsNumber(propertyName[j]) &&
                    !char.IsNumber(propertyName[j - 1]))
                    propertyName = propertyName.Insert(j, _separator);
            return propertyName.ToLower();
        }
    }
}
