namespace ISynergy.Framework.UI.Behaviors;

public partial class SelectAllOnFocusBehavior : Behavior<Entry>
{
    public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(SelectAllOnFocusBehavior), false, propertyChanged: OnAttachBehaviorChanged);

    public static bool GetAttachBehavior(BindableObject view) =>
        (bool)view.GetValue(AttachBehaviorProperty);

    public static void SetAttachBehavior(BindableObject view, bool value) =>
        view.SetValue(AttachBehaviorProperty, value);

    static void OnAttachBehaviorChanged(BindableObject view, object? oldValue, object? newValue)
    {
        var entry = view as Entry;

        if (entry == null)
            return;

        bool attachBehavior = (bool)(newValue ?? false);

        if (attachBehavior)
            entry.Behaviors.Add(new SelectAllOnFocusBehavior());
        else
        {
            var toRemove = entry.Behaviors.FirstOrDefault(b => b is SelectAllOnFocusBehavior);

            if (toRemove != null)
                entry.Behaviors.Remove(toRemove);
        }
    }

    protected override void OnAttachedTo(Entry bindable)
    {
        bindable.Focused += Bindable_Focused;
#if ANDROID
        OnAttachedPlatform(bindable);
#endif
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        bindable.Focused -= Bindable_Focused;
#if ANDROID
        OnDetachedPlatform(bindable);
#endif
    }

#if ANDROID
    /// <summary>
    /// Extra attachment work for the Android head: adds the native select-on-focus flag to
    /// the EditText. Other heads rely on the Focused handler above.
    /// </summary>
    /// <param name="bindable">The entry the behaviour is attached to.</param>
    partial void OnAttachedPlatform(Entry bindable);

    /// <summary>
    /// Undoes <see cref="OnAttachedPlatform"/>.
    /// </summary>
    /// <param name="bindable">The entry the behaviour is detached from.</param>
    partial void OnDetachedPlatform(Entry bindable);
#endif

    private static void Bindable_Focused(object? sender, FocusEventArgs e)
    {
        var entry = sender as Entry;

        if (entry == null)
            return;

        if (!string.IsNullOrEmpty(entry.Text) && entry.Text.Length > 0)
        {
            entry.CursorPosition = 0;
            entry.SelectionLength = entry.Text.Length;
        }
    }

}