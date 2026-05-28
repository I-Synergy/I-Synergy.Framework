using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Mvvm.Messages;
public class ShowWarningMessage(MessageBoxRequest request) : BaseMessage<MessageBoxRequest>(request);
