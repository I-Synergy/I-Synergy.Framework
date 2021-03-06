﻿using System;

namespace ISynergy.Framework.Payment.Mollie.Models.Profile.Response
{
    /// <summary>
    /// Class ApiKey.
    /// </summary>
    public class ApiKey
    {
        /// <summary>
        /// Indicates the response contains an API key object.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The API key's identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The actual API key, which you'll use when creating payments or when otherwise communicating with the API. Never
        /// share the API key with anyone.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// The API key's date and time of creation.
        /// </summary>
        /// <value>The created datetime.</value>
        public DateTime CreatedDatetime { get; set; }
    }
}
