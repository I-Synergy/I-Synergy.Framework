using ISynergy.Framework.Core.Abstractions.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace ISynergy.Framework.UI.Components;


public partial class NotificationCenterPanel : IDialogContentComponent<ILocalSettings>
{
    [Parameter]
    public ILocalSettings Content { get; set; } = default!;
}
