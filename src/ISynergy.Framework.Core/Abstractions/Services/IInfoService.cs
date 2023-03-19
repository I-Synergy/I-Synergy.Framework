namespace ISynergy.Framework.Core.Abstractions.Services
{
    /// <summary>
    /// Interface IInfoService
    /// </summary>
    public interface IInfoService : IVersionService
    {
        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        string ApplicationPath { get; }
        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        string CompanyName { get; }
        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        string ProductName { get; }
        /// <summary>
        /// Gets the copy rights.
        /// </summary>
        /// <value>The copy rights detail.</value>
        string Copyrights { get; }
    }
}
