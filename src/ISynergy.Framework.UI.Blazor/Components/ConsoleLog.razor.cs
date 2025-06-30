using Microsoft.AspNetCore.Components;

namespace ISynergy.Framework.UI.Components;

public partial class ConsoleLog : ComponentBase
{
    private bool _expanded = true;
    protected override void OnInitialized()
    {
        //DemoLogger.OnLogHandler += OnLineReceived;

        base.OnInitialized();
    }

    private async void OnLineReceived(string text)
    {
        ConsoleContent = $"{ConsoleContent}{Environment.NewLine}[{DateTime.Now:HH:mm:ss}] - {text}";
        await InvokeAsync(() => StateHasChanged());
    }

    [Parameter]
    public string ConsoleContent { get; set; } = "";
}
