using ISynergy.Framework.Core.Messages.Base;
using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Messages;

public sealed class StyleChangedMessage : BaseMessage<Style>
{
    public StyleChangedMessage(Style content)
        : base(content)
    {
    }
}