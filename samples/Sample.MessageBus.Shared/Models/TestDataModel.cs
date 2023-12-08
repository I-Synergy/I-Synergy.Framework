using ISynergy.Framework.MessageBus.Abstractions.Messages;
using ISynergy.Framework.MessageBus.Enumerations;
using ISynergy.Framework.MessageBus.Models;

namespace Sample.MessageBus.Models;

/// <summary>
/// Class TestDataModel.
/// </summary>
public class TestDataModel : QueueMessage<string>, IQueueMessage<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestDataModel"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="data">The data.</param>
    public TestDataModel(QueueMessageActions action, string data)
        : base(action, data)
    {
    }
}
