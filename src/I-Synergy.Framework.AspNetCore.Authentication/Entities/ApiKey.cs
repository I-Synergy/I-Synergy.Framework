using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    /// <summary>
    /// Class ApiKey.
    /// Implements the <see cref="ClassBase" />
    /// </summary>
    /// <seealso cref="ClassBase" />
    public class ApiKey : ClassBase
    {
        /// <summary>
        /// Gets or sets the API key identifier.
        /// </summary>
        /// <value>The API key identifier.</value>
        [Required] [Identity] public Guid ApiKeyId { get; set; }
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>The account identifier.</value>
        [Required] public Guid AccountId { get; set; }
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [Required] public string Key { get; set; }
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTimeOffset CreatedDate { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        /// <value>The account.</value>
        public Account Account { get; set; }
    }
}
