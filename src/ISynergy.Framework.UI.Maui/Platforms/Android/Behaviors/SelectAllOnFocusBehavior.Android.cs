namespace ISynergy.Framework.UI.Behaviors;

/// <summary>
/// Android half of <see cref="SelectAllOnFocusBehavior"/>: sets the native select-on-focus flag on the
/// EditText once it is realised.
/// </summary>
public partial class SelectAllOnFocusBehavior
{
    /// <inheritdoc />
    partial void OnAttachedPlatform(Entry bindable) => bindable.Loaded += Bindable_Loaded;

    /// <inheritdoc />
    partial void OnDetachedPlatform(Entry bindable) => bindable.Loaded -= Bindable_Loaded;

    private static void Bindable_Loaded(object? sender, EventArgs e)
    {
        if (sender is Entry { Handler.PlatformView: Android.Widget.EditText editText })
            editText.SetSelectAllOnFocus(true);
    }
}
