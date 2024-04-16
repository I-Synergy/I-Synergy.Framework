using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Messaging;

public class StyleChangedMessage : Message<Style>
{
    public StyleChangedMessage(Style content)
        : base(content)
    {
    }
}
