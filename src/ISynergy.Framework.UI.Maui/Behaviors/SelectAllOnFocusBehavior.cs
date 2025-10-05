namespace ISynergy.Framework.UI.Behaviors;

public class SelectAllOnFocusBehavior : Behavior<Entry>
{
    public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(SelectAllOnFocusBehavior), false, propertyChanged: OnAttachBehaviorChanged);

    public static bool GetAttachBehavior(BindableObject view) =>
        (bool)view.GetValue(AttachBehaviorProperty);

    public static void SetAttachBehavior(BindableObject view, bool value) =>
        view.SetValue(AttachBehaviorProperty, value);

    static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
    {
        Entry entry = view as Entry;

        if (entry == null)
            return;

        bool attachBehavior = (bool)newValue;

        if (attachBehavior)
            entry.Behaviors.Add(new SelectAllOnFocusBehavior());
        else
        {
            Behavior toRemove = entry.Behaviors.FirstOrDefault(b => b is SelectAllOnFocusBehavior);

            if (toRemove != null)
                entry.Behaviors.Remove(toRemove);
        }
    }

    protected override void OnAttachedTo(Entry bindable)
    {
#if ANDROID
        bindable.Loaded += Bindable_Loaded;
#else
        bindable.Focused += Bindable_Focused;
#endif
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
#if ANDROID
        bindable.Loaded -= Bindable_Loaded;
#else
        bindable.Focused -= Bindable_Focused;
#endif
    }

    private void Bindable_Focused(object sender, FocusEventArgs e)
    {
        Entry entry = (Entry)sender;

        if (!string.IsNullOrEmpty(entry.Text) && entry.Text.Length > 0)
        {
            entry.CursorPosition = 0;
            entry.SelectionLength = entry.Text.Length;
        }
    }

#if ANDROID
    private void Bindable_Loaded(object sender, EventArgs e) =>
        ((Android.Widget.EditText)((Entry)sender).Handler.PlatformView).SetSelectAllOnFocus(true);
#endif
}