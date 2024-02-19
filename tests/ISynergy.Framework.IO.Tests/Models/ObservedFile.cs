namespace ISynergy.Framework.IO.Models.Tests;

/// <summary>
/// Class ObservedFile.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ObservedFile"/> class.
/// </remarks>
/// <param name="eventName">Name of the event.</param>
/// <param name="filterName">Name of the filter.</param>
/// <param name="fileName">Name of the file.</param>
public class ObservedFile(string eventName, string filterName, string fileName)
{
    /// <summary>
    /// Gets the time stamp.
    /// </summary>
    /// <value>The time stamp.</value>
    public string TimeStamp { get; private set; } = DateTime.Now.ToString("HH:mm:ss");
    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    /// <value>The name of the event.</value>
    public string EventName { get; private set; } = eventName;
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    /// <value>The name of the filter.</value>
    public string FilterName { get; private set; } = filterName;
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName { get; private set; } = fileName;
}
