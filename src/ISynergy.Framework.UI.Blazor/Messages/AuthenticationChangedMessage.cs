using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.UI.Messages;
public class AuthenticationChangedMessage(bool authenticated) : BaseMessage<bool>(authenticated);
