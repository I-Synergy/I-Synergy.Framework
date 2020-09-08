using System;

namespace ISynergy.Framework.Models.Abstractions
{
    /// <summary>
    /// Interface ICaller
    /// </summary>
    public interface ICaller
    {
        /// <summary>
        /// Gets or sets the caller identifier.
        /// </summary>
        /// <value>The caller identifier.</value>
        Guid Caller_Id { get; set; }
        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        DateTimeOffset DateTime { get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the relation.
        /// </summary>
        /// <value>The relation.</value>
        string Relation { get; set; }
    }
}
