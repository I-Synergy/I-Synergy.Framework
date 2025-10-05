using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;
public class AuthenticationChangedMessage(bool authenticated) : BaseMessage<bool>(authenticated);
