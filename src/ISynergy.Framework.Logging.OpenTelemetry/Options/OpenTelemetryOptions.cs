namespace ISynergy.Framework.Logging.Options;
public class OpenTelemetryOptions
{
    public bool IsAzureMonitorExporterEnabled { get; set; }
    public string OTLPExporterEndpoint { get; set; }
}
