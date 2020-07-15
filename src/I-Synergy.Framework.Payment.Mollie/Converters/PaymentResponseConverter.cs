using System;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Factories;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;

namespace ISynergy.Framework.Payment.Mollie.Converters
{
    /// <summary>
    /// Class PaymentResponseConverter.
    /// Implements the <see cref="JsonCreationConverter{PaymentResponse}" />
    /// </summary>
    /// <seealso cref="JsonCreationConverter{PaymentResponse}" />
    public class PaymentResponseConverter : JsonCreationConverter<PaymentResponse>
    {
        /// <summary>
        /// The payment response factory
        /// </summary>
        private readonly PaymentResponseFactory _paymentResponseFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentResponseConverter" /> class.
        /// </summary>
        /// <param name="paymentResponseFactory">The payment response factory.</param>
        public PaymentResponseConverter(PaymentResponseFactory paymentResponseFactory)
        {
            _paymentResponseFactory = paymentResponseFactory;
        }

        /// <summary>
        /// Creates the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="jObject">The j object.</param>
        /// <returns>T.</returns>
        protected override PaymentResponse Create(Type objectType, JObject jObject)
        {
            var paymentMethod = GetPaymentMethod(jObject);

            return _paymentResponseFactory.Create(paymentMethod);
        }

        /// <summary>
        /// Gets the payment method.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns>System.Nullable&lt;PaymentMethods&gt;.</returns>
        private PaymentMethods? GetPaymentMethod(JObject jObject)
        {
            if (FieldExists("method", jObject))
            {
                var paymentMethodValue = (string)jObject["method"];
                if (!string.IsNullOrEmpty(paymentMethodValue))
                {
                    return (PaymentMethods)Enum.Parse(typeof(PaymentMethods), paymentMethodValue, true);
                }
            }

            return null;
        }
    }
}
