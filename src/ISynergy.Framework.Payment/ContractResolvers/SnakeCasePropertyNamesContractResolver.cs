namespace ISynergy.Framework.Payment.ContractResolvers
{
    /// <summary>
    /// Class SnakeCasePropertyNamesContractResolver.
    /// Implements the <see cref="DeliminatorSeparatedPropertyNamesContractResolver" />
    /// </summary>
    /// <seealso cref="DeliminatorSeparatedPropertyNamesContractResolver" />
    public class SnakeCasePropertyNamesContractResolver : DeliminatorSeparatedPropertyNamesContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeCasePropertyNamesContractResolver" /> class.
        /// </summary>
        public SnakeCasePropertyNamesContractResolver() : base('_')
        {
        }
    }
}
