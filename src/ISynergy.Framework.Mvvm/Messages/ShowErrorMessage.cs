using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Mvvm.Messages;
public class ShowErrorMessage(MessageBoxRequest request) : BaseMessage<MessageBoxRequest>(request);
