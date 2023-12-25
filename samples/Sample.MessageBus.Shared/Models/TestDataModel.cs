using ISynergy.Framework.MessageBus.Abstractions.Messages;
using ISynergy.Framework.MessageBus.Enumerations;
using ISynergy.Framework.MessageBus.Models;

namespace Sample.MessageBus.Models;

/// <summary>
/// Class TestDataModel.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestDataModel"/> class.
/// </remarks>
/// <param name="action">The action.</param>
/// <param name="data">The data.</param>
public class TestDataModel(QueueMessageActions action, string data) : QueueMessage<string>(action, data), IQueueMessage<string>
{
}
