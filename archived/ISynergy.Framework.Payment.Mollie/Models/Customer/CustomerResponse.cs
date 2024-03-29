﻿using System;
using System.Collections.Generic;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Models.Customer
{
    /// <summary>
    /// Class CustomerResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class CustomerResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a customer object. Will always contain customer for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The customer's unique identifier, for example cst_4pmbK7CqtT.
        /// Store this identifier for later recurring payments.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The mode used to create this payment. Mode determines whether a payment is real or a test payment.
        /// </summary>
        /// <value>The mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMode Mode { get; set; }

        /// <summary>
        /// Name of your customer.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// E-mailaddress of your customer.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Allows you to preset the language to be used in the payment screens shown to the consumer. If this parameter was not
        /// provided when the customer was created, the browser language will be used instead in the payment flow (which is usually
        /// more accurate).
        /// </summary>
        /// <value>The locale.</value>
        public string Locale { get; set; }

        /// <summary>
        /// Optional metadata. Use this if you want ISynergy.Framework.Payment.Mollie to store additional info.
        /// </summary>
        /// <value>The metadata.</value>
        [JsonConverter(typeof(RawJsonConverter))]
        public string Metadata { get; set; }

        /// <summary>
        /// Payment methods that the customer recently used for payments.
        /// </summary>
        /// <value>The recently used methods.</value>
        public List<PaymentMethods> RecentlyUsedMethods { get; set; }

        /// <summary>
        /// DateTime when user was created.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the customer. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public CustomerResponseLinks Links { get; set; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>T.</returns>
        public T GetMetadata<T>(JsonSerializerSettings jsonSerializerSettings = null)
        {
            return JsonConvert.DeserializeObject<T>(Metadata, jsonSerializerSettings);
        }
    }
}
