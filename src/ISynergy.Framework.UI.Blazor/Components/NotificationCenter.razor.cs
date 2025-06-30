using ISynergy.Framework.Core.Abstractions.Base;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Components;

public partial class NotificationCenter : IDisposable
{
    private IDialogReference? _dialog;

    protected override void OnInitialized()
    {
        MessageService.OnMessageItemsUpdated += UpdateCount;
    }

    private void UpdateCount()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task OpenNotificationCenterAsync()
    {
        Debug.WriteLine($"Open notification center");

        _dialog = await DialogService.ShowPanelAsync<NotificationCenterPanel>(new DialogParameters<ILocalSettings>()
        {
            Alignment = HorizontalAlignment.Right,
            Title = $"Notifications",
            PrimaryAction = null,
            SecondaryAction = null,
            ShowDismiss = true
        });
        DialogResult result = await _dialog.Result;
        HandlePanel(result);
    }

    private static void HandlePanel(DialogResult result)
    {
        if (result.Cancelled)
        {
            Debug.WriteLine($"Notification center dismissed");
            return;
        }

        if (result.Data is not null)
        {
            Debug.WriteLine($"Notification center closed");
            return;
        }
    }

    public void Dispose()
    {
        MessageService.OnMessageItemsUpdated -= UpdateCount;
    }

}
