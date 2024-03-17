using ISynergy.Framework.Payment.Abstractions;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Abstractions.Services;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Clients;
using ISynergy.Framework.Payment.Mollie.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Payment.Mollie.Extensions
{
    /// <summary>
    /// Class ConfigureServicesExtension.
    /// </summary>
    public static class ConfigureServicesExtension
    {
        /// <summary>
        /// Adds the payment gateway mollie.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddPaymentGatewayMollie(this IServiceCollection services)
        {
            services.AddLogging();

            services.AddHttpClient();
            services.AddSingleton<IClientService, MollieClientService>();
            services.AddSingleton<IMollieClientService, MollieClientService>();
            services.AddSingleton<IJsonConverterService, JsonConverterService>();
            services.AddSingleton<IValidatorService, ValidatorService>();

            services.AddScoped<IChargebacksClient, ChargebacksClient>();
            services.AddScoped<IConnectClient, ConnectClient>();
            services.AddScoped<ICustomerClient, CustomerClient>();
            services.AddScoped<IInvoicesClient, InvoicesClient>();
            services.AddScoped<IMandateClient, MandateClient>();
            services.AddScoped<IOrderClient, OrderClient>();
            services.AddScoped<IOrganizationsClient, OrganizationsClient>();
            services.AddScoped<IPaymentClient, PaymentClient>();
            services.AddScoped<IPaymentMethodClient, PaymentMethodClient>();
            services.AddScoped<IPermissionsClient, PermissionsClient>();
            services.AddScoped<IProfileClient, ProfileClient>();
            services.AddScoped<IRefundClient, RefundClient>();
            services.AddScoped<ISettlementsClient, SettlementsClient>();
            services.AddScoped<ISubscriptionClient, SubscriptionClient>();

            return services;
        }
    }
}
