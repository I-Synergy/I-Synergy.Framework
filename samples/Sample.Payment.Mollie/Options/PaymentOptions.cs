namespace Mollie.Sample.Options;

/// <summary>
/// Configuration options for payment processing.
/// </summary>
public class PaymentOptions
{
    /// <summary>
    /// Gets or sets the URL to redirect to after a payment is completed.
    /// </summary>
    public string DefaultRedirectUrl { get; set; } = string.Empty;
}
