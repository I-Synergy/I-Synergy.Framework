namespace Sample.Abstractions;

/// <summary>
/// Interface IPrintingService
/// </summary>
public interface IPrintingService
{
    /// <summary>
    /// Prints the dymo label.
    /// </summary>
    /// <param name="content">The content.</param>
    void PrintDymoLabel(string content);
}
