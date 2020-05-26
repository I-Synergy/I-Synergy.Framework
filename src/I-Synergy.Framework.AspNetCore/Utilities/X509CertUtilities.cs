using System.Security.Cryptography.X509Certificates;

namespace ISynergy.Framework.AspNetCore.Utilities
{
    /// <summary>
    /// Class X509CertUtilities.
    /// </summary>
    public static class X509CertUtilities
    {
        /// <summary>
        /// Finds the cert from store.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <param name="findType">Type of the find.</param>
        /// <param name="findValue">The find value.</param>
        /// <param name="validOnly">if set to <c>true</c> [valid only].</param>
        /// <returns>System.Nullable&lt;X509Certificate2&gt;.</returns>
        public static X509Certificate2 FindCertFromStore(StoreName name, StoreLocation location, X509FindType findType, object findValue, bool validOnly)
        {
            using var store = new X509Store(name, location);
            store.Open(OpenFlags.ReadOnly);
            var certificateCollection = store.Certificates.Find(findType, findValue, validOnly);
            return certificateCollection.Count == 0 ? null : certificateCollection[0];
        }
    }
}
