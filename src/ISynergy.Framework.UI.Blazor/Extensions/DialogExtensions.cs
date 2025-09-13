using Microsoft.FluentUI.AspNetCore.Components;

namespace ISynergy.Framework.UI.Extensions;

public static class DialogExtensions
{
    public static DialogParameters GetModalDialogParameters(string? width = "auto", string? height = "auto")
    {
        return new DialogParameters
        {
            Width = width,
            Height = height,
            Modal = true,
            DialogType = DialogType.Dialog,
            TrapFocus = true,
            PreventScroll = true
        };
    }

    public static DialogParameters GetModalPanelParameters(string? width = "400px", string? height = "100%")
    {
        return new DialogParameters
        {
            Width = width,
            Height = height,
            Modal = true,
            DialogType = DialogType.Panel,
            TrapFocus = true,
            PreventScroll = true
        };
    }
}
