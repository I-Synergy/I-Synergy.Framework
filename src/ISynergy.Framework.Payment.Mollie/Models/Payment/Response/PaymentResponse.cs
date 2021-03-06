﻿using System;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response
{
    /// <summary>
    /// Class PaymentResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class PaymentResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a payment object. Will always contain payment for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The identifier uniquely referring to this payment. ISynergy.Framework.Payment.Mollie assigns this identifier randomly at payment creation
        /// time. For example tr_7UhSN1zuXS. Its ID will always be used by ISynergy.Framework.Payment.Mollie to refer to a certain payment.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The mode used to create this payment. Mode determines whether a payment is real or a test payment.
        /// </summary>
        /// <value>The mode.</value>
        public PaymentMode Mode { get; set; }

        /// <summary>
        /// The payment's date and time of creation, in ISO 8601 format.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The payment's status. Please refer to the page about statuses for more info about which statuses occur at what
        /// point.
        /// </summary>
        /// <value>The status.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus? Status { get; set; }

        /// <summary>
        /// Whether or not the payment can be canceled.
        /// </summary>
        /// <value><c>true</c> if this instance is cancelable; otherwise, <c>false</c>.</value>
        public bool IsCancelable { get; set; }

        /// <summary>
        /// The date and time the payment became authorized, in ISO 8601 format. This parameter is omitted if the payment is not authorized (yet).
        /// </summary>
        /// <value>The authorized at.</value>
        public DateTime? AuthorizedAt { get; set; }

        /// <summary>
        /// The date and time the payment became paid, in ISO 8601 format. Null is returned if the payment isn't completed
        /// (yet).
        /// </summary>
        /// <value>The paid at.</value>
        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// The date and time the payment was cancelled, in ISO 8601 format. Null is returned if the payment isn't cancelled
        /// (yet).
        /// </summary>
        /// <value>The canceled at.</value>
        public DateTime? CanceledAt { get; set; }

        /// <summary>
        /// The date and time the payment was expired, in ISO 8601 format. Null is returned if the payment did not expire
        /// (yet).
        /// </summary>
        /// <value>The expires at.</value>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// The time until the payment will expire in ISO 8601 duration format.
        /// </summary>
        /// <value>The expired at.</value>
        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// The date and time the payment failed, in ISO 8601 format. This parameter is omitted if the payment did not fail (yet).
        /// </summary>
        /// <value>The failed at.</value>
        public DateTime? FailedAt { get; set; }

        /// <summary>
        /// The amount of the payment, e.g. {"currency":"EUR", "value":"100.00"} for a €100.00 payment.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// Only available when refunds are available for this payment – The total amount in EURO that is already refunded. For
        /// some payment methods, this
        /// amount may be higher than the payment amount, for example to allow reimbursement of the costs for a return shipment
        /// to the consumer.
        /// </summary>
        /// <value>The amount refunded.</value>
        public Amount AmountRefunded { get; set; }

        /// <summary>
        /// Only available when refunds are available for this payment – The remaining amount in EURO that can be refunded.
        /// </summary>
        /// <value>The amount remaining.</value>
        public Amount AmountRemaining { get; set; }

        /// <summary>
        /// The total amount that is already captured for this payment. Only available when this payment supports captures.
        /// </summary>
        /// <value>The amount captured.</value>
        public Amount AmountCaptured { get; set; }

        /// <summary>
        /// A short description of the payment. The description will be shown on the consumer's bank or card statement when
        /// possible.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// The URL the consumer will be redirected to after completing or cancelling the payment process.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// The URL ISynergy.Framework.Payment.Mollie will call as soon an important status change takes place.
        /// </summary>
        /// <value>The webhook URL.</value>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// The payment method used for this payment, either forced on creation by specifying the method parameter, or chosen
        /// by the consumer our
        /// payment method selection screen.
        /// </summary>
        /// <value>The method.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethods? Method { get; set; }

        /// <summary>
        /// The optional metadata you provided upon payment creation. Metadata can be used to link an order to a payment.
        /// </summary>
        /// <value>The metadata.</value>
        [JsonConverter(typeof(RawJsonConverter))]
        public string Metadata { get; set; }

        /// <summary>
        /// The consumer's locale, either forced on creation by specifying the locale parameter, or detected by us during
        /// checkout.
        /// </summary>
        /// <value>The locale.</value>
        public string Locale { get; set; }

        /// <summary>
        /// The customer’s ISO 3166-1 alpha-2 country code, detected by us during checkout. For example: BE.
        /// </summary>
        /// <value>The country code.</value>
        public string CountryCode { get; set; }

        /// <summary>
        /// The identifier referring to the profile this payment was created on. For example, pfl_QkEhN94Ba.
        /// </summary>
        /// <value>The profile identifier.</value>
        public string ProfileId { get; set; }

        /// <summary>
        /// This optional field will contain the amount that will be settled to your account, converted to the currency your
        /// account is settled in. It follows the same syntax as the amount property.
        /// </summary>
        /// <value>The settlement amount.</value>
        public Amount SettlementAmount { get; set; }

        /// <summary>
        /// The identifier referring to the settlement this payment belongs to. For example, stl_BkEjN2eBb.
        /// </summary>
        /// <value>The settlement identifier.</value>
        public string SettlementId { get; set; }

        /// <summary>
        /// The customerid of this payment
        /// </summary>
        /// <value>The customer identifier.</value>
        public string CustomerId { get; set; }

        /// <summary>
        /// Indicates which type of payment this is in a recurring sequence. Set to first for first payments that allow the customer to agree
        /// to automatic recurring charges taking place on their account in the future. Set to recurring for payments where the customer’s card
        /// is charged automatically.
        /// </summary>
        /// <value>The type of the sequence.</value>
        public SequenceType? SequenceType { get; set; }


        /// <summary>
        /// Only available for recurring payments – If the payment is a recurring payment, this field will hold the ID of the
        /// mandate used to authorize the recurring payment.
        /// </summary>
        /// <value>The mandate identifier.</value>
        public string MandateId { get; set; }

        /// <summary>
        /// Only available for recurring payments – When implementing the Subscriptions API, any recurring charges resulting
        /// from the subscription will hold the ID of the subscription that triggered the payment.
        /// </summary>
        /// <value>The subscription identifier.</value>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// If the payment was created for an order, the ID of that order will be part of the response.
        /// </summary>
        /// <value>The order identifier.</value>
        public string OrderId { get; set; }

        /// <summary>
        /// The application fee, if the payment was created with one.
        /// </summary>
        /// <value>The application fee.</value>
        public PaymentRequestApplicationFee ApplicationFee { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the payment. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public PaymentResponseLinks Links { get; set; }

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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Id: {Id} - Status: {Status} - Method: {Method} - Amount: {Amount}";
        }
    }
}
