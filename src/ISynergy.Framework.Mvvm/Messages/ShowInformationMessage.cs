using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Mvvm.Messages;
public class ShowInformationMessage(MessageBoxRequest request) : BaseMessage<MessageBoxRequest>(request);
