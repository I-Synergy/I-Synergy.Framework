namespace Sample.Components.Pages;
public partial class Counter
{
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
        StateHasChanged();
    }
}