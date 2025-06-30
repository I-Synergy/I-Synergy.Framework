using ISynergy.Framework.Core.Messages.Base;
using ISynergy.Framework.UI.Requests;

namespace ISynergy.Framework.UI.Messages;
public class ShowInformationMessage(MessageBoxRequest request) : BaseMessage<MessageBoxRequest>(request);
public class ShowWarningMessage(MessageBoxRequest request) : BaseMessage<MessageBoxRequest>(request);
public class ShowErrorMessage(MessageBoxRequest request) : BaseMessage<MessageBoxRequest>(request);
